﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EpMon
{
    public class HttpMonitor : IHealthMonitor
    {
        public HttpClientFactory HttpClientFactory;

        public HttpMonitor() : this("http")
        {
        }

        public HttpMonitor(string name)
        {
            Name = name;
            HttpClientFactory = new HttpClientFactory();
        }

        public HttpMonitor(string name, HttpClientFactory httpClientFactory)
        {
            Name = name;
            HttpClientFactory = httpClientFactory;
        }

        public string Name { get; }

        public HealthInfo CheckHealth(string address)
        {
            var baseUri = UriHelper.GetBaseUri(address);
            var httpClient = HttpClientFactory.Create(new Uri(baseUri));

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