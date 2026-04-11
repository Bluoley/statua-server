using Statua.Domain.Events.Common;

namespace Statua.Domain.Common;

public abstract class BaseEntity
{
	public Guid Id { get; protected set; }
	public DateTime CreatedAt { get; protected set; }
	public DateTime? UpdatedAt { get; protected set; }

	private readonly List<IDomainEvent> _domainEvents = [];
	public IReadOnlyCollection<IDomainEvent> DomainEVents => _domainEvents.AsReadOnly();

	protected BaseEntity()
	{
		Id = Guid.CreateVersion7();
		CreatedAt = DateTime.UtcNow;
	}

	protected void AddDomainEvent(IDomainEvent @event)
	{
		_domainEvents.Add(@event);
	}

	protected void RemoveDomainEvent(IDomainEvent @event)
	{
		_domainEvents.Remove(@event);
	}

	protected void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}
