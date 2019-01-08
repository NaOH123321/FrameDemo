using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Messages
{
    public class NotAcceptableMessage : Message<String>
    {
        public override int Code { get; set; } = StatusCodes.Status406NotAcceptable;
        public override string Msg { get; set; } = "不支持的Acceptable";
        public override int ErrorCode { get; set; } = ErrorCodeStatus.ErrorCode40005;
    }
}
