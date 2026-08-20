namespace Summaries.API.Contracts.Common;

public sealed record ApiError(
    string Code,
    string Message,
    string Type);