namespace DinoCars.Domain
{
    public sealed class ReservationService
    {
        public Guid ReservationId { get; private set; }
        public Guid ServiceTypeId { get; private set; }
        public ServiceType ServiceType { get; private set; } = null!;
        public int DurationMinutes { get; private set; }

        private ReservationService() { } 

        internal ReservationService(Guid reservationId, ServiceType serviceType)
        {
            ReservationId = reservationId;
            ServiceTypeId = serviceType.Id;
            ServiceType = serviceType;
            DurationMinutes = serviceType.DurationMinutes; 
        }
    }
}
