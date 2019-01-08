using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;

namespace FrameDemo.Core.Interfaces
{
    public interface ISampleRepository
    {
        Task<PaginatedList<Sample>> GetAllSamplesAsync(SampleParameters sampleParameters);
        Task<Sample> GetSampleByIdAsync(int id);
        void AddSamples(Sample post);
    }
}
