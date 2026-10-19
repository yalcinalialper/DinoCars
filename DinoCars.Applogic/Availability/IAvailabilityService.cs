namespace DinoCars.AppLogic.Availability
{
    public interface IAvailabilityService
    {
        Task<IReadOnlyList<AvailableSlot>> GetAvailableSlotsAsync(DateOnly localDate, int durationMinutes, CancellationToken cancellationToken = default);
        Task<bool> IsSlotAvailableAsync(DateTime startUtc, int durationMinutes, CancellationToken cancellationToken = default);
    }
}
