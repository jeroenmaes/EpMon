using EpMon.Model;

namespace EpMon.Monitor
{
    public interface IHealthMonitor
    {
        string Name { get; }
        HealthInfo CheckHealth(string address);
    }
}