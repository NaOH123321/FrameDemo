﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;

namespace FrameDemo.Core.Interfaces
{
    public interface ISampleRepository
    {
        Task<IEnumerable<Sample>> GetAllSamplesAsync();
        Task<Sample> GetSampleByIdAsync(int id);
        void AddSamples(Sample post);
    }
}
