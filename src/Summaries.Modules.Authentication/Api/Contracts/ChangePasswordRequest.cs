namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);