using System.Collections.Generic;

namespace FrameDemo.Infrastructure.Services
{
    public interface IPropertyMapping
    {
        Dictionary<string, List<MappedProperty>> MappingDictionary { get; }
    }
}