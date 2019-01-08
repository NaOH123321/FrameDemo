using System;
using System.Collections.Generic;
using System.Text;
using FrameDemo.Core.Interfaces;

namespace FrameDemo.Core.Entities
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }
}
