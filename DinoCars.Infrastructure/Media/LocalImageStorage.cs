using DinoCars.AppLogic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DinoCars.Infrastructure.Media
{
    /// <summary>Stores images in a folder on the server's disk. Swap for a Blob Storage version when needed.</summary>
    internal sealed class LocalImageStorage : IImageStorage
    {
        private readonly string _rootPath;
        private readonly string _requestPath;

        public LocalImageStorage(IOptions<MediaOptions> options, IHostEnvironment environment)
        {
            _rootPath = options.Value.ResolveRootPath(environment);
            _requestPath = options.Value.RequestPath.Trim('/');
        }

        public async Task SaveAsync(string key, ReadOnlyMemory<byte> content, string contentType, CancellationToken cancellationToken = default)
        {
            var path = ResolvePath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            // Write to a temporary file first, then rename: visitors never see a half-written photo.
            var temporaryPath = path + ".tmp";
            await using (var file = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true))
            {
                await file.WriteAsync(content, cancellationToken);
            }

            File.Move(temporaryPath, path, overwrite: true);
        }

        public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            var path = ResolvePath(key);

            if (File.Exists(path))
                File.Delete(path);

            return Task.CompletedTask;
        }

        public string GetPublicUrl(string key) => $"{_requestPath}/{key}";

        /// <summary>Turns a key into a file path, and refuses keys that try to escape the media folder ("../").</summary>
        private string ResolvePath(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var fullPath = Path.GetFullPath(Path.Combine(_rootPath, key));

            if (!fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                throw new ArgumentException("Invalid storage key.", nameof(key));

            return fullPath;
        }
    }
}
