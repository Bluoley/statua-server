using Statua.Domain.Common;
using Statua.Domain.Entities.ImageAggregate.ValueObjects;

namespace Statua.Domain.Entities.ImageAggregate;

public class ImageDetail : BaseEntity
{
	public Guid ImageId { get; private set; }

	public string? Prompt { get; private set; }
	public string? NegativePrompt { get; private set; }
	public string? Seed { get; private set; }
	public string? Model { get; private set; }
	public string? AspectRatio { get; private set; }
	public TimeSpan GenerationTime { get; private set; }
	// public string Metadata { get; private set; } = "{}";

	private ImageDetail()
	{
	}

	private ImageDetail(
		Guid imageId,
		string prompt,
		string? negativePrompt,
		string seed,
		string model,
		string aspectRatio,
		TimeSpan generationTime
	)
	{
		ImageId = imageId;
		Prompt = prompt;
		NegativePrompt = negativePrompt;
		Seed = seed;
		Model = model;
		AspectRatio = aspectRatio;
		GenerationTime = generationTime;
	}
	public static ImageDetail Create(GenerationParameters parameters, Guid imageId)
	{
		return new ImageDetail(
				imageId: imageId,
				prompt: parameters.Prompt,
				negativePrompt: parameters.NegativePrompt,
				seed: parameters.Seed,
				model: parameters.Model,
				aspectRatio: parameters.AspectRatio,
				generationTime: parameters.GenerationTime
		);
	}

	public void UpdateGenerationTime(TimeSpan generationTime)
	{
		if (generationTime.TotalSeconds < 0)
			throw new ArgumentException("Generation time cannot be negative");

		GenerationTime = generationTime;
	}
}
