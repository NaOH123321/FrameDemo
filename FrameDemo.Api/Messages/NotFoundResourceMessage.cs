using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Messages
{
    public class NotFoundResourceMessage : Message<string>
    {
        public override int Code { get; set; } = StatusCodes.Status404NotFound;
        public override string Msg { get; set; } = "请求的资源不存在";
        public override int ErrorCode { get; set; } = ErrorCodeStatus.ErrorCode40004;
    }
}
