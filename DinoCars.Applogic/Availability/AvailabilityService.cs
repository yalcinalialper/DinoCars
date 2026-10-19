using Microsoft.Extensions.Options;

namespace DinoCars.AppLogic.Availability
{
    internal sealed class AvailabilityService(
        IReservationRepository reservations,
        IClock clock,
        IOptions<WorkshopOptions> options) : IAvailabilityService
    {
        private readonly WorkshopOptions _workshop = options.Value;
        private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId);

        public async Task<IReadOnlyList<AvailableSlot>> GetAvailableSlotsAsync(
            DateOnly localDate, int durationMinutes, CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationMinutes);

            if (!_workshop.OpeningHours.TryGetValue(localDate.DayOfWeek, out var hours))
                return [];

            var openingMinutes = (int)hours.Open.ToTimeSpan().TotalMinutes;
            var closingMinutes = (int)hours.Close.ToTimeSpan().TotalMinutes;

            var dayStartUtc = ToUtc(localDate, openingMinutes);
            var dayEndUtc = ToUtc(localDate, closingMinutes);

            // One database call for the whole day, then everything else happens in memory.
            var existing = await reservations.GetBlockingInRangeAsync(dayStartUtc, dayEndUtc, cancellationToken);
            var earliestAllowedUtc = clock.UtcNow.AddMinutes(_workshop.MinimumNoticeMinutes);

            var slots = new List<AvailableSlot>();

            for (var start = openingMinutes; start + durationMinutes <= closingMinutes; start += _workshop.SlotIntervalMinutes)
            {
                var startUtc = ToUtc(localDate, start);
                var endUtc = startUtc.AddMinutes(durationMinutes);

                if (startUtc < earliestAllowedUtc)
                    continue;

                var overlapping = existing.Count(r => r.Overlaps(startUtc, endUtc));

                if (overlapping < _workshop.Capacity)
                    slots.Add(new AvailableSlot(startUtc, TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(start))));
            }

            return slots;
        }

        public async Task<bool> IsSlotAvailableAsync(
            DateTime startUtc, int durationMinutes, CancellationToken cancellationToken = default)
        {
            var localDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(startUtc, _timeZone));
            var slots = await GetAvailableSlotsAsync(localDate, durationMinutes, cancellationToken);
            return slots.Any(s => s.StartUtc == startUtc);
        }

        private DateTime ToUtc(DateOnly localDate, int minutesSinceMidnight)
        {
            var local = localDate.ToDateTime(TimeOnly.MinValue).AddMinutes(minutesSinceMidnight);
            return TimeZoneInfo.ConvertTimeToUtc(local, _timeZone);
        }
    }
}
