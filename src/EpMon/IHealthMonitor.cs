namespace EpMon
{
    public interface IHealthMonitor
    {
        string Name { get; }
        HealthInfo CheckHealth(string address);
    }
}
