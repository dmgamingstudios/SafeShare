﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Knapcode.TorSharp.Tools.Privoxy
{
    public class PrivoxyFetcher : IFileFetcher
    {
        private static readonly Uri BaseUrl = new Uri("http://sourceforge.net/projects/ijbswa/rss?path=/Win32");
        private readonly HttpClient _httpClient;

        public PrivoxyFetcher(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DownloadableFile> GetLatestAsync()
        {
            SyndicationFeed syndicationFeed;
            using (var stream = await _httpClient.GetStreamAsync(BaseUrl).ConfigureAwait(false))
            {
                var streamReader = new StreamReader(stream);
                var xmlReader = XmlReader.Create(streamReader);
                syndicationFeed = SyndicationFeed.Load(xmlReader);
                if (syndicationFeed == null)
                {
                    return null;
                }
            }

            var latest = syndicationFeed
                .Items
                .Where(i => i.Links.Any())
                .OrderByDescending(i => i.PublishDate)
                .FirstOrDefault(IsMatch);

            if (latest == null)
            {
                return null;
            }

            var name = latest.Title.Text.Split('/').Last();
            return new DownloadableFile
            {
                Name = name,
                GetContentAsync = () => _httpClient.GetStreamAsync(latest.Links.First().Uri)
            };
        }

        private bool IsMatch(SyndicationItem item)
        {
            return Regex.IsMatch(item.Title.Text, @"privoxy-[\d\.]+.zip$", RegexOptions.IgnoreCase);
        }
    }
}