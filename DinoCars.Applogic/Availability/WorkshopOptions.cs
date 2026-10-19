namespace DinoCars.AppLogic.Availability
{
    public sealed class WorkshopOptions
    {
        public const string SectionName = "Workshop";

        public string TimeZoneId { get; set; } = "Europe/Brussels";
        public Dictionary<DayOfWeek, DayHours> OpeningHours { get; set; } = new();
        public int Capacity { get; set; } = 1;
        public int SlotIntervalMinutes { get; set; } = 60;
        public int MinimumNoticeMinutes { get; set; } = 60;
    }

    public sealed class DayHours
    {
        public TimeOnly Open { get; set; }
        public TimeOnly Close { get; set; }
    }
}
