using System;
using System.Collections.Generic;
using System.Text;
using FrameDemo.Core.Interfaces;

namespace FrameDemo.Core.Entities
{
    public abstract class Entity : IEntity
    {
        public string Id { get; set; }
    }
}
