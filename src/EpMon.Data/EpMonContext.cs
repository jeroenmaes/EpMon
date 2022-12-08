using System;
using System.Linq;
using System.Reflection;
using EpMon.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EpMon.Data
{
    public partial class EpMonContext : DbContext
    {
        private static string _connectionString;

        public EpMonContext() : base()
        {
            ChangeTracker.LazyLoadingEnabled = false;

            LoadConnectionString();
        }

        public EpMonContext(DbContextOptions<EpMonContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            LoadConnectionString();

            if (_connectionString.Contains(".db"))
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
            else
            {
                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                optionsBuilder.UseSqlServer(_connectionString, options =>
                    options.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null));

                optionsBuilder.UseSqlServer(_connectionString, options =>
                    options.MigrationsAssembly(typeof(EpMonContext).GetTypeInfo().Assembly.GetName().Name));
            }
        }


        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", true);
            builder.AddJsonFile("appsettings.Development.json", true);

            var configuration = builder.Build();

            _connectionString = configuration.GetConnectionString("EpMonConnection");
        }
        
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<EndpointStat> EndpointStats { get; set; }
    }

}
