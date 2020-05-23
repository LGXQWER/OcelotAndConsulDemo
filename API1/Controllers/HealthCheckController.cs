using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        // GET: api/HealthCheck
        [HttpGet]
        [HttpHead]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}