namespace DinoCars.AppLogic.Reservations
{
    /// <summary>
    /// Limits that stop spam or pranks from filling the calendar with fake online bookings.
    /// Staff bookings (phone, WhatsApp, walk-in) are never limited.
    /// </summary>
    public sealed class BookingProtectionOptions
    {
        public const string SectionName = "BookingProtection";

        /// <summary>How many upcoming (requested or approved) appointments one phone number may have at once.</summary>
        public int MaxOpenBookingsPerPhone { get; set; } = 3;

        /// <summary>How many online bookings the whole site accepts per hour, from all visitors together.</summary>
        public int MaxOnlineBookingsPerHour { get; set; } = 15;
    }
}
