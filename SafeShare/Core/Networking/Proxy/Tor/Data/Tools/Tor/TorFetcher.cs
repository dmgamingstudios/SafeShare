﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Knapcode.TorSharp.Tools.Tor
{
    public class TorFetcher : IFileFetcher
    {
        private static readonly Uri BaseUrl = new Uri("https://dist.torproject.org/torbrowser/");
        private readonly HttpClient _httpClient;

        public TorFetcher(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DownloadableFile> GetLatestAsync()
        {
            var versionsContent = await _httpClient.GetStringAsync(BaseUrl).ConfigureAwait(false);
            var latestVersion = GetLinks(versionsContent)
                .Select(x => new {Link = x, Version = GetVersion(x)})
                .Where(x => x.Version != null)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();
            if (latestVersion == null)
            {
                return null;
            }

            var listUrl = new Uri(BaseUrl, latestVersion.Link);
            var listContent = await _httpClient.GetStringAsync(listUrl).ConfigureAwait(false);
            var name = GetLinks(listContent).FirstOrDefault(x => Regex.IsMatch(x, @"^tor-win32-.+\.zip$"));
            if (name == null)
            {
                return null;
            }

            var downloadUrl = new Uri(listUrl, name);
            return new DownloadableFile
            {
                Name = name,
                GetContentAsync = () => _httpClient.GetStreamAsync(downloadUrl)
            };
        }

        private IEnumerable<string> GetLinks(string content)
        {
            return Regex
                .Matches(content, @"<a[^>]+?href=""(?<Link>[^""]+)"">")
                .OfType<Match>()
                .Select(x => x.Groups["Link"].Value);
        }

        private Version GetVersion(string link)
        {
            var last = Regex
                .Matches(link, @"(?<Version>\d+(?:\.\d+)+)/")
                .OfType<Match>()
                .LastOrDefault();

            if (last == null)
            {
                return null;
            }

            Version version;
            if (!Version.TryParse(last.Groups["Version"].Value, out version))
            {
                return null;
            }

            return version;
        }
    }
}