using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace EpMon
{
    public class HttpMonitor : IHealthMonitor
    {
        public static HttpClientFactory HttpClientFactory;
        public HttpMonitor() : this("http") { }

        public HttpMonitor(string name)
        {
            Name = name;
            HttpClientFactory = new HttpClientFactory();

            ServicePointManager.DefaultConnectionLimit = 100;
        }
        public HttpMonitor(string name, HttpClientFactory httpClientFactory)
        {
            Name = name;
            HttpClientFactory = httpClientFactory;

            ServicePointManager.DefaultConnectionLimit = 100;
        }

        public string Name { get; }
        
        public HealthInfo CheckHealth(string address)
        {
            var baseUri = GetBaseUri(address);
            var httpClient = HttpClientFactory.Create(new Uri(baseUri));

            using (var response = httpClient.GetAsync(address).Result)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new HealthInfo(HealthStatus.NotExists);
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    return new HealthInfo(HealthStatus.Offline);
                if (response.IsSuccessStatusCode)
                    return new HealthInfo(HealthStatus.Healthy, ReadContent(response));

                return new HealthInfo(HealthStatus.Faulty, ReadContent(response));
            }
        }

        private static string GetBaseUri(string address)
        {
            var uri = new Uri(address);
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            return baseUri;
        }

        private IReadOnlyDictionary<string, string> ReadContent(HttpResponseMessage response)
        {
            return new Dictionary<string, string>
            {
                {"code", response.StatusCode.ToString()},
                {"content", response.Content.ReadAsStringAsync().Result},
            };
        }
    }
}
