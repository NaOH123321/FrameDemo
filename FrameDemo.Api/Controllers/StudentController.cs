using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FrameDemo.Api.Messages;
using FrameDemo.Core.Entities;
using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Extensions;
using FrameDemo.Infrastructure.Resources;
using FrameDemo.Infrastructure.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FrameDemo.Api.Controllers
{
    [Route("api/student")]
    public class StudentController : SampleController
    {
        public StudentController(
            IRepository<Sample, SampleParameters> sampleRepository, 
            IUnitOfWork unitOfWork,
            IMapper mapper, 
            IUrlHelper urlHelper, 
            IPropertyMappingContainer propertyMappingContainer,
            ITypeHelperService typeHelperService) : base(sampleRepository, unitOfWork, mapper, urlHelper,
            propertyMappingContainer, typeHelperService)
        {
        }

        [HttpGet(Name = "GetStudents")]
        public override async Task<IActionResult> Get(SampleParameters sampleParameters)
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

            CreateGetHeaders(sampleParameters, sampleList);

            return Ok(shapedSampleResources);
        }

        private void CreateGetHeaders(SampleParameters sampleParameters, PaginatedList<Sample> sampleList)
        {
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
                    return _urlHelper.Link("GetStudents", previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetStudents", nextParameters);
                default:
                    var currentParameters = new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetStudents", currentParameters);
            }
        }

        [HttpGet("{id}", Name = "GetStudent")]
        public override async Task<IActionResult> Get(int id, string fields = null)
        {
            return Ok();
        }

        [HttpPost(Name = "CreateStudent")]
        public override async Task<IActionResult> Post([FromBody] SampleAddResource sampleAddResource)
        {
            return Ok();
        }

        [HttpPut("{id}", Name = "UpdateStudent")]
        public override async Task<IActionResult> Put(int id, [FromBody] SampleUpdateResource sampleUpdateResource)
        {
            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateStudent")]
        public override async Task<IActionResult> Patch(int id,
            [FromBody] JsonPatchDocument<SampleUpdateResource> patchDoc)
        {
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteStudent")]
        public override async Task<IActionResult> Delete(int id)
        {
            return Ok();
        }
    }
}
