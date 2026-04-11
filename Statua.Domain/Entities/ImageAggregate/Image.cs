using Statua.Domain.Common;
using Statua.Domain.Entities.ImageAggregate.ValueObjects;
using Statua.Domain.Enums;
using Statua.Domain.Events.ImageAggregate;
using Statua.Domain.Exceptions.ImageAggregate;
using Statua.Domain.Interfaces;

namespace Statua.Domain.Entities.ImageAggregate;

public class Image : BaseEntity, IAggregateRoot
{
	public string? Slug { get; private set; }
	public string? Url { get; private set; }
	public Guid UserId { get; private set; }

	public FileInformation FileInformation { get; private set; }
	public ImageDimensions Dimensions { get; private set; }

	public ImageStatus Status { get; private set; }
	public DateTime? ProcessingStartedAt { get; private set; }
	public DateTime? ProcessingCompletedAt { get; private set; }

	public ImageDetail? Detail
	{
		get; private set;

	}

	private Image()
	{
		Status = ImageStatus.Pending;
	}

	public static Image Create(
			string fileName,
			string url,
			int width,
			int height,
			string mimeType,
			long sizeBytes,
			Guid userId,
			string? blurHash = null)
	{
		var fileInfo = FileInformation.Create(fileName, mimeType, sizeBytes);
		var dimensions = ImageDimensions.Create(width, height);
		var slug = GenerateSlug(fileName);

		var image = new Image
		{
			Slug = slug,
			Url = url,
			UserId = userId,
			FileInformation = fileInfo,
			Dimensions = dimensions,
			Status = ImageStatus.Pending
		};

		image.AddDomainEvent(new ImageCreatedDomainEvent(image.Id, userId, fileName));

		return image;
	}

	public static Image CreateGenerated(
		string fileName,
		string url,
		int width,
		int height,
		string mimeType,
		long sizeBytes,
		Guid userId,
		GenerationParameters generationParams,
		string? blurHash = null)
	{
		var image = Create(fileName, url, width, height, mimeType, sizeBytes, userId, blurHash);

		// Crear el detalle asociado
		var detail = ImageDetail.Create(generationParams, image.Id);
		image.Detail = detail;

		return image;
	}

	/// <summary>
	/// Inicia el procesamiento de la imagen.
	/// Transición: Pending → Processing
	/// </summary>
	public void StartProcessing()
	{
		ValidateStatusTransition(ImageStatus.Pending);

		Status = ImageStatus.Pending; // Mantenemos Pending mientras se procesa
		ProcessingStartedAt = DateTime.UtcNow;

		AddDomainEvent(new ImageProcessingStartedDomainEvent(Id));
	}

	/// <summary>
	/// Marca la imagen como completada exitosamente.
	/// Transición: Pending → Completed
	/// </summary>
	public void MarkAsCompleted(TimeSpan? processingDuration = null)
	{
		var previousStatus = Status;
		Status = ImageStatus.Completed;
		ProcessingCompletedAt = DateTime.UtcNow;

		var duration = processingDuration ?? CalculateProcessingDuration();

		AddDomainEvent(new ImageStatusChangedDomainEvent(Id, previousStatus, Status));
		AddDomainEvent(new ImageProcessingCompletedDomainEvent(Id, Status, duration));
	}

	/// <summary>
	/// Marca la imagen como fallida.
	/// Transición: Pending/Processing → Failed
	/// </summary>
	public void MarkAsFailed(string? failureReason = null)
	{
		var previousStatus = Status;
		Status = ImageStatus.Failed;
		ProcessingCompletedAt = DateTime.UtcNow;

		// if (!string.IsNullOrWhiteSpace(failureReason))
		// {
		// 	Metadata = Metadata.WithAttribute("failureReason", failureReason);
		// }

		AddDomainEvent(new ImageStatusChangedDomainEvent(Id, previousStatus, Status));
	}

	/// <summary>
	/// Reagenda la imagen para reprocesamiento.
	/// Transición: Failed/Completed → Pending
	/// </summary>
	public void ScheduleForRedo()
	{
		if (Status != ImageStatus.Failed && Status != ImageStatus.Completed)
		{
			throw new ImageStateException(Status, ImageStatus.Redo);
		}

		var previousStatus = Status;
		Status = ImageStatus.Redo;
		ProcessingStartedAt = null;
		ProcessingCompletedAt = null;

		AddDomainEvent(new ImageStatusChangedDomainEvent(Id, previousStatus, Status));
	}

	/// <summary>
	/// Actualiza el BlurHash después de procesar la imagen.
	/// </summary>
	// public void UpdateBlurHash(string blurHash)
	// {
	// 	if (string.IsNullOrWhiteSpace(blurHash))
	// 		throw new ArgumentException("BlurHash cannot be empty");

	// 	Metadata = Metadata.WithBlurHash(blurHash);
	// }

	/// <summary>
	/// Agrega un atributo custom al metadata.
	/// </summary>
	// public void AddMetadataAttribute(string key, string value)
	// {
	// 	Metadata = Metadata.WithAttribute(key, value);
	// }

	// --- Métodos de Negocio ---

	public bool IsPending => Status == ImageStatus.Pending;
	public bool IsCompleted => Status == ImageStatus.Completed;
	public bool IsFailed => Status == ImageStatus.Failed;
	public bool NeedsProcessing => Status == ImageStatus.Pending || Status == ImageStatus.Redo;

	public ImageOrientation Orientation => Dimensions.Orientation;

	public string GetDisplayUrl() => Url!;

	public bool HasDetail => Detail != null;

	// --- Métodos Internos ---

	private static string GenerateSlug(string fileName)
	{
		var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		var extension = Path.GetExtension(fileName);

		// Slug más corto y SEO-friendly
		var shortGuid = Guid.NewGuid().ToString()[..8];
		var slug = $"{nameWithoutExtension[..Math.Min(50, nameWithoutExtension.Length)]}-{shortGuid}{extension}";

		return slug;
	}

	private void ValidateStatusTransition(ImageStatus expectedStatus)
	{
		if (Status != expectedStatus)
		{
			throw new ImageStateException(Status, expectedStatus);
		}
	}

	private TimeSpan CalculateProcessingDuration()
	{
		if (ProcessingStartedAt == null)
			return TimeSpan.Zero;

		var end = ProcessingCompletedAt ?? DateTime.UtcNow;
		return end - ProcessingStartedAt.Value;
	}

}
