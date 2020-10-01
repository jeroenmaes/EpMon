using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using EpMon.Data.Entities;
using EpMon.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EpMon.Web.Api
{
    public class EndpointsController : BaseApiController
    {
        private readonly EndpointStore _store;
        
        public EndpointsController(EndpointStore store)
        {
            _store = store;
        }

        [HttpPost("/endpoints")]
        public async Task RegisterEndpoint([FromBody]EndpointDto endpointDto)
        {
            var endpoint = new Endpoint { Name = endpointDto.Name, CheckInterval = endpointDto.CheckInterval, CheckType = (CheckType)endpointDto.CheckInterval, IsActive = endpointDto.IsActive, IsCritical = endpointDto.IsCritical, Tags = endpointDto.Tags, Url = endpointDto.Url };
            await _store.StoreEndpointAsync(endpoint);
        }

        [HttpPut("/endpoints/{id}")]
        public async Task UpdateEndpoint(int id, [FromBody]EndpointDto endpointDto)
        {
            var endpoint = await _store.GetByEndpointIdAsync(id);
            
            if (!string.IsNullOrEmpty(endpointDto.Name))
                endpoint.Name = endpointDto.Name;

            if (!string.IsNullOrEmpty(endpointDto.Tags))
                endpoint.Tags = endpointDto.Tags;

            if (!string.IsNullOrEmpty(endpointDto.Url))
                endpoint.Url = endpointDto.Url;

            endpoint.CheckInterval = endpointDto.CheckInterval;
            endpoint.CheckType = (CheckType)endpointDto.CheckType;
            endpoint.IsActive = endpointDto.IsActive;
            endpoint.IsCritical = endpointDto.IsCritical;
            endpoint.PublishStats = endpointDto.PublishStats;

            await _store.UpdateEndpointAsync(endpoint);
        }

        [HttpGet("/endpoints")]
        public async Task<IEnumerable<EndpointDto>> GetEndpoints()
        {
            return (await _store.GetAllEndpointsAsync("")).Select(x => new EndpointDto { Name  = x.Name, CheckInterval = x.CheckInterval, CheckType = x.CheckInterval, IsActive = x.IsActive, Tags = x.Tags, IsCritical = x.IsCritical, Url = x.Url, Id = x.Id});
        }

        [HttpDelete("/endpoints/{id}")]
        public async Task DeleteEndpoint(int id)
        {
            await _store.DeleteEndpointById(id);
        }
    }
}
