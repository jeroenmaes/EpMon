using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CronScheduler.Extensions.StartupInitializer;
using EpMon.Data;
using Microsoft.EntityFrameworkCore;

namespace EpMon.Web.Jobs
{
    public class MigrateDatabaseJob : IStartupJob
    {
        private readonly EpMonContext _context;
        public MigrateDatabaseJob(EpMonContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Console.WriteLine("Run Database migration...");

                _context.Database.Migrate();

                Console.WriteLine("Database migrated.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.CompletedTask;
        }
    }
}
