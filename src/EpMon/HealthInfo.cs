using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpMon
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
