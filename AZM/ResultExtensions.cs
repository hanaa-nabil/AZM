using AZM.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AZM.Api
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
            => result.IsSuccess
                ? new OkObjectResult(result.Data)
                : new ObjectResult(new { message = result.Error }) { StatusCode = result.StatusCode };
    }
}
