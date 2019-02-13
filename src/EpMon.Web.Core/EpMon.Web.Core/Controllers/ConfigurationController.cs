using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EpMon.Web.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly EpMonRepository _repo;
        public ConfigurationController()
        {
            _repo = new EpMonRepository();
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _repo.GetEndpoints().Select(x => x.Url);
        }

        [HttpPost]
        public void Post(string url, int checkInterval, string tags)
        {
            int id = _repo.AddEndpoint(url, checkInterval, tags);
        }
    }
}
