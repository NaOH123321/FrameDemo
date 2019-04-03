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
    public class SampleRepository : IRepository<Sample, SampleParameters>
    {
        private readonly MyContext _myContext;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public SampleRepository(MyContext myContext, IPropertyMappingContainer propertyMappingContainer)
        {
            _myContext = myContext;
            _propertyMappingContainer = propertyMappingContainer;
        }

        public async Task<PaginatedList<Sample>> GetAllAsync(SampleParameters sampleParameters)
        {
            var query = _myContext.Samples.AsQueryable();

            if (!string.IsNullOrEmpty(sampleParameters.Title))
            {
                var title = sampleParameters.Title.ToLowerInvariant();
                query = query.Where(x =>!string.IsNullOrEmpty(x.Title) && x.Title.ToLowerInvariant().Contains(title));
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

        public async Task<Sample> GetByIdAsync(int id)
        {
            List<List<List<string>>> ppp = new List<List<List<string>>>();
            var pp11 = new List<List<string>>();
            var tt11 = new List<List<string>>();
            var mm11 = new List<List<string>>();

            var pp22 = new List<string>();
            var tt22 = new List<string>();
            var mm22 = new List<string>();

            pp22.Add("sas");
            pp22.Add("pppp");
            tt22.Add("gggg");

            pp11.Add(pp22);
            pp11.Add(mm22);
            tt11.Add(tt22);

            ppp.Add(pp11);
            ppp.Add(tt11);
            ppp.Add(mm11);

            var query = ppp.AsQueryable();

            //await query.ForEachAsync(x => x.RemoveAll(y => y.Count(z => z == "sas") == 0));

            //await query.ForEachAsync(x =>
            //    {
            //        //var p = new List<string>();

            //        foreach (var list in x)
            //        {
            //            list.RemoveAll(z => z != "sas");
            //        }

            //       //x = x.AsQueryable().Where(y => y.Count(z => z == "sas") > 0).ToList();

            //    }
            // );
            //await query.ForEachAsync(x => { x.Count.ToString(); }
            //);

            //query = query.Where(x => x.Count(y => y.Count(z => z == "sas") > 0) > 0);

            //var sd = query.Select((lists, index) => new 
            //{ 
            //    x = lists.Where(x => x.Count(p => p == "sas") > 0).ToList()
            //}).ToList();

            //var temp333 = query.ToList();

            var ssasdasdsadssd =  query.SelectMany(x => x.Where(t => t.Contains("sas")), (x, y) => new
            {
                x,
                y
            }).ToList();


            var temp333555 = query.Select(x => new List<List<string>>(x.Where(p => p.Count(t => t == "sas") > 0).ToList())
            ).Where(x=>x.Count>0).ToList();
            foreach (var lists in query.ToList())
            {
               lists.RemoveAll(x => x.Count(p => p == "sas") == 0);
            }

            var temp = query.SingleOrDefault();
            //var temp = ppp;

            //query = query.Where(x => x.RemoveAll(y => y.Count(z => z == "sas") == 0) > 0);

            //query = query.Where(x => x.Count(y => y.Count(z => z == "sas") > 0) > 0);

           


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
