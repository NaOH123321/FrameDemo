using System;
using System.Collections.Generic;
using System.Text;
using FrameDemo.Core.Entities;
using FrameDemo.Infrastructure.Services;

namespace FrameDemo.Infrastructure.Resources
{
    public class SamplePropertyMapping : PropertyMapping<SampleResource, Sample>
    {
        public SamplePropertyMapping() : base(
            new Dictionary<string, List<MappedProperty>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(SampleResource.Title)] = new List<MappedProperty>()
                {
                    new MappedProperty() {Name = nameof(Sample.Title)}
                },
                [nameof(SampleResource.Author)] = new List<MappedProperty>()
                {
                    new MappedProperty() {Name = nameof(Sample.Author)}
                },
                [nameof(SampleResource.Body)] = new List<MappedProperty>()
                {
                    new MappedProperty() {Name = nameof(Sample.Body)}
                },
                [nameof(SampleResource.UpdateTime)] = new List<MappedProperty>()
                {
                    new MappedProperty() {Name = nameof(Sample.LastModified)}
                }
            }
        )
        {
        }
    }
}

