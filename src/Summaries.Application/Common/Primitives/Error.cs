namespace Summaries.Application.Common.Primitives;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type);