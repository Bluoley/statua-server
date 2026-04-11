namespace Statua.Domain.Exceptions.ImageAggregate;

public class ImageStateException : ImageDomainException
{

	public Enums.ImageStatus CurrentStatus { get; }
	public Enums.ImageStatus RequestedStatus { get; }

	public ImageStateException(Enums.ImageStatus currentStatus, Enums.ImageStatus requestedStatus)
			: base($"Cannot transition from {currentStatus} to {requestedStatus}")
	{
		CurrentStatus = currentStatus;
		RequestedStatus = requestedStatus;
	}
}
