using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using EpMon.Data.Entities;
using EpMon.Web.Api;
using EpMon.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace EpMon.Web.Core.Controllers
{
    public class EndpointsController : BaseApiController
    {
        private readonly EpMonAsyncRepository _asyncRepo;

        public EndpointsController(EpMonAsyncRepository asyncRepo)
        {
            _asyncRepo = asyncRepo;
        }

        [HttpPost("/endpoints")]
        public async Task RegisterEndpoint([FromBody]EndpointDto endpointDto)
        {
            var endpoint = new Endpoint { Name = endpointDto.Name, CheckInterval = endpointDto.CheckInterval, CheckType = (CheckType)endpointDto.CheckInterval, IsActive = endpointDto.IsActive, IsCritical = endpointDto.IsCritical, Tags = endpointDto.Tags, Url = endpointDto.Url };
            await _asyncRepo.AddEndpoint(endpoint);
        }

        [HttpGet("/endpoints")]
        public async Task<IEnumerable<EndpointDto>> GetEndpoints()
        {
            return (await _asyncRepo.GetEndpointsAsync("")).Select(x => new EndpointDto { Name  = x.Name, CheckInterval = x.CheckInterval, CheckType = x.CheckInterval, IsActive = x.IsActive, Tags = x.Tags, IsCritical = x.IsCritical, Url = x.Url, Id = x.Id});
        }

        [HttpDelete("/endpoints/{endpointId}")]
        public async Task DeleteEndpoint(int endpointId)
        {
            await _asyncRepo.DeleteEndpointById(endpointId);
        }
    }
}
