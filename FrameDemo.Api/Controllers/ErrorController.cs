using System.Threading.Tasks;
using FrameDemo.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrameDemo.Api.Controllers
{
    [Route("api/error")]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult Errors(string errCode)
        {
            return NotFound(new ReturnMessage()
            {
                Code = StatusCodes.Status404NotFound,
                Msg = "请求的资源不存在",
                ErrorCode = ErrorCodeStatus.ErrorCode40000
            });
        }
    }
}