using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace EpMon
{
    public sealed class HttpClientFactory : IDisposable // Register as singleton
    {
        private readonly ConcurrentDictionary<Uri, HttpClient> _httpClients;

        public HttpClientFactory()
        {
            _httpClients = new ConcurrentDictionary<Uri, HttpClient>();
        }

        public void Dispose()
        {
            foreach (var httpClient in _httpClients.Values) httpClient.Dispose();
        }

        private HttpClient Create(Uri baseAddress, HttpClientHandler handler)
        {
            return _httpClients.GetOrAdd(baseAddress,
                b => new HttpClient(handler) {BaseAddress = b});
        }

        public HttpClient Create(Uri baseAddress)
        {
            var defaultHandler = new HttpClientHandler {UseDefaultCredentials = true};
            return Create(baseAddress, defaultHandler);
        }
    }
}