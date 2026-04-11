namespace Statua.Domain.Exceptions.ImageAggregate;

public class InvalidFileSizeException : ImageDomainException
{
	public long SizeBytes { get; }
	public long MaxAllowedBytes { get; }

	public InvalidFileSizeException(string message) : base(message)
	{
	}

	public InvalidFileSizeException(long sizeBytes, long maxAllowedBytes, string message)
			: base(message)
	{
		SizeBytes = sizeBytes;
		MaxAllowedBytes = maxAllowedBytes;
	}
}
