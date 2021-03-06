﻿using System;
using System.Net;

namespace EpMon.Data.Entities
{
    public class EndpointStat : IEntity
    {
        public int Id { get; set; }

        public long ResponseTime { get; set; }
        public HttpStatusCode Status { get; set; }
        public bool IsHealthy { get; set; }
        public DateTime TimeStamp { get; set; }
                
        public string Message { get; set; }

        public int EndpointId { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
