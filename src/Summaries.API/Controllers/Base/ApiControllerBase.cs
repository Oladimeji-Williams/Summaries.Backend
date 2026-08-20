using Microsoft.AspNetCore.Mvc;

namespace Summaries.API.Controllers.Base;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
}