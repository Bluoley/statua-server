using Statua.Domain.Events.Common;

namespace Statua.Domain.Events.ImageAggregate;

public class ImageStatusChangedDomainEvent : DomainEventBase
{
	public override string EventType => nameof(ImageStatusChangedDomainEvent);
	public Guid ImageId { get; }
	public Enums.ImageStatus PreviousStatus { get; }
	public Enums.ImageStatus NewStatus { get; }

	public ImageStatusChangedDomainEvent(Guid imageId, Enums.ImageStatus previousStatus, Enums.ImageStatus newStatus)
	{
		SetAggregateRootId(imageId);
		ImageId = imageId;
		PreviousStatus = previousStatus;
		NewStatus = newStatus;

	}
}
