using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Messages
{
    public class OkMessage : Message<string>
    {
        public override int Code { get; set; } = StatusCodes.Status200OK;
        public override string Msg { get; set; } = "查询成功";
        public override int ErrorCode { get; set; } = 0;

        public OkMessage(object value)
        {
            Data = value;
        }
    }
}
