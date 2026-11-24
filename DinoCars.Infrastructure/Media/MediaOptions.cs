using Microsoft.Extensions.Hosting;

namespace DinoCars.Infrastructure.Media
{
    public sealed class MediaOptions
    {
        public const string SectionName = "Media";

        /// <summary>
        /// Folder for uploaded photos. A relative path is relative to the app's folder.
        /// On Azure App Service use a persistent folder such as /home/data/media, because
        /// the app's own folder is replaced on every deployment.
        /// </summary>
        public string LocalPath { get; set; } = "App_Data/media";

        /// <summary>URL path the photos are served under.</summary>
        public string RequestPath { get; set; } = "/media";

        internal string ResolveRootPath(IHostEnvironment environment) =>
            Path.GetFullPath(Path.Combine(environment.ContentRootPath, LocalPath));
    }
}
