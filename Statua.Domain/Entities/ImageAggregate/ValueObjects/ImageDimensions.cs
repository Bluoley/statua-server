// Archivo: Statua.Domain/Entities/ImageAggregate/ValueObjects/ImageDimensions.cs
using Statua.Domain.Enums;
using Statua.Domain.Exceptions.ImageAggregate;

namespace Statua.Domain.Entities.ImageAggregate.ValueObjects;

/// <summary>
/// Value Object inmutable que representa las dimensiones de una imagen.
/// Encapsula validación, cálculos derivados y comportamiento relacionado con tamaño y orientación.
/// </summary>
public readonly record struct ImageDimensions
{
	// --- Constantes de Validación ---
	public const int MinDimension = 1;
	public const int MaxDimension = 100_000;
	public const long MaxPixelCount = 100_000L * 100_000L; // 10,000 Megapixels máx

	// --- Propiedades Inmutables ---
	public int Width { get; }
	public int Height { get; }

	// --- Propiedades Computadas (Derived Properties) ---

	/// <summary>
	/// Número total de píxeles en la imagen.
	/// </summary>
	public long PixelCount => (long)Width * Height;

	/// <summary>
	/// Orientación calculada automáticamente de las dimensiones.
	/// </summary>
	public ImageOrientation Orientation => Width > Height
			? ImageOrientation.Landscape
			: Width < Height
					? ImageOrientation.Portrait
					: ImageOrientation.Square;

	/// <summary>
	/// Relación de aspecto como double (ej: 1.78 para 16:9).
	/// </summary>
	public double AspectRatio => (double)Width / Height;

	/// <summary>
	/// Megapixels de la imagen.
	/// </summary>
	public double Megapixels => PixelCount / 1_000_000.0;

	// --- Constructores Privados (Patrón Factory) ---

	private ImageDimensions(int width, int height)
	{
		if (width < MinDimension || height < MinDimension)
			throw new InvalidImageDimensionsException(
					width, height,
					$"Dimensions must be at least {MinDimension}x{MinDimension}"
			);

		if (width > MaxDimension || height > MaxDimension)
			throw new InvalidImageDimensionsException(
					width, height,
					$"Dimensions cannot exceed {MaxDimension} pixels in any direction"
			);

		if ((long)width * height > MaxPixelCount)
			throw new InvalidImageDimensionsException(
					width, height,
					$"Total pixel count exceeds maximum allowed ({MaxPixelCount:N0} pixels)"
			);

		Width = width;
		Height = height;
	}

	// --- Factory Methods ---

	/// <summary>
	/// Crea un ImageDimensions desde ancho y alto.
	/// Valida los límites y lanza InvalidImageDimensionsException si es inválido.
	/// </summary>
	public static ImageDimensions Create(int width, int height) =>
			new(width, height);

	/// <summary>
	/// Crea dimensiones desde megapixels y relación de aspecto.
	/// Útil para calcular dimensiones de imágenes generadas por AI.
	/// </summary>
	/// <param name="megapixels">Cantidad de megapixels (ej: 4.0, 8.0, 16.0)</param>
	/// <param name="aspectRatio">Relación de aspecto (ej: 1.78 para 16:9, 1.33 para 4:3)</param>
	public static ImageDimensions FromMegapixels(double megapixels, double aspectRatio = 1.0)
	{
		if (megapixels <= 0)
			throw new ArgumentException("Megapixels must be greater than zero", nameof(megapixels));

		if (aspectRatio <= 0)
			throw new ArgumentException("Aspect ratio must be greater than zero", nameof(aspectRatio));

		var areaPixels = (long)(megapixels * 1_000_000);
		var height = (int)Math.Sqrt(areaPixels / aspectRatio);
		var width = (int)(height * aspectRatio);

		return Create(width, height);
	}

	/// <summary>
	/// Crea dimensiones desde una cadena en formato "anchoxalto".
	/// Ej: "1920x1080", "1024x768"
	/// </summary>
	public static ImageDimensions FromString(string dimensions)
	{
		if (string.IsNullOrWhiteSpace(dimensions))
			throw new ArgumentException("Dimensions string cannot be empty", nameof(dimensions));

		var parts = dimensions.Split('x', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		if (parts.Length != 2)
			throw new FormatException($"Invalid dimensions format. Expected 'widthxheight', got '{dimensions}'");

		if (!int.TryParse(parts[0], out var width) || !int.TryParse(parts[1], out var height))
			throw new FormatException($"Dimensions must be numeric. Got '{dimensions}'");

		return Create(width, height);
	}

	/// <summary>
	/// Crea dimensiones escalando unas dimensiones existentes.
	/// Mantiene la relación de aspecto original.
	/// </summary>
	public static ImageDimensions ScaledFrom(ImageDimensions source, int newWidth)
	{
		if (newWidth <= 0)
			throw new ArgumentException("New width must be positive", nameof(newWidth));

		var scale = (double)newWidth / source.Width;
		var newHeight = (int)(source.Height * scale);

		return Create(newWidth, newHeight);
	}

	// --- Métodos de Negocio ---

	/// <summary>
	/// Verifica si esta imagen puede contener otra imagen (las dimensiones son mayores o iguales).
	/// </summary>
	public bool CanContain(ImageDimensions other) =>
			Width >= other.Width && Height >= other.Height;

	/// <summary>
	/// Devuelve las dimensiones escaladas para caber en un bounding box sin deformar.
	/// </summary>
	public ImageDimensions FitIn(int maxWidth, int maxHeight)
	{
		if (maxWidth <= 0 || maxHeight <= 0)
			throw new ArgumentException("Max dimensions must be positive");

		if (Width <= maxWidth && Height <= maxHeight)
			return this;

		var ratioWidth = (double)maxWidth / Width;
		var ratioHeight = (double)maxHeight / Height;
		var scale = Math.Min(ratioWidth, ratioHeight);

		return Create(
				(int)(Width * scale),
				(int)(Height * scale)
		);
	}

	/// <summary>
	/// Devuelve las dimensiones escaladas para cubrir completamente un área (cover).
	/// </summary>
	public ImageDimensions Cover(int targetWidth, int targetHeight)
	{
		if (targetWidth <= 0 || targetHeight <= 0)
			throw new ArgumentException("Target dimensions must be positive");

		var ratioWidth = (double)targetWidth / Width;
		var ratioHeight = (double)targetHeight / Height;
		var scale = Math.Max(ratioWidth, ratioHeight);

		return Create(
				(int)(Width * scale),
				(int)(Height * scale)
		);
	}

	// --- Métodos de Utilidad ---

	/// <summary>
	/// Devuelve true si es una imagen de alta resolución (más de 2MP).
	/// </summary>
	public bool IsHighResolution => Megapixels > 2.0;

	/// <summary>
	/// Devuelve true si es una imagen 4K (al menos 3840x2160).
	/// </summary>
	public bool Is4K => Width >= 3840 && Height >= 2160;

	/// <summary>
	/// Devuelve true si es una imagen 8K (al menos 7680x4320).
	/// </summary>
	public bool Is8K => Width >= 7680 && Height >= 4320;

	/// <summary>
	/// Calcula el área en centímetros cuadrados para impresión (a 300 DPI).
	/// </summary>
	public double PrintAreaInCm(double dpi = 300)
	{
		const double inchesToCm = 2.54;
		var widthInches = Width / dpi;
		var heightInches = Height / dpi;
		return widthInches * heightInches * inchesToCm * inchesToCm;
	}

	// --- ToString Overrides ---

	public override string ToString() => $"{Width}x{Height}";

	public string ToFormattedString() => $"{Width} × {Height} px";

	public string ToHumanReadableString() =>
			$"{Width} × {Height} px ({Megapixels:F1} MP) - {Orientation}";

	/// <summary>
	/// Serializa para almacenamiento en BD o caché.
	/// </summary>
	public string ToJsonString() => $"{{\"w\":{Width},\"h\":{Height}}}";

	/// <summary>
	/// Deserializa desde formato JSON simple.
	/// </summary>
	public static ImageDimensions FromJsonString(string json)
	{
		// Nota: En producción usar System.Text.Json o Newtonsoft
		// Esto es solo demostrativo
		throw new NotImplementedException("Implement JSON parsing con System.Text.Json");
	}

}
