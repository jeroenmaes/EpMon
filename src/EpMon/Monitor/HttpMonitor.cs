using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using EpMon.Helpers;
using EpMon.Infrastructure;
using EpMon.Model;
using IdentityModel.Client;

namespace EpMon.Monitor
{
    public class HttpMonitor : IHealthMonitor
    {
        public IHttpClientFactory HttpClientFactory;
        public ITokenService TokenService;

        public HttpMonitor() : this("http")
        {
        }

        public HttpMonitor(string name)
        {
            Name = name;
            HttpClientFactory = new HttpClientFactory();
        }

        public HttpMonitor(string name, IHttpClientFactory httpClientFactory, ITokenService tokenService)
        {
            Name = name;
            HttpClientFactory = httpClientFactory;
            TokenService = tokenService;
        }

        public string Name { get; }
        
        public HealthInfo CheckHealth(string address)
        {
            var baseUri = UriHelper.GetBaseUri(address);
            var httpClient = HttpClientFactory.Create(new Uri(baseUri));
            httpClient.SetBearerToken(TokenService.GetToken());

            var response = AsyncHelper.RunSync(() => httpClient.GetAsync(address));

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new HealthInfo(HealthStatus.NotExists);
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                return new HealthInfo(HealthStatus.Offline);
            if (response.IsSuccessStatusCode)
                return new HealthInfo(HealthStatus.Healthy, ReadContent(response));

            return new HealthInfo(HealthStatus.Faulty, ReadContent(response));
        }

        private IReadOnlyDictionary<string, string> ReadContent(HttpResponseMessage response)
        {
            var response2 = AsyncHelper.RunSync(() => response.Content.ReadAsStringAsync());

            return new Dictionary<string, string>
            {
                {"code", response.StatusCode.ToString()},
                {"content", response2}
            };
        }
    }
}