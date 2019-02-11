using System.Data.Entity.Migrations;

namespace EpMon.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EpMonContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(EpMonContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
