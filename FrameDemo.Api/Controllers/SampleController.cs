using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.IdentityModel.Tokens.Saml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FrameDemo.Api.Controllers
{
    [Authorize]
    [Route("api/sample")]
    public class SampleController : BasicController<SampleResource, Sample>
    {
        private readonly IRepository<Sample, SampleParameters> _sampleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        //private readonly IUrlHelper _urlHelper;
        //private readonly IPropertyMappingContainer _propertyMappingContainer;
        //private readonly ITypeHelperService _typeHelperService;

        public SampleController(IRepository<Sample, SampleParameters> sampleRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUrlHelper urlHelper,
            IPropertyMappingContainer propertyMappingContainer,
            ITypeHelperService typeHelperService) : base(urlHelper, propertyMappingContainer, typeHelperService)
        {
            _sampleRepository = sampleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_urlHelper = urlHelper;
            //_propertyMappingContainer = propertyMappingContainer;
            //_typeHelperService = typeHelperService;

            //base.EntityType = typeof(Sample);
            //base.ResourceType = typeof(SampleResource);
            //base.UpdateOrAddResourceType = typeof(SampleAddOrUpdateResource);
            //base.ParametersType = typeof(SampleParameters);
        }

        [HttpGet(Name = "GetSamples")]
        public async Task<IActionResult> Get(SampleParameters sampleParameters)
        {
            //if (!_propertyMappingContainer.ValidateMappingExistsFor<SampleResource, Sample>(sampleParameters.OrderBy))
            //{
            //    return BadRequest(new BadRequestForSortingMessage());
            //}

            //if (!_typeHelperService.TypeHasProperties<SampleResource>(sampleParameters.Fields))
            //{
            //    return BadRequest(new BadRequestFieldsMessage());
            //}

            //ObjectResult result= GetValidate<SampleResource, Sample, SampleParameters>(sampleParameters);
            //if (result != null) return result;

            ValidateMapping(sampleParameters.OrderBy);
            ValidateFields(sampleParameters.Fields);
            if (Results.Count != 0) return Results.First();

            var sampleList = await _sampleRepository.GetAllAsync(sampleParameters);

            var sampleResources = _mapper.Map<IEnumerable<Sample>, IEnumerable<SampleResource>>(sampleList);

            var shapedSampleResources = sampleResources.ToDynamicIEnumerable(sampleParameters.Fields);

            CreateHeader(sampleParameters, sampleList, "GetSamples");

            return Ok(shapedSampleResources);
        }

        [HttpGet("{id}", Name = "GetSample")]
        public async Task<IActionResult> Get(int id, string fields = null)
        {
            //if (!_typeHelperService.TypeHasProperties<SampleResource>(fields))
            //{
            //    return BadRequest(new BadRequestFieldsMessage());
            //}
            ValidateFields(fields);
            if (Results.Count != 0) return Results.First();

            var sample = await _sampleRepository.GetByIdAsync(id);

            ValidateNotFound(sample);
            if (Results.Count != 0) return Results.First();

            //if (sample == null)
            //{
            //    return NotFound(new NotFoundResourceMessage());
            //}

            var sampleResource = _mapper.Map<Sample, SampleResource>(sample);

            var shapedSampleResource = sampleResource.ToDynamic(fields);

            return Ok(shapedSampleResource);
        }

        [HttpPost(Name = "CreateSample")]
        public async Task<IActionResult> Post([FromBody] SampleAddResource sampleAddResource)
        {
            //if (sampleAddResource == null)
            //{
            //    return BadRequest(new BadRequestMessage());
            //}

            //if (!ModelState.IsValid)
            //{
            //    return new MyUnprocessableEntityObjectResult(ModelState);
            //}

            ValidateNotNull(sampleAddResource);
            ValidateParameters();
            if (Results.Count != 0) return Results.First();

            var sampleResource = _mapper.Map<SampleAddResource, Sample>(sampleAddResource);

            sampleResource.Author = "admin";
            sampleResource.LastModified = DateTime.Now;

            _sampleRepository.Add(sampleResource);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Save Failed!");
            }

            return CreatedAtRoute("GetSample", new { id = sampleResource.Id }, sampleResource);
        }

        [HttpPut("{id}", Name = "UpdateSample")]
        public async Task<IActionResult> Put(int id, [FromBody] SampleUpdateResource sampleUpdateResource)
        {
            //if (sampleUpdateResource == null)
            //{
            //    return BadRequest(new BadRequestMessage());
            //}

            //if (!ModelState.IsValid)
            //{
            //    return new MyUnprocessableEntityObjectResult(ModelState);
            //}
            ValidateNotNull(sampleUpdateResource);
            ValidateParameters();
            if (Results.Count != 0) return Results.First();

            var sample = await _sampleRepository.GetByIdAsync(id);

            ValidateNotFound(sample);
            if (Results.Count != 0) return Results.First();
            //if (sample == null)
            //{
            //    return NotFound(new NotFoundResourceMessage());
            //}

            sample.LastModified = DateTime.Now;
            _mapper.Map(sampleUpdateResource, sample);
            _sampleRepository.Update(sample);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Updating post {id} failed when saving.");
            }

            return Ok(_mapper.Map<SampleResource>(sample));
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateSample")]
        public async Task<IActionResult> Patch(int id,
            [FromBody] JsonPatchDocument<SampleUpdateResource> patchDoc)
        {
            //if (patchDoc == null)
            //{
            //    return BadRequest(new BadRequestMessage());
            //}

            ValidateNotNull(patchDoc);
            if (Results.Count != 0) return Results.First();

            var sample = await _sampleRepository.GetByIdAsync(id);

            ValidateNotFound(sample);
            if (Results.Count != 0) return Results.First();

            //if (sample == null)
            //{
            //    return NotFound(new NotFoundResourceMessage());
            //}

            var sampleToPatch = _mapper.Map<SampleUpdateResource>(sample);

            patchDoc.ApplyTo(sampleToPatch, ModelState);

            TryValidateModel(sampleToPatch);

            ValidateParameters();
            if (Results.Count != 0) return Results.First();

            //if (!ModelState.IsValid)
            //{
            //    return new MyUnprocessableEntityObjectResult(ModelState);
            //}

            _mapper.Map(sampleToPatch, sample);
            sample.LastModified = DateTime.Now;
            _sampleRepository.Update(sample);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Patching city {id} failed when saving.");
            }

            return Ok(_mapper.Map<SampleResource>(sample));
        }

        [HttpDelete("{id}", Name = "DeleteSample")]
        public async Task<IActionResult> Delete(int id)
        {
            var sample = await _sampleRepository.GetByIdAsync(id);

            ValidateNotFound(sample);
            if (Results.Count != 0) return Results.First();
            //if (sample == null)
            //{
            //    return NotFound(new NotFoundResourceMessage());
            //}

            _sampleRepository.Delete(sample);
     
            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Deleting sample {id} failed when saving.");
            }

            return NoContent();
        }
    }
}
