using Statua.Domain.Events.Common;

namespace Statua.Domain.Events.ImageAggregate;

public class ImageCreatedDomainEvent : DomainEventBase
{
	public override string EventType => nameof(ImageCreatedDomainEvent);

	public Guid UserId { get; }
	public string FileName { get; }

	public ImageCreatedDomainEvent(Guid imageId, Guid userId, string fileName)
	{
		SetAggregateRootId(imageId);
		UserId = userId;
		FileName = fileName;
	}
}
