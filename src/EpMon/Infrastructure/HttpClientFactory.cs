using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace EpMon.Infrastructure
{
    //public interface  IHttpClientFactory
    //{
    //    HttpClient Create(Uri baseAddress);
    //}

    //public sealed class HttpClientFactory : IDisposable, IHttpClientFactory // Register as singleton
    //{
    //    private readonly ConcurrentDictionary<Uri, HttpClient> _httpClients;

    //    public HttpClientFactory()
    //    {
    //        _httpClients = new ConcurrentDictionary<Uri, HttpClient>();
    //    }

    //    public void Dispose()
    //    {
    //        foreach (var httpClient in _httpClients.Values) httpClient.Dispose();
    //    }

    //    private HttpClient Create(Uri baseAddress, HttpMessageHandler handler)
    //    {
    //        //http://byterot.blogspot.com/2016/07/singleton-httpclient-dns.html
    //        var sp = ServicePointManager.FindServicePoint(baseAddress);
    //        if(sp.ConnectionLeaseTimeout == -1)
    //            sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute

    //        return _httpClients.GetOrAdd(baseAddress,
    //            b => CreateHttpClient(handler, b));
    //    }

    //    private static HttpClient CreateHttpClient(HttpMessageHandler handler, Uri b)
    //    {
    //        var client = new HttpClient(handler)
    //        {
    //            BaseAddress = b, 
    //            Timeout = TimeSpan.FromSeconds(30)
    //        };

    //        return client;
    //    }

    //    public HttpClient Create(Uri baseAddress)
    //    {
    //        var defaultHandler = new HttpClientHandler {UseDefaultCredentials = true};
    //        return Create(baseAddress, defaultHandler);
    //    }
    //}
}