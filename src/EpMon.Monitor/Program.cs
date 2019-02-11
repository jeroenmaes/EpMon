using System;
using FluentScheduler;

namespace EpMon.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                JobManager.Initialize(new MonitorRegistry());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
