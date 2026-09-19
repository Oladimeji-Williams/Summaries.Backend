namespace Summaries.SharedKernel.Common.Primitives;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type);