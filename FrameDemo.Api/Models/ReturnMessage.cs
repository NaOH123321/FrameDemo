using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Models
{
    public class ReturnMessage
    {
        public int Code { get; set; } = StatusCodes.Status500InternalServerError;
        public Object Msg { get; set; } = "服务器错误";
        public int ErrorCode { get; set; } = ErrorCodeStatus.ErrorCode999;
    }
}
