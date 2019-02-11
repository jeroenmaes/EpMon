using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using EpMon.Data.Entities;

namespace EpMon.Data
{
    class EpMonContext : DbContext
    {
        public EpMonContext() : base("name=EpMonContext")
        {
            Database.SetInitializer<EpMonContext>(new CreateDatabaseIfNotExists<EpMonContext>());
            
        }
        
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<EndpointStat> EndpointStats { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEndpointStats(modelBuilder.Entity<EndpointStat>());
        }
        
        private void ConfigureEndpointStats(EntityTypeConfiguration<EndpointStat> config)
        {
            
        }
    }
}
