using Statua.Domain.Events.Common;

namespace Statua.Domain.Events.ImageAggregate;

public class ImageProcessingCompletedDomainEvent : DomainEventBase
{
	public override string EventType => nameof(ImageProcessingCompletedDomainEvent);

	public Guid ImageId { get; }
	public Enums.ImageStatus FinalStatus { get; }
	public TimeSpan ProcessingDuration { get; }

	public ImageProcessingCompletedDomainEvent(Guid imageId, Enums.ImageStatus finalStatus, TimeSpan duration)
	{
		SetAggregateRootId(imageId);
		ImageId = imageId;
		FinalStatus = finalStatus;
		ProcessingDuration = duration;
	}
}
