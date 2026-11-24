namespace DinoCars.AppLogic
{
    /// <summary>Turns an uploaded photo into safe, web-ready versions.</summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Checks that the stream really is an image, rotates it upright, removes all metadata
        /// (including GPS location) and returns a large and a thumbnail version.
        /// </summary>
        /// <exception cref="InvalidImageException">The stream is not a supported image.</exception>
        Task<ProcessedImage> ProcessAsync(Stream source, CancellationToken cancellationToken = default);
    }

    public sealed record ProcessedImage(ImageVariant Large, ImageVariant Thumbnail);

    public sealed record ImageVariant(ReadOnlyMemory<byte> Content, int Width, int Height, string ContentType);

    public sealed class InvalidImageException(string message, Exception? innerException = null)
        : Exception(message, innerException);
}
