using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;

namespace FrameDemo.Core.Interfaces
{
    public interface IRepository<TEntity, in TParameters> where TEntity : Entity where TParameters : QueryParameters
    {
        Task<PaginatedList<TEntity>> GetAllSamplesAsync(TParameters parameters);
        Task<TEntity> GetSampleByIdAsync(int id);
        void Add(TEntity t);
        void Delete(TEntity t);
        void Update(TEntity t);
    }
}
