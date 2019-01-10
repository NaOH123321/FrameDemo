using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;
using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Database;
using FrameDemo.Infrastructure.Extensions;
using FrameDemo.Infrastructure.Resources;
using FrameDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FrameDemo.Infrastructure.Repositories
{
    public class SampleRepository : ISampleRepository
    {
        private readonly MyContext _myContext;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public SampleRepository(MyContext myContext, IPropertyMappingContainer propertyMappingContainer)
        {
            _myContext = myContext;
            _propertyMappingContainer = propertyMappingContainer;
        }

        public async Task<PaginatedList<Sample>> GetAllSamplesAsync(SampleParameters sampleParameters)
        {
            var query = _myContext.Samples.AsQueryable();

            if (!string.IsNullOrEmpty(sampleParameters.Title))
            {
                var title = sampleParameters.Title.ToLowerInvariant();
                query = query.Where(x => x.Title.ToLowerInvariant().Contains(title));
            }

            query = query.ApplySort(sampleParameters.OrderBy,
                _propertyMappingContainer.Resolve<SampleResource, Sample>());

            var count = await _myContext.Samples.CountAsync();
            var data = await query
                .Skip(sampleParameters.PageIndex * sampleParameters.PageSize)
                .Take(sampleParameters.PageSize)
                .ToListAsync();
            return new PaginatedList<Sample>(sampleParameters.PageIndex, sampleParameters.PageSize, count, data);
        }

        public async Task<Sample> GetSampleByIdAsync(int id)
        {
            return await _myContext.Samples.FindAsync(id);
        }

        public void Add(Sample sample)
        {
            _myContext.Add(sample);
        }

        public void Delete(Sample sample)
        {
            _myContext.Samples.Remove(sample);
        }

        public void Update(Sample sample)
        {
            _myContext.Entry(sample).State = EntityState.Modified;
        }
    }
}
