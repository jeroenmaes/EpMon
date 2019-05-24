using EpMon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EpMon.Data
{
    public partial class EpMonContext : DbContext
    {
        private static string _connectionString;

        public EpMonContext() : base()
        {
            LoadConnectionString();
            
           // Database.Migrate();
        }

        public EpMonContext(DbContextOptions<EpMonContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            LoadConnectionString();

            //optionsBuilder.UseSqlServer(_connectionString, options => options.EnableRetryOnFailure());

            optionsBuilder.UseSqlServer(_connectionString, options => options.MigrationsAssembly("EpMon.Data"));
        }


        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", false);

            var configuration = builder.Build();

            _connectionString = configuration.GetConnectionString("EpMonConnection");
        }

        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<EndpointStat> EndpointStats { get; set; }
    }

}
