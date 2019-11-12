using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FastNet.Framework.Consul;

namespace Test.Consul.A.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AController : ControllerBase
    {        

        private readonly ILogger<AController> _logger;
        private ConsulHttpClient _consulHttpClient;


        public AController(ILogger<AController> logger, ConsulHttpClient consulHttpClient)
        {
            _logger = logger;
            _consulHttpClient = consulHttpClient;
        }

        /// <summary>
        /// Call_B
        /// </summary>
        /// <returns></returns>
        [HttpGet("call_b")]
        public string Call_B()
        {
            string result = _consulHttpClient.DoGet(serviceName: "consul-b", requestUrl: "api/B");
            return $"Call_B result -> {result}";
        }

        [HttpGet]
        public string Get()
        {
            return "this is consul-a service";
        }
    }
}
