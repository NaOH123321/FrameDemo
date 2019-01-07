using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;
using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Database;
using FrameDemo.Infrastructure.Repositories;

namespace FrameDemo.Api.Controllers
{
    [Route("api/sample")]
    public class SampleController:Controller
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SampleController(ISampleRepository sampleRepository, IUnitOfWork unitOfWork)
        {
            _sampleRepository = sampleRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result =  await _sampleRepository.GetAllSamplesAsync();

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Sample s = new Sample()
            {
                Title = "title1",
                Author = "dsa",
                LastModified = DateTime.Now
            };

            throw new Exception("sddddsssss");
            //return BadRequest("Can't finds fields for sorting.");

            return NotFound(s);

            _sampleRepository.AddSamples(s);

            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}
