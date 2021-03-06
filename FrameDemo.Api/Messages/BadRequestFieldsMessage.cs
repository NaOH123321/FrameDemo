﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrameDemo.Api.Messages
{
    public class BadRequestFieldsMessage : Message<string>
    {
        public override int Code { get; set; } = StatusCodes.Status400BadRequest;
        public override string Msg { get; set; } = "不能找到相应的字段塑形";
        public override int ErrorCode { get; set; } = ErrorCodeStatus.ErrorCode40008;
    }
}
