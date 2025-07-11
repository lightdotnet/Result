using Light.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult(this Light.Contracts.IResult result)
    {
        return new ObjectResult(result)
        {
            StatusCode = (int)result.MapHttpStatusCode()
        };
    }
}
