namespace DinoCars.AppLogic
{
    internal sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
