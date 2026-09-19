using Microsoft.AspNetCore.Mvc;

namespace Summaries.Shared.Infrastructure.Api;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
}