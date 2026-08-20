namespace Summaries.API.Contracts.Common;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    IReadOnlyList<ApiError>? Errors);