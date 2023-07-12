using Microsoft.AspNetCore.Mvc;

namespace EpMon.Web.Api
{
    [ApiController]
    [ApiKey]
    public abstract class BaseApiController : Controller
    {
    }
}
