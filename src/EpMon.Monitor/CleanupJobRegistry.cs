using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using EpMon.Data;
using FluentScheduler;
using System.Linq;

namespace EpMon.Monitor
{
    public class CleanupJobRegistry : Registry
    {
        private readonly EpMonRepository _epMonRepository;

        public CleanupJobRegistry(IServiceProvider serviceProvider)
        {
            _epMonRepository = serviceProvider.GetService<EpMonRepository>();
            
            NonReentrantAsDefault();
            
            Schedule(() => CleanData()).WithName("CleanData").ToRunNow().AndEvery(1).Hours();
        }
        
        public void CleanData()
        {
            _epMonRepository.CleanStats(30);
        }
    }
}
