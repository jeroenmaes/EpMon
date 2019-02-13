using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using EpMon.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EpMon.Data
{
    public class EpMonContext : DbContext
    {
        public EpMonContext() : base()
        {
            Database.Migrate();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["EpMonContext"].ConnectionString);
        }

        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<EndpointStat> EndpointStats { get; set; }
    }

}
