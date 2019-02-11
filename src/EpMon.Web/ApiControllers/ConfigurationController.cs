using System.Linq;
using System.Web.Http;
using EpMon.Data;
using EpMon.Data.Entities;

namespace EpMon.Web.ApiControllers
{
    [RoutePrefix("api")]
    [Authorize]
    public class ConfigurationController : ApiController
    {
        private readonly EpMonRepository _repo;
        public ConfigurationController()
        {
            _repo = new EpMonRepository();
        }
        
        [HttpGet]
        [Route("GetEndpoints")]
        public IHttpActionResult GetEndpoints()
        {
            return Ok(_repo.GetEndpoints().Select(x => x.Url));
        }

        [HttpPost]
        [Route("AddEndpoint")]
        public IHttpActionResult AddEndpoint(string url, int checkInterval, string tags)
        {
            int id = _repo.AddEndpoint(url, checkInterval, tags);
            return Ok(id);
        }
    }
}
