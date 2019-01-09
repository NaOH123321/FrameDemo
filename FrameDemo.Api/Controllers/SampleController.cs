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
using Microsoft.AspNetCore.Http;
using FrameDemo.Api.Helpers;
using FrameDemo.Api.Messages;
using FrameDemo.Infrastructure.Extensions;
using FrameDemo.Infrastructure.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FrameDemo.Api.Controllers
{
    [Route("api/sample")]
    public class SampleController : Controller
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        private readonly ITypeHelperService _typeHelperService;

        public SampleController(ISampleRepository sampleRepository, 
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IUrlHelper urlHelper,
            IPropertyMappingContainer propertyMappingContainer,
            ITypeHelperService typeHelperService)
        {
            _sampleRepository = sampleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _propertyMappingContainer = propertyMappingContainer;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetSamples")]
        public async Task<IActionResult> Get(SampleParameters sampleParameters)
        {
            if (!_propertyMappingContainer.ValidateMappingExistsFor<SampleResource, Sample>(sampleParameters.OrderBy))
            {
                return BadRequest(new BadRequestForSortingMessage());
            }

            if (!_typeHelperService.TypeHasProperties<SampleResource>(sampleParameters.Fields))
            {
                return BadRequest(new BadRequestFieldsMessage());
            }

            var sampleList = await _sampleRepository.GetAllSamplesAsync(sampleParameters);

            var sampleResources = _mapper.Map<IEnumerable<Sample>, IEnumerable<SampleResource>>(sampleList);

            var shapedSampleResources = sampleResources.ToDynamicIEnumerable(sampleParameters.Fields);

            var previousPageLink = sampleList.HasPrevious ?
                CreatePostUri(sampleParameters, PaginationResourceUriType.PreviousPage) : null;

            var nextPageLink = sampleList.HasNext ?
                CreatePostUri(sampleParameters, PaginationResourceUriType.NextPage) : null;

            var meta = new
            {
                sampleList.PageIndex,
                sampleList.PageSize,
                sampleList.PageCount,
                sampleList.TotalItemsCount,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            return Ok(shapedSampleResources);
        }

        [HttpGet("{id}", Name = "GetSample")]
        public async Task<IActionResult> Get(int id, string fields = null)
        {
            if (!_typeHelperService.TypeHasProperties<SampleResource>(fields))
            {
                return BadRequest(new BadRequestFieldsMessage());
            }

            var sample = await _sampleRepository.GetSampleByIdAsync(id);

            if (sample == null)
            {
                return NotFound(new NotFoundResourceMessage());
            }

            var sampleResource = _mapper.Map<Sample, SampleResource>(sample);

            var shapedSampleResource = sampleResource.ToDynamic(fields);

            return Ok(shapedSampleResource);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SampleAddResource sample)
        {
            if (sample == null)
            {
                return BadRequest(new BadRequestMessage());
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

        private string CreatePostUri(SampleParameters parameters, PaginationResourceUriType uriType)
        {
            switch (uriType)
            {
                case PaginationResourceUriType.PreviousPage:
                    var previousParameters = new
                    {
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetSamples", previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetSamples", nextParameters);
                default:
                    var currentParameters = new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetSamples", currentParameters);
            }
        }
    }
}
