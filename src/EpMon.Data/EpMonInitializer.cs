using System.Data.Entity;
using EpMon.Data.Entities;

namespace EpMon.Data
{
    public class EpMonInitializer : CreateDatabaseIfNotExists<EpMonContext>
    {
        protected override void Seed(EpMonContext context)
        {
            var endpoint = new Endpoint
            {
                CheckInterval = 5, CheckType = CheckType.AvailabilityCheck, Tags = "Personal",
                Url = @"http:\\blog.jeroenmaes.eu"
            };

            context.Endpoints.Add(endpoint);

            base.Seed(context);
        }
    }
}