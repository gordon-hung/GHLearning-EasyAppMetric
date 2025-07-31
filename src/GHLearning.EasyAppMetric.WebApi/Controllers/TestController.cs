using Microsoft.AspNetCore.Mvc;

namespace GHLearning.EasyAppMetric.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    // 模擬返回 200 OK
    [HttpGet("ok")]
    public IActionResult GetOk()
         => Ok(new { Message = "Request was successful!" });

    // 模擬返回 404 Not Found
    [HttpGet("notfound")]
    public IActionResult GetNotFound()
        => NotFound(new { Message = "Resource not found!" });

    // 模擬返回 400 Bad Request
    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
        => BadRequest(new { Message = "The request is invalid!" });

    // 模擬返回 500 Internal Server Error
    [HttpGet("error")]
    public IActionResult GetInternalServerError()
        => StatusCode(500, new { Message = "An internal server error occurred!" });

    // 模擬返回未實現的功能，拋出 NotImplementedException
    [HttpGet("exception")]
    public IActionResult GetNotImplementedException()
        => throw new NotImplementedException();

    // 增加隨機回應方法
    [HttpGet("randomresponse")]
    public IActionResult GetRandomResponse(
        [FromServices] Random random)
        => random.Next(1, 5) switch
        {
            1 => Ok(new { Timestamp = DateTimeOffset.UtcNow, Message = "Random request was successful!" }),
            2 => BadRequest(new { Timestamp = DateTimeOffset.UtcNow, Message = "Random bad request!" }),
            3 => NotFound(new { Timestamp = DateTimeOffset.UtcNow, Message = "Random resource not found!" }),
            4 => StatusCode(500, new { Timestamp = DateTimeOffset.UtcNow, Message = "Random internal server error!" }),
            _ => StatusCode(500, new { Timestamp = DateTimeOffset.UtcNow, Message = "Unexpected error occurred." })
        };
}