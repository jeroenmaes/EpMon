using CronScheduler.Extensions.StartupInitializer;
using EpMon.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon.Web.Jobs
{
    public class MigrateDatabaseJob : IStartupJob
    {
        private readonly EpMonContext _context;
        private readonly ILogger<MigrateDatabaseJob> _logger;

        public MigrateDatabaseJob(EpMonContext context, ILogger<MigrateDatabaseJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Run Database migration...");

                await _context.Database.MigrateAsync(cancellationToken);

                _logger.LogInformation("Database migrated.");

                if (! await _context.Endpoints.AnyAsync(cancellationToken))
                {
                    _context.Endpoints.Add(new Data.Entities.Endpoint { CheckInterval = 1, CheckType = Data.Entities.CheckType.AvailabilityCheck, IsActive = true, IsCritical = true, Name = "nuget.org", Url = "https://nuget.org", Tags = "nuget" });
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            await Task.CompletedTask;
        }
    }
}
