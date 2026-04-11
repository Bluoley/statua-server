namespace Statua.Domain.Exceptions.ImageAggregate;

public class InvalidImageDimensionsException : ImageDomainException
{
	public int Width { get; }
	public int Height { get; }

	public InvalidImageDimensionsException(string message) : base(message)
	{
	}

	public InvalidImageDimensionsException(int width, int height, string message) : base(message)
	{
		Width = width;
		Height = height;
	}

	public InvalidImageDimensionsException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
