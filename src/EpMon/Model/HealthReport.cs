using System;
using System.Net;

namespace EpMon.Model
{
    public class HealthReport
    {
        public DateTime TimeStamp { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; }
        public HttpStatusCode Status { get; set; }
        public bool IsHealthy { get; set; }
    }
}