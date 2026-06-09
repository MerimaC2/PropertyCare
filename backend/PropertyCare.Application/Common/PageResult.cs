namespace PropertyCare.Application.Common;

public sealed class PageResult<T>
{
    public IReadOnlyList<T> Items { get; private init; } = [];
    public int PageSize { get; private init; }
    public int CurrentPage { get; private init; }
    public int TotalItems { get; private init; }
    public int TotalPages { get; private init; }

    public static async Task<PageResult<T>> FromQueryableAsync(
        IQueryable<T> query,
        PageRequest paging,
        CancellationToken ct)
    {
        var normalized = paging.Normalize();

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .ToListAsync(ct);

        return new PageResult<T>
        {
            Items = items.AsReadOnly(),
            PageSize = normalized.PageSize,
            CurrentPage = normalized.Page,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling((decimal)total / normalized.PageSize)
        };
    }
}
