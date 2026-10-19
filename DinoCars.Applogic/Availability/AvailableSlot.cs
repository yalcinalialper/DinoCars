namespace DinoCars.AppLogic.Availability
{
    public sealed record AvailableSlot(DateTime StartUtc, TimeOnly LocalStart);
}
