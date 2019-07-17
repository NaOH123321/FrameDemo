using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FrameDemo.Api.Messages;
using FrameDemo.Core.Entities;
using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Repositories;
using FrameDemo.Infrastructure.Resources;
using FrameDemo.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrameDemo.Api.Controllers
{
    [Route("api/songs")]
    public class TopSongController : ControllerBase
    {
        private readonly SongRepository _songRepository;
        private readonly IMapper _mapper;

        public TopSongController(SongRepository songRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUrlHelper urlHelper,
            IPropertyMappingContainer propertyMappingContainer,
            ITypeHelperService typeHelperService)
        {
            _songRepository = songRepository;
            _mapper = mapper;
        }


        [HttpGet(Name = "GetSongs")]
        public async Task<IActionResult> Get(SongParameters songParameters)
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

            //ValidateMapping(sampleParameters.OrderBy);
            //ValidateFields(sampleParameters.Fields);
            //if (Results.Count != 0) return Results.First();

            var list = await _songRepository.GetAllAsync(songParameters);

            var resources = _mapper.Map<IEnumerable<Song>, IEnumerable<SongResource>>(list);

            //var shapedSampleResources = sampleResources.ToDynamicIEnumerable(sampleParameters.Fields);

            //CreateHeader(sampleParameters, sampleList, "GetSamples");

            return Ok(new OkMessage(resources));
        }

        [HttpGet("search", Name = "GetSongsBySearch")]
        public async Task<IActionResult> Get(SongParameters songParameters, string keywords)
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

            //ValidateMapping(sampleParameters.OrderBy);
            //ValidateFields(sampleParameters.Fields);
            //if (Results.Count != 0) return Results.First();

            var list = await _songRepository.GetSongsBySearch(songParameters, keywords);

            //var sampleResources = _mapper.Map<IEnumerable<Sample>, IEnumerable<SampleResource>>(sampleList);

            //var shapedSampleResources = sampleResources.ToDynamicIEnumerable(sampleParameters.Fields);

            //CreateHeader(sampleParameters, sampleList, "GetSamples");

            return Ok(list);
        }

        [HttpGet("{mid}", Name = "GetSongPlayInfo")]
        public async Task<IActionResult> Get(string mid)
        {
            //if (!_typeHelperService.TypeHasProperties<SampleResource>(fields))
            //{
            //    return BadRequest(new BadRequestFieldsMessage());
            //}
            //ValidateFields(fields);
            //if (Results.Count != 0) return Results.First();

            var playInfo = await _songRepository.GetSongPlayInfo(mid);

            //ValidateNotFound(sample);
            //if (Results.Count != 0) return Results.First();

            //if (sample == null)
            //{
            //    return NotFound(new NotFoundResourceMessage());
            //}

            //var sampleResource = _mapper.Map<Sample, SampleResource>(sample);

            //var shapedSampleResource = sampleResource.ToDynamic(fields);

            return Ok(playInfo);
        }
    }
}
