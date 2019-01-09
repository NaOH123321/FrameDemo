using System.Collections.Generic;

namespace FrameDemo.Infrastructure.Services
{
    public class MappedProperty
    {
        public string Name { get; set; }

        /// <summary>
        /// 排序是否是反转的
        /// </summary>
        public bool Revert { get; set; } = false;
    }
}