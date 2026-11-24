namespace DinoCars.AppLogic
{
    /// <summary>Upload limits shared by the upload screen (early, friendly check) and the image processor (hard check).</summary>
    public static class ImageLimits
    {
        /// <summary>Phone photos are 3–10 MB; anything far bigger is refused before it is decoded.</summary>
        public const long MaxUploadBytes = 20 * 1024 * 1024;

        public const int MaxUploadMegabytes = (int)(MaxUploadBytes / (1024 * 1024));

        /// <summary>What the file picker offers. iPhones convert HEIC photos to JPEG when HEIC isn't listed.</summary>
        public const string AcceptedContentTypes = "image/jpeg,image/png,image/webp";
    }
}
