using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using EpMon.Web.Api;
using Microsoft.AspNetCore.Mvc;

namespace EpMon.Web.Core.Controllers
{    
    public class ConfigurationController :  BaseApiController
    {
        private readonly EpMonAsyncRepository _asyncRepo;
        
        public ConfigurationController(EpMonAsyncRepository asyncRepo)
        {
            _asyncRepo = asyncRepo;
        }

        [HttpGet("/configuration")]
        public async Task<IEnumerable<string>> Get()
        {
            return (await _asyncRepo.GetEndpointsAsync("")).Select(x => x.Url);
        }
    }
}
