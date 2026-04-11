using Statua.Domain.Events.Common;

namespace Statua.Domain.Events.ImageAggregate;

public class ImageProcessingStartedDomainEvent : DomainEventBase
{
	public override string EventType => nameof(ImageProcessingStartedDomainEvent);
	public Guid ImageId { get; }

	public ImageProcessingStartedDomainEvent(Guid imageId)
	{
		SetAggregateRootId(imageId);
		ImageId = imageId;
	}
}
