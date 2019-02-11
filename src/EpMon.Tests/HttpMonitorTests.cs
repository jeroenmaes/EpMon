using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EpMon.Tests
{
    [TestClass]
    public class HttpMonitorTests
    {
        [TestMethod]
        public void Test_200()
        {
            var monitor = new HttpMonitor();
            var result = monitor.CheckHealth(@"https://httpstat.us/200");

            Assert.AreEqual(HealthStatus.Healthy, result.Status);
        }

        [TestMethod]
        public void Test_404()
        {
            var monitor = new HttpMonitor();
            var result = monitor.CheckHealth(@"https://httpstat.us/404");

            Assert.AreEqual(HealthStatus.NotExists, result.Status);
        }

        [TestMethod]
        public void Test_500()
        {
            var monitor = new HttpMonitor();
            var result = monitor.CheckHealth(@"https://httpstat.us/500");

            Assert.AreEqual(HealthStatus.Faulty, result.Status);
        }

        [TestMethod]
        public void Test_503()
        {
            var monitor = new HttpMonitor();
            var result = monitor.CheckHealth(@"https://httpstat.us/503");

            Assert.AreEqual(HealthStatus.Offline, result.Status);
        }
    }
}
