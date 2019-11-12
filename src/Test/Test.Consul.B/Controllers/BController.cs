using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FastNet.Framework.Consul;

namespace Test.Consul.B.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BController : ControllerBase
    {
        private readonly ILogger<BController> _logger;
        private ConsulHttpClient _consulHttpClient;

        public BController(ILogger<BController> logger, ConsulHttpClient consulHttpClient)
        {
            _logger = logger;
            _consulHttpClient = consulHttpClient;
        }

        /// <summary>
        /// Call_A
        /// </summary>
        /// <returns></returns>
        [HttpGet("call_a")]
        public string Call_A()
        {
            string result = _consulHttpClient.DoGet(serviceName: "consul-a", requestUrl: "api/A");
            return $"Call_A result -> {result}";
        }

        [HttpGet]
        public string Get()
        {
            return "this is consul-b service";
        }
    }
}
