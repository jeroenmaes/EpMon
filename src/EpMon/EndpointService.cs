using EpMon.Data;
using EpMon.Model;
using System.Collections.Generic;
using System.Linq;


namespace EpMon
{
    public class EndpointService
    {
        private readonly EndpointStore _store;
        
        public EndpointService(EndpointStore store)
        {
            _store = store;
        }

        public IEnumerable<Model.Endpoint> GetAllActiveEndpoints()
        {
            return _store.GetAllEndpoints().Where(x => x.IsActive).Select(x => new Model.Endpoint { Name = x.Name, Id = x.Id, Url = x.Url, IsActive = x.IsActive, CheckInterval = x.CheckInterval, CheckType = (int)x.CheckType, Tags = x.Tags, IsCritical = x.IsCritical, PublishStats = x.PublishStats});
        }

        public void SaveHealthReport(int endpointId, HealthReport report)
        {
            _store.AddEndpointStat(new Data.Entities.EndpointStat { EndpointId = endpointId, IsHealthy = report.IsHealthy, Message = report.Message, ResponseTime = report.ResponseTime, TimeStamp = report.TimeStamp, Status = report.Status });
        }
    }
}
