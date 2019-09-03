using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using EpMon.Data.Entities;
using EpMon.Model;


namespace EpMon
{
    public class EndpointService
    {
        private readonly EndpointStore _store;

        public EndpointService(EndpointStore store)
        {
            _store = store;
        }
        
        public IEnumerable<Model.Endpoint> GetAllEndpoints()
        {
            return _store.GetAllEndpoints().Select(x => new Model.Endpoint { Name = x.Name });
        }
    }
}
