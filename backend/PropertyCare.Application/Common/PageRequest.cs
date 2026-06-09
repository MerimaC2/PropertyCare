namespace PropertyCare.Application.Common;

public sealed class PageRequest
{
    public const int MaxPageSize = 100;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    /// <summary>Returns a copy with values clamped to sane bounds.</summary>
    public PageRequest Normalize()
    {
        return new PageRequest
        {
            Page = Page < 1 ? 1 : Page,
            PageSize = PageSize < 1 ? 20 : Math.Min(PageSize, MaxPageSize)
        };
    }
}
