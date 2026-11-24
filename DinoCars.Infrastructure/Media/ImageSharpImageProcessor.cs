using DinoCars.AppLogic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace DinoCars.Infrastructure.Media
{
    /// <summary>
    /// Makes uploaded photos safe and web-ready with ImageSharp:
    /// only real JPG/PNG/WebP images are accepted, they are rotated upright, all metadata
    /// (including GPS location) is removed, and two compressed JPG versions are created.
    /// </summary>
    internal sealed class ImageSharpImageProcessor : IImageProcessor
    {
        private const int LargeMaxSize = 1600;
        private const int ThumbnailMaxSize = 800;
        private const int JpegQuality = 82;
        private const string JpegContentType = "image/jpeg";

        // Protects against "decompression bombs": tiny files that decode into gigantic images.
        private const long MaxPixels = 50_000_000;

        private static readonly DecoderOptions Decoder = new()
        {
            // Only these formats are recognised; anything else is rejected as "not an image".
            Configuration = new Configuration(
                new JpegConfigurationModule(),
                new PngConfigurationModule(),
                new WebpConfigurationModule()),
            MaxFrames = 1
        };

        private static readonly JpegEncoder Encoder = new() { Quality = JpegQuality };

        public async Task<ProcessedImage> ProcessAsync(Stream source, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(source);

            using var image = await LoadAsync(source, cancellationToken);

            image.Mutate(x => x.AutoOrient().BackgroundColor(Color.White));
            RemoveMetadata(image);

            var large = await EncodeAsync(image, LargeMaxSize, cancellationToken);
            var thumbnail = await EncodeAsync(image, ThumbnailMaxSize, cancellationToken);

            return new ProcessedImage(large, thumbnail);
        }

        private static async Task<Image> LoadAsync(Stream source, CancellationToken cancellationToken)
        {
            // Uploads arrive as a one-way stream; buffer it so it can be read twice (header check, then decode).
            await using var buffer = await CopyWithLimitAsync(source, cancellationToken);

            try
            {
                var info = await Image.IdentifyAsync(Decoder, buffer, cancellationToken);

                if ((long)info.Width * info.Height > MaxPixels)
                    throw new InvalidImageException("The image has too many pixels.");

                buffer.Position = 0;
                return await Image.LoadAsync(Decoder, buffer, cancellationToken);
            }
            catch (ImageFormatException ex)
            {
                throw new InvalidImageException("The file is not a supported image.", ex);
            }
        }

        private static async Task<MemoryStream> CopyWithLimitAsync(Stream source, CancellationToken cancellationToken)
        {
            var buffer = new MemoryStream();
            var chunk = new byte[81920];
            int bytesRead;

            while ((bytesRead = await source.ReadAsync(chunk, cancellationToken)) > 0)
            {
                if (buffer.Length + bytesRead > ImageLimits.MaxUploadBytes)
                {
                    await buffer.DisposeAsync();
                    throw new InvalidImageException("The image file is too large.");
                }

                await buffer.WriteAsync(chunk.AsMemory(0, bytesRead), cancellationToken);
            }

            buffer.Position = 0;
            return buffer;
        }

        /// <summary>EXIF (camera, date, GPS), IPTC and XMP can contain personal data; none of it is needed on the web.</summary>
        private static void RemoveMetadata(Image image)
        {
            image.Metadata.ExifProfile = null;
            image.Metadata.IptcProfile = null;
            image.Metadata.XmpProfile = null;

            // Some formats also keep metadata per frame.
            foreach (var frame in image.Frames)
            {
                frame.Metadata.ExifProfile = null;
                frame.Metadata.IptcProfile = null;
                frame.Metadata.XmpProfile = null;
            }
        }

        private static async Task<ImageVariant> EncodeAsync(Image image, int maxSize, CancellationToken cancellationToken)
        {
            // Only make images smaller, never bigger.
            using var resized = image.Width > maxSize || image.Height > maxSize
                ? image.Clone(x => x.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(maxSize, maxSize) }))
                : image.Clone(_ => { });

            using var output = new MemoryStream();
            await resized.SaveAsJpegAsync(output, Encoder, cancellationToken);

            return new ImageVariant(output.ToArray(), resized.Width, resized.Height, JpegContentType);
        }
    }
}
