using AutoMapper;
using FrameDemo.Core.Entities;
using FrameDemo.Infrastructure.Resources;

namespace FrameDemo.Api.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Sample, SampleResource>()
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.LastModified));
            //CreateMap<SampleResource, Sample>();
            CreateMap<SampleAddResource, Sample>();
            CreateMap<SampleUpdateResource, Sample>();


            CreateMap<Song, SongResource>()
                .ForMember(dest => dest.DurationTime, opt => opt.MapFrom(src => src.Interval));
        }
    }
}