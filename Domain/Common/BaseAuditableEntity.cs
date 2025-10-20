namespace Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset? UpdatedAt { get; set; }
	public DateTimeOffset? DeletedAt { get; set; }
	public bool IsDeleted { get; set; }
}