using System;
using System.Collections.Generic;
using System.Text;

namespace FrameDemo.Infrastructure.Resources.Hateoas
{
    public abstract class LinkResourceBase
    {
        public List<LinkResource> Links { get; set; } = new List<LinkResource>();
    }
}
