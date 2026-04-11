namespace Statua.Domain.Events.Common;

public abstract class DomainEventBase : IDomainEvent
{
	public DateTime OccurredOn { get; } = DateTime.UtcNow;
	public Guid EventId { get; } = Guid.NewGuid();
	public Guid AggregateRootId { get; protected set; } = Guid.Empty;
	public int Version { get; protected set; } = 1;

	/// <summary>
	/// Propiedad de solo lectura para el nombre del tipo (usado por event buses).
	/// </summary>
	public abstract string EventType { get; }

	/// <summary>
	/// Configura el AggregateRootId (útil cuando se crea el evento fuera del contexto).
	/// </summary>
	protected void SetAggregateRootId(Guid aggregateRootId) =>
			AggregateRootId = aggregateRootId;
}
