using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameDemo.Api.Helpers;
using FrameDemo.Api.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FrameDemo.Api.Extensions
{
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseStatusCodeHandling(this IApplicationBuilder app)
        {
            return app.UseStatusCodePages(context =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                switch (context.HttpContext.Response.StatusCode)
                {
                    case StatusCodes.Status401Unauthorized:
                        var headers = context.HttpContext.Response.Headers;
                        if (headers.ContainsKey("X-Error"))
                        {
                            if (string.Equals(headers["X-Error"], ErrorCodeStatus.ErrorCode40009.ToString()))
                            {
                                context.HttpContext.Response.Headers.Remove("X-Error");
                                return context.HttpContext.Response.WriteAsync(new UnauthorizedNotValidTokenMessage().ToJson());
                            }
                            if (string.Equals(headers["X-Error"], ErrorCodeStatus.ErrorCode40010.ToString()))
                            {
                                context.HttpContext.Response.Headers.Remove("X-Error");
                                return context.HttpContext.Response.WriteAsync(new UnauthorizedTokenTimeoutMessage().ToJson());
                            }
                        }
                        return context.HttpContext.Response.WriteAsync(new UnauthorizedMessage().ToJson());
                    case StatusCodes.Status404NotFound:
                        return context.HttpContext.Response.WriteAsync(new NotFoundMessage().ToJson());
                    case StatusCodes.Status406NotAcceptable:
                        return context.HttpContext.Response.WriteAsync(new NotAcceptableMessage().ToJson());
                    case StatusCodes.Status415UnsupportedMediaType:
                        return context.HttpContext.Response.WriteAsync(new UnsupportedMediaTypeMessage().ToJson());
                    default:
                        throw new Exception("Error, status code: " + context.HttpContext.Response.StatusCode);
                }
            });
        }
    }
}
