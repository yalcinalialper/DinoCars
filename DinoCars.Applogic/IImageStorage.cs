namespace DinoCars.AppLogic
{
    /// <summary>
    /// Stores public image files (car photos). Locally they go to disk; later this can be swapped
    /// for Azure Blob Storage without touching the use cases.
    /// </summary>
    public interface IImageStorage
    {
        Task SaveAsync(string key, ReadOnlyMemory<byte> content, string contentType, CancellationToken cancellationToken = default);

        /// <summary>Deletes the file. Does nothing if it doesn't exist.</summary>
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>The URL browsers use to load the file, relative to the site root (e.g. "media/listings/…").</summary>
        string GetPublicUrl(string key);
    }
}
