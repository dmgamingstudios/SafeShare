using FuhrerShare.Core.Networking.Clients;
using SecuProxy.SecuNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace SecuProxy
{
    public class SecuProxy
    {
        SafeNode[] nodes;
        ProxyServer proxyServer = new ProxyServer();
        public void StartProxy(int port, SafeNode[] nodes)
        {
            this.nodes = nodes;
            var PEP = new ExplicitProxyEndPoint(IPAddress.Any, port, false) { };
            proxyServer.TrustRootCertificate = true;
            proxyServer.BeforeRequest += OnRequest;
            proxyServer.BeforeResponse += OnResponse;
            proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;
            proxyServer.AddEndPoint(PEP);
            proxyServer.Start();
            foreach (var endPoint in proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);
            proxyServer.SetAsSystemHttpProxy(PEP);
            proxyServer.SetAsSystemHttpsProxy(PEP);
        }
        public void StopProxy()
        {
            proxyServer.BeforeRequest -= OnRequest;
            proxyServer.BeforeResponse -= OnResponse;
            proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
            proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;
            proxyServer.Stop();
        }
        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            Console.WriteLine(e.WebSession.Request.Url);
            var requestHeaders = e.WebSession.Request.RequestHeaders;
            var method = e.WebSession.Request.Method.ToUpper();
            if ((method == "POST" || method == "PUT" || method == "PATCH"))
            {
                byte[] bodyBytes = await e.GetRequestBody();
                await e.SetRequestBody(bodyBytes);
                string bodyString = await e.GetRequestBodyAsString();
                await e.SetRequestBodyString(bodyString);
            }
            if (!e.WebSession.Request.RequestUri.AbsoluteUri.Contains(".secunet"))
            {
                await e.Ok("<!DOCTYPE html>" +
                      "<html><title>ClearLinks Disallowed</title><body><h1>" +
                      "Access Denied" +
                      "</h1>" +
                      "<p>Blocked by SecuNet Proxy, powered by titanium web proxy</p>" +
                      "<p>We do NOT provide exit nodes, and we never will make them, if you are crazy enough to try to make exit nodes into this. Then go ahead</p>" +
                      "</body>" +
                      "</html>");
            }
            string Site = new MessageHandler().RequestSecuNetWebSite("", "", nodes);
            await e.Ok(Site);
        }
        public async Task OnResponse(object sender, SessionEventArgs e)
        {
            var responseHeaders = e.WebSession.Response.ResponseHeaders;
            if (e.WebSession.Request.Method == "GET" || e.WebSession.Request.Method == "POST")
            {
                if (e.WebSession.Response.ResponseStatusCode == "200")
                {
                    if (e.WebSession.Response.ContentType != null && e.WebSession.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        byte[] bodyBytes = await e.GetResponseBody();
                        await e.SetResponseBody(bodyBytes);
                        string body = await e.GetResponseBodyAsString();
                        await e.SetResponseBodyString(body);
                    }
                }
            }
        }
        public Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;
            return Task.FromResult(0);
        }
        public Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            return Task.FromResult(0);
        }
    }
}