using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon
{
    public interface IHealthMonitor
    {
        string Name { get; }
        HealthInfo CheckHealth(string address);
    }
}
