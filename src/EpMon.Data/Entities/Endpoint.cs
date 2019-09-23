using System;
using System.Collections.Generic;

namespace EpMon.Data.Entities
{
    public class Endpoint : IEntity, ITrackableEntity
    {
        public int Id { get; set; }
        public int CheckInterval { get; set; }
        public CheckType CheckType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }

        public bool IsActive { get; set; }

        public bool IsCritical { get; set; }

        public ICollection<EndpointStat> Stats { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }

    public enum CheckType
    {
        AvailabilityCheck,
        ContentCheck
    }
}
