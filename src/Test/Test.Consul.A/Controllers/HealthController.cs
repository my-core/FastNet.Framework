using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Test.Consul.A.Controllers
{
    /// <summary>
    /// Consul会通过call这个API来确认Service的健康状态。 
    /// </summary>
    [Produces("application/json")]
    [Route("api/Health")]
    public class HealthController : Controller
    {
        private ILogger<HealthController> _logger;
        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("call api/ealth");
             return Ok("ok");
        }
    }
}