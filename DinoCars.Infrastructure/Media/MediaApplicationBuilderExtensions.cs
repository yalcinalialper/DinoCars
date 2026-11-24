using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DinoCars.Infrastructure.Media
{
    public static class MediaApplicationBuilderExtensions
    {
        // Photo file names are unique and never change, so browsers may cache them for a year.
        private const string CacheControl = "public, max-age=31536000, immutable";

        /// <summary>Serves uploaded photos from the media folder (e.g. /media/listings/…/photo.jpg).</summary>
        public static IApplicationBuilder UseMediaFiles(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<MediaOptions>>().Value;
            var environment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

            var rootPath = options.ResolveRootPath(environment);
            Directory.CreateDirectory(rootPath);

            return app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(rootPath),
                RequestPath = options.RequestPath,
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.CacheControl = CacheControl;
                    context.Context.Response.Headers.XContentTypeOptions = "nosniff";
                }
            });
        }
    }
}
