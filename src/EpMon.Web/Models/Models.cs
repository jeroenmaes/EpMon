using System.Collections.Generic;
using EpMon.Data.Entities;

namespace EpMon.Web.Models
{
    public class EndpointsOverview
    {
        
        public Dictionary<string, List<Endpoint>> EndpointsByTag { set; get; }
        
        public bool UnHealthyEndpoints { get; set; }
    }

    public class EndpointDetails
    {
        public Endpoint Endpoint { get; set; }
        public IEnumerable<EndpointStat> Stats { set; get; }
      
        public string ResponseTimeData { get; set; }
        public string UptimeData { get; set; }
        public double ResponseTime { get; set; }

        public double Uptime { get; set; }
        public EndpointStat LastStat { get; set; }
    }
}