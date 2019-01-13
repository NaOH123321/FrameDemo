using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Messages
{
    public class UnauthorizedTokenTimeoutMessage : Message<string>
    {
        public override int Code { get; set; } = StatusCodes.Status401Unauthorized;
        public override string Msg { get; set; } = "Token已过期";
        public override int ErrorCode { get; set; } = ErrorCodeStatus.ErrorCode40010;
    }
}
