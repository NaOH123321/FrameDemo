using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrameDemo.Api.Controllers
{
    [Route("api/sample")]
    public class SampleController:Controller
    {
        [HttpGet]
        public IActionResult Get()
        {

            return Ok("hello");
        }
    }
}
