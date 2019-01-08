using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrameDemo.Api.Messages
{
    public interface IMessage<T>
    {
        int Code { get; set; }
        T Msg { get; set; }
        int ErrorCode { get; set; }
        string ToJson();
    }
}
