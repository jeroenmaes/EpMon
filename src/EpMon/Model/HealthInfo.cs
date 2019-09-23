using System.Collections.Generic;

namespace EpMon.Model
{
    public class HealthInfo
    {
        public HealthInfo(HealthStatus status, IReadOnlyDictionary<string, string> details = null)
        {
            Status = status;
            Details = details ?? new Dictionary<string, string>();
        }

        public HealthStatus Status { get; }
        public IReadOnlyDictionary<string, string> Details { get; }
    }
}