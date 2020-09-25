using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EpMon.Web.ViewModels
{
    public class EndpointTags
    {
        public List<string> Tags { set; get; }
    }
    
    public class EndpointsOverviewViewModel
    {
       public List<EndpointDto> Endpoints { get; set; }
        
       public string TagName { set; get; }
    }

    public class EndpointDetailsViewModel
    {
        public EndpointDto Endpoint { get; set; }
        public IEnumerable<EndpointStatDto> Stats { set; get; }
      
        public string ResponseTimeData { get; set; }
        public string UptimeData { get; set; }
        public EndpointStatDto LastStat { get; set; }
    }

    public class EndpointStatDto
    {
        public int Id { get; set; }

        public long ResponseTime { get; set; }
        public HttpStatusCode Status { get; set; }
        public bool IsHealthy { get; set; }
        public DateTime TimeStamp { get; set; }

        public string Message { get; set; }

        public int EndpointId { get; set; }
        public EndpointDto Endpoint { get; set; }
    }

    public class EndpointDto
    {

        public int Id { get; set; }
        public int CheckInterval { get; set; }
        public CheckType CheckType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }
        public double ResponseTime { get; set; }
        public double Uptime { get; set; }
        public bool IsActive { get; set; }

        public bool IsCritical { get; set; }
        public bool PublishStats { get; set; }
        public ICollection<EndpointStatDto> Stats { get; set; }
    }

    public enum CheckType
    {
        AvailabilityCheck,
        ContentCheck
    }
}