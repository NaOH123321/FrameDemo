using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Sample>> GetAllSamplesAsync()
        {
            return await _myContext.Samples.ToListAsync();
        }

        public void AddSamples(Sample sample)
        {
            _myContext.Add(sample);
        }
    }
}
