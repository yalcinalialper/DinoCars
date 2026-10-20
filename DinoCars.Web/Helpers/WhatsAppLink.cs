namespace DinoCars.Web.Helpers
{
    public static class WhatsAppLink
    {
        public static string Create(string phone, string? message = null)
        {
            var url = $"https://wa.me/{ToInternational(phone)}";
            return string.IsNullOrWhiteSpace(message) ? url : $"{url}?text={Uri.EscapeDataString(message)}";
        }

        // wa.me needs the number in international format, digits only: 32491888824
        private static string ToInternational(string phone)
        {
            var digits = new string(phone.Where(char.IsDigit).ToArray());

            if (phone.TrimStart().StartsWith('+')) return digits;          // +32 491... → 32491...
            if (digits.StartsWith("00")) return digits[2..];               // 0032 491... → 32491...
            if (digits.StartsWith('0')) return "32" + digits[1..];         // 0491... → 32491... (Belgian default)
            return digits;
        }
    }
}
