using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameDemo.Api.Helpers;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Messages
{
    public class InternalServerErrorMessage : Message<string>
    {
        public override int Code { get; set; }= StatusCodes.Status500InternalServerError;
        public override string Msg { get; set; } = "服务器错误";
        public override int ErrorCode { get; set; } = ErrorCodeStatus.ErrorCode999;
    }
}
