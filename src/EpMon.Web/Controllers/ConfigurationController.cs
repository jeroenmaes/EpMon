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
        private readonly EpMonRepository _repo;
        public ConfigurationController()
        {
            _asyncRepo = new EpMonAsyncRepository();
            _repo = new EpMonRepository();
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return (await _asyncRepo.GetEndpointsAsync("")).Select(x => x.Url);
        }

        //[HttpPost]
        //public int Post(string url, int checkInterval, string tags)
        //{
        //    return _repo.AddEndpoint(url, checkInterval, tags);
        //}
    }
}
