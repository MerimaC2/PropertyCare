namespace PropertyCare.Domain.Common;

/// <summary>
/// Base class for all entities: int PK, soft-delete flag and audit timestamps.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
}
