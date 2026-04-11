namespace Statua.Domain.Events.Common;

public interface IDomainEvent
{
	DateTime OccurredOn { get; }
	Guid EventId { get; }
	Guid AggregateRootId { get; }
	int Version { get; }
	string EventType { get; }
}
