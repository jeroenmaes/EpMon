using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using Microsoft.AspNetCore.Mvc;

namespace EpMon.Web.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly EpMonAsyncRepository _asyncRepo;
        
        public ConfigurationController(EpMonAsyncRepository asyncRepo)
        {
            _asyncRepo = asyncRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return (await _asyncRepo.GetEndpointsAsync("")).Select(x => x.Url);
        }
    }
}
