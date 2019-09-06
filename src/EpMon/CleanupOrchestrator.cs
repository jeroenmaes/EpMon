using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using EpMon.Data;
using FluentScheduler;
using System.Linq;

namespace EpMon
{
    public class CleanupOrchestrator : Registry
    {
        private readonly EndpointStore _store;

        public CleanupOrchestrator(IServiceProvider serviceProvider)
        {
            _store = serviceProvider.GetService<EndpointStore>();
            
            NonReentrantAsDefault();
            
            Schedule(() => CleanData()).WithName("CleanData").ToRunNow().AndEvery(1).Hours();
        }
        
        public void CleanData()
        {
            _store.RemoveEndpointStatsByDaysToKeep(30);
        }
    }
}
