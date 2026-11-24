using System.Security;
using System.Text;
using DinoCars.AppLogic.Listings;
using DinoCars.Web.Helpers;

namespace DinoCars.Web.Endpoints
{
    /// <summary>
    /// robots.txt tells search engines what not to crawl; sitemap.xml lists every public page,
    /// including each car that is (or was recently) for sale.
    /// </summary>
    public static class SeoEndpoints
    {
        private static readonly string[] StaticPages = ["/", "/occasies", "/afspraak", "/privacy"];

        public static IEndpointRouteBuilder MapSeoEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/robots.txt", RobotsTxt);
            endpoints.MapGet("/sitemap.xml", SitemapXmlAsync);
            return endpoints;
        }

        private static IResult RobotsTxt(HttpRequest request) => Results.Text($"""
            User-agent: *
            Disallow: /beheer
            Disallow: /Account

            Sitemap: {BaseUrl(request)}/sitemap.xml
            """, "text/plain");

        private static async Task<IResult> SitemapXmlAsync(HttpRequest request, IListingCatalog catalog, CancellationToken cancellationToken)
        {
            var baseUrl = BaseUrl(request);
            var listings = await catalog.GetPublishedLinksAsync(cancellationToken);

            var xml = new StringBuilder("""<?xml version="1.0" encoding="UTF-8"?><urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");

            foreach (var page in StaticPages)
                AppendUrl(xml, $"{baseUrl}{page}");

            foreach (var listing in listings)
                AppendUrl(xml, $"{baseUrl}/{ListingFormat.Url(listing.Reference, listing.Slug)}", listing.UpdatedAtUtc);

            xml.Append("</urlset>");
            return Results.Text(xml.ToString(), "application/xml");
        }

        private static void AppendUrl(StringBuilder xml, string location, DateTime? lastModifiedUtc = null)
        {
            xml.Append("<url><loc>").Append(SecurityElement.Escape(location)).Append("</loc>");

            if (lastModifiedUtc is { } modified)
                xml.Append("<lastmod>").Append(modified.ToString("yyyy-MM-dd")).Append("</lastmod>");

            xml.Append("</url>");
        }

        private static string BaseUrl(HttpRequest request) => $"{request.Scheme}://{request.Host}";
    }
}
