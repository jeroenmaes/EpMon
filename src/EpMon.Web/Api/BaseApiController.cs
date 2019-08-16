using Microsoft.AspNetCore.Mvc;

namespace EpMon.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Controller
    {
    }
}
