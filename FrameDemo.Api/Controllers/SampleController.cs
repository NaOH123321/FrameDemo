using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;
using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Database;
using FrameDemo.Infrastructure.Repositories;
using AutoMapper;
using FrameDemo.Infrastructure.Resources;
using FrameDemo.Api.Models;
using Microsoft.AspNetCore.Http;
using FrameDemo.Api.Helpers;

namespace FrameDemo.Api.Controllers
{
    [Route("api/sample")]
    public class SampleController : Controller
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SampleController(ISampleRepository sampleRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _sampleRepository = sampleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var samples = await _sampleRepository.GetAllSamplesAsync();

            var sampleResources = _mapper.Map<IEnumerable<Sample>, IEnumerable<SampleResource>>(samples);

            return Ok(sampleResources);
        }

        [HttpGet("{id}", Name = "GetSample")]
        public async Task<IActionResult> Get(int id, string fields = null)
        {
            // if (!_typeHelperService.TypeHasProperties<PostResource>(fields))
            // {
            //     return BadRequest("Fields not exist.");
            // }

            var sample = await _sampleRepository.GetSampleByIdAsync(id);

            if (sample == null)
            {
                return NotFound(new ReturnMessage()
                {
                    Code = StatusCodes.Status404NotFound,
                    Msg = "请求的资源不存在",
                    ErrorCode = ErrorCodeStatus.ErrorCode40000
                });
            }

            return Ok(sample);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SampleAddResource sample)
        {
            // SampleResource s = new SampleResource()
            // {
            //     Title = "title1",
            //     Author = "",
            //     UpdateTime = DateTime.Now
            // };

            if (sample == null)
            {
                return BadRequest(new ReturnMessage()
                {
                    Code = StatusCodes.Status400BadRequest,
                    Msg = "参数错误",
                    ErrorCode = ErrorCodeStatus.ErrorCode10000
                });
            }

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            var sampleResource = _mapper.Map<SampleAddResource, Sample>(sample);

            sampleResource.Author = "admin";
            sampleResource.LastModified = DateTime.Now;

            _sampleRepository.AddSamples(sampleResource);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Save Failed!");
            }

            return CreatedAtRoute("GetSample", new { id = sampleResource.Id }, sampleResource);
        }
    }
}
