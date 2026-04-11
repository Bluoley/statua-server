namespace Statua.Domain.Exceptions.ImageAggregate;

public class ImageDomainException : Exception
{
	public ImageDomainException(string message) : base(message)
	{
	}

	public ImageDomainException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
