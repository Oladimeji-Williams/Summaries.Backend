using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Shared.Errors;

public static class UserErrors
{
    public static Error NotAuthenticated() => new(
        "Users.NotAuthenticated", "You must be logged in.", ErrorType.Unauthorized);

    public static Error NotFound(Guid id) => new(
        "Users.NotFound", $"User with ID '{id}' was not found.", ErrorType.NotFound);
}