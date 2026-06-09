namespace PropertyCare.Application.Common;

/// <summary>Base for queries that return a paged result.</summary>
public abstract class BasePagedQuery<TDto> : IRequest<PageResult<TDto>>
{
    public PageRequest Paging { get; set; } = new();
}
