namespace Application.Common.Models;

public class PagedResult<T>
{
    private PagedResult(IEnumerable<T> items, int page, int pageSize, long totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public IEnumerable<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public int TotalPages { get; }

    public static PagedResult<T> Create(IEnumerable<T> items, int page, int pageSize, long totalCount)
        => new(items, page, pageSize, totalCount);
}