namespace Statua.Domain.Entities.ImageAggregate.ValueObjects;

public readonly record struct GenerationParameters
{
	public string Prompt { get; }
	public string? NegativePrompt { get; }
	public string Seed { get; }
	public string Model { get; }
	public string AspectRatio { get; }
	public TimeSpan GenerationTime { get; }

	private GenerationParameters(
				 string prompt,
				 string? negativePrompt,
				 string seed,
				 string model,
				 string aspectRatio,
				 TimeSpan generationTime)
	{
		Prompt = prompt;
		NegativePrompt = negativePrompt;
		Seed = seed;
		Model = model;
		AspectRatio = aspectRatio;
		GenerationTime = generationTime;
	}

	public static GenerationParameters Create(
				 string prompt,
				 string? negativePrompt = null,
				 string? seed = null,
				 string model = "stable-diffusion-xl",
				 string aspectRatio = "16:9",
				 TimeSpan? generationTime = null)
	{
		if (string.IsNullOrWhiteSpace(prompt) || prompt.Length > 1000)
			throw new ArgumentException("Prompt is required and must be less than 1000 characters");

		if (string.IsNullOrWhiteSpace(model))
			throw new ArgumentException("Model name is required");

		seed ??= Guid.NewGuid().ToString("N")[..8];

		return new GenerationParameters(
				prompt.Trim(),
				!string.IsNullOrWhiteSpace(negativePrompt) ? negativePrompt.Trim() : null,
				seed,
				model,
				aspectRatio,
				generationTime ?? TimeSpan.Zero
		);
	}

	public bool HasNegativePrompt => !string.IsNullOrWhiteSpace(NegativePrompt);

	public string ToLogFormat() =>
			$"Model: {Model}, Seed: {Seed}, Ratio: {AspectRatio}, Time: {GenerationTime:ss\\.ff}";
}
