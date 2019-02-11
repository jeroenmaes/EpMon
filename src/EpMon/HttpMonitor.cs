using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon
{
    public class HttpMonitor : IHealthMonitor
    {
        public HttpMonitor() : this("http") { }

        protected HttpMonitor(string name)
        {
            Name = name;

            ServicePointManager.DefaultConnectionLimit = 100;
        }

        public string Name { get; }
        
        public HealthInfo CheckHealth(string address)
        {
            try
            {
                var request = WebRequest.Create(new Uri(address)) as HttpWebRequest;
                if (request != null)
                {
                    request.Proxy = null;

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        return new HealthInfo(HealthStatus.Healthy, ReadContent(response));
                    }
                }
            }
            catch (WebException ex)
            {
                using (var errorResponse = (HttpWebResponse)ex.Response)
                {
                    if (errorResponse != null)
                    {
                        if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                            return new HealthInfo(HealthStatus.NotExists);

                        if (errorResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
                            return new HealthInfo(HealthStatus.Offline);

                        return new HealthInfo(HealthStatus.Faulty, ReadContent(errorResponse));
                    }
                }
            }

            return new HealthInfo(HealthStatus.Faulty);
        }

        private IReadOnlyDictionary<string, string> ReadContent(HttpWebResponse response)
        {
            string content;
            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream);
                content = reader.ReadToEnd();
            }
            
            return new Dictionary<string, string>
            {
                {"code", response.StatusCode.ToString()},
                {"content", content}
            };
        }
    }
}
