using System.Diagnostics;
using EpMon.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace EpMon.Web.Api
{
    public class MetaController : BaseApiController
    {
        [HttpGet("/api/info")]
        public ActionResult<MetaInfoDto> Info()
        {
            var assembly = typeof(Startup).Assembly;

            var creationDate = System.IO.File.GetCreationTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            var appName = assembly.GetName().Name.Split(',')[0];

            return Ok(new MetaInfoDto{ VersionInfo = version, BuildDate = creationDate, AppName = appName });
        }
    }
}