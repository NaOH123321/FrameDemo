using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;

namespace FrameDemo.Core.Interfaces
{
    public interface IRepository<TEntity, in TQuery> where TEntity : Entity where TQuery : QueryParameters
    {
        Task<PaginatedList<TEntity>> GetAllSamplesAsync(TQuery parameters);
        Task<TEntity> GetSampleByIdAsync(int id);
        void Add(TEntity t);
        void Delete(TEntity t);
        void Update(TEntity t);
    }
}
