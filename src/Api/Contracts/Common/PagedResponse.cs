namespace Api.Contracts.Common;

public sealed record PagedResponse<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    long TotalCount,
    int TotalPages);