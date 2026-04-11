namespace Statua.Domain.Entities.ImageAggregate.ValueObjects;

public readonly record struct FileInformation
{
	public const long MaxFileSizeBytes = 100L * 1024 * 1024; // 100 MB

	public string FileName { get; }
	public string MimeType { get; }
	public long SizeBytes { get; }
	public string Extension { get; }

	public bool IsImage => MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
	public double SizeInMb => SizeBytes / (1024.0 * 1024.0);

	private FileInformation(string fileName, string mimeType, long sizeBytes, string extension)
	{
		FileName = fileName;
		MimeType = mimeType;
		SizeBytes = sizeBytes;
		Extension = extension;
	}

	public static FileInformation Create(string fileName, string mimeType, long sizeBytes)
	{
		if (string.IsNullOrWhiteSpace(fileName))
			throw new ArgumentException("File name is required", nameof(fileName));

		if (sizeBytes <= 0)
			throw new ArgumentException("Size must be greater than zero", nameof(sizeBytes));

		if (sizeBytes > MaxFileSizeBytes)
			throw new Exceptions.ImageAggregate.InvalidFileSizeException(
				sizeBytes, MaxFileSizeBytes,
				$"File size {sizeBytes} bytes exceeds maximum allowed {MaxFileSizeBytes} bytes");

		var extension = Path.GetExtension(fileName)?.ToLowerInvariant() ?? "";

		if (string.IsNullOrWhiteSpace(extension))
			throw new ArgumentException("File must have an extension", nameof(fileName));

		return new FileInformation(fileName, mimeType, sizeBytes, extension);
	}

	public static FileInformation FromUri(string fileUri, string mimeType, long sizeBytes) =>
		Create(Path.GetFileName(fileUri), mimeType, sizeBytes);

	public string ToDisplaySize()
	{
		return SizeBytes < 1024
			? $"{SizeBytes} B"
			: SizeBytes < 1024 * 1024
				? $"{SizeBytes / 1024f:F1} KB"
				: $"{SizeBytes / (1024f * 1024f):F1} MB";
	}

}
