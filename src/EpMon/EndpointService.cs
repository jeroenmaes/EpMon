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

        private readonly GenericRepository<EpMon.Data.Entities.Endpoint> _endpointRepository;
        private readonly GenericRepository<EpMon.Data.Entities.EndpointStat> _endpointStatRepository;

        public EndpointService(EndpointStore store)
        {
            _store = store;
        }
        
        public IEnumerable<Model.Endpoint> GetAllActiveEndpoints()
        {
            return _store.GetAllEndpoints().Where(x => x.IsActive).Select(x => new Model.Endpoint { Name = x.Name, Id = x.Id, Url = x.Url, IsActive = x.IsActive, CheckInterval = x.CheckInterval, CheckType = (int)x.CheckType, Tags = x.Tags, IsCritical = x.IsCritical});
        }

        public void SaveHealthReport(int endpointId, HealthReport report)
        {
            _store.AddEndpointStat(new Data.Entities.EndpointStat { EndpointId = endpointId, IsHealthy = report.IsHealthy, Message = report.Message, ResponseTime = report.ResponseTime, TimeStamp = report.TimeStamp, Status = report.Status });
        }
    }
}
