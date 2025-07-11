using Light.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Abstract BaseApi Controller Class
/// </summary>

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Default success response
    /// </summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    public new virtual IActionResult Ok()
    {
        var result = Result.Success();
        return result.ToActionResult();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public virtual IActionResult Ok<T>(T data)
    {
        var result = data as Light.Contracts.IResult ?? Result<T>.Success(data);
        return result.ToActionResult();
    }
}
