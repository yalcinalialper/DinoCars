using System.Globalization;
using System.Text;

namespace DinoCars.Domain.Common
{
    /// <summary>Turns text into a readable URL part: "Volkswagen Golf GTI 2018" → "volkswagen-golf-gti-2018".</summary>
    public static class SlugGenerator
    {
        private const int MaxLength = 80;

        public static string Create(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            // Split accented characters into letter + accent (é → e + ´), so the accent can be dropped.
            var decomposed = text.Normalize(NormalizationForm.FormD);
            var slug = new StringBuilder(decomposed.Length);
            var previousWasDash = true; // avoids a leading dash

            foreach (var character in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                    continue;

                if (char.IsAsciiLetterOrDigit(character))
                {
                    slug.Append(char.ToLowerInvariant(character));
                    previousWasDash = false;
                }
                else if (!previousWasDash)
                {
                    slug.Append('-');
                    previousWasDash = true;
                }

                if (slug.Length >= MaxLength)
                    break;
            }

            return slug.ToString().TrimEnd('-');
        }
    }
}
