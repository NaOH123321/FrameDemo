using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Messages
{
    public class UnauthorizedNotValidTokenMessage : Message<string>
    {
        public override int Code { get; set; } = StatusCodes.Status401Unauthorized;
        public override string Msg { get; set; } = "无效Token";
        public override int ErrorCode { get; set; } = ErrorCodeStatus.ErrorCode40009;
    }
}
