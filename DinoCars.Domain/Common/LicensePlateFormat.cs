using DinoCars.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.Domain.Common
{
    public static class LicensePlateFormat
    {
        /// <summary>"1-abc-111", "1 ABC 111" and "1abc111" all become "1ABC111".</summary>
        public static string Normalize(string? input)
        {
            var normalized = new string((input ?? "").Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

            if (normalized.Length == 0)
                throw new DomainException("Geef een geldige nummerplaat in.");

            return normalized;
        }

        /// <summary>"1ABC111" becomes "1-ABC-111": a dash wherever letters and digits switch.</summary>
        public static string Display(string normalized)
        {
            if (string.IsNullOrEmpty(normalized))
                return normalized;

            var result = new StringBuilder(normalized.Length + 3);

            for (var i = 0; i < normalized.Length; i++)
            {
                if (i > 0 && char.IsDigit(normalized[i]) != char.IsDigit(normalized[i - 1]))
                    result.Append('-');

                result.Append(char.ToUpperInvariant(normalized[i]));
            }

            return result.ToString();
        }
    }
}
