using EpMon.Helpers;
using EpMon.Infrastructure;
using EpMon.Model;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace EpMon.Monitor
{
    public class HttpMonitor : IHealthMonitor
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ITokenService TokenService;
        private readonly double _timeout = 30;
        private readonly ILogger _logger;

        public HttpMonitor() : this("http")
        {
        }

        public HttpMonitor(string name)
        {
            Name = name;
        }

        public HttpMonitor(string name, IHttpClientFactory httpClientFactory, ITokenService tokenService, double timeoutInSeconds, ILogger logger)
        {
            Name = name;
            HttpClientFactory = httpClientFactory;
            TokenService = tokenService;
            _timeout = timeoutInSeconds;
            _logger = logger;
        }

        public string Name { get; }
        
        public HealthInfo CheckHealth(string address)
        {
            try
            {
                var baseUri = UriHelper.GetBaseUri(address);
                
                var httpClient = GetHttpClient(baseUri);
                
                var token = TokenService?.GetToken();

                if (token != string.Empty)
                {
                    httpClient.SetBearerToken(token);
                }
                
                var response = AsyncHelper.RunSync(() => httpClient.GetAsync(address));
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new HealthInfo(HealthStatus.NotExists);
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    return new HealthInfo(HealthStatus.Offline);
                if (response.IsSuccessStatusCode)
                    return new HealthInfo(HealthStatus.Healthy, ReadContent(response));

                return new HealthInfo(HealthStatus.Faulty, ReadContent(response));
            }
            catch (Exception e)
            {
                _logger?.LogError(e, e.Message);

                var content = new Dictionary<string, string>
                {
                    {"code","500"},
                    {"content", e.Message}
                };

                return new HealthInfo(HealthStatus.Faulty, content);
            }
        }

        private HttpClient GetHttpClient(string baseUri)
        {
            if(HttpClientFactory == null)
                return new HttpClient();

            var client = HttpClientFactory.CreateClient("default");
            client.BaseAddress = new Uri(baseUri);
            client.Timeout = TimeSpan.FromSeconds(_timeout);

            return client;
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