using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;
using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FrameDemo.Infrastructure.Repositories
{
    public class SampleRepository : ISampleRepository
    {
        private readonly MyContext _myContext;

        public SampleRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public async Task<PaginatedList<Sample>> GetAllSamplesAsync(SampleParameters sampleParameters)
        {
            var query = _myContext.Samples.AsQueryable();

            if (!string.IsNullOrEmpty(sampleParameters.Title))
            {
                var title = sampleParameters.Title.ToLowerInvariant();
                query = query.Where(x => x.Title.ToLowerInvariant() == title);
            }

            query = query.OrderBy(x => x.Id);

            var count = await _myContext.Samples.CountAsync();
            var data = await query
                .Skip(sampleParameters.PageIndex * sampleParameters.PageSize)
                .Take(sampleParameters.PageSize)
                .ToListAsync();
            return new PaginatedList<Sample>(sampleParameters.PageIndex, sampleParameters.PageSize, count, data);
        }

        public void AddSamples(Sample sample)
        {
            _myContext.Add(sample);
        }

        public async Task<Sample> GetSampleByIdAsync(int id)
        {
            return await _myContext.Samples.FindAsync(id);
        }
    }
}
