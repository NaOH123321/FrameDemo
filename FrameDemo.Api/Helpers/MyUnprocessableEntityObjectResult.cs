using System;
using FrameDemo.Api.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FrameDemo.Api.Helpers
{
    public class MyUnprocessableEntityObjectResult: UnprocessableEntityObjectResult
    {
        public MyUnprocessableEntityObjectResult(ModelStateDictionary modelState) : base(modelState)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 422;
            Value = new UnprocessableEntityMessage(){
                Msg = new ResourceValidationResult(modelState),
            };
        }
    }
}
