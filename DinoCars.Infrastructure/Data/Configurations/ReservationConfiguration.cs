using DinoCars.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DinoCars.Infrastructure.Data.Configurations
{
    internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedNever();

            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(r => r.CustomerNotes).HasMaxLength(1000);
            builder.Property(r => r.AccessToken).HasMaxLength(64).IsRequired();

            builder.HasIndex(r => r.AccessToken).IsUnique();
            builder.HasIndex(r => r.StartUtc);

            builder.Ignore(r => r.BlocksCapacity);

            builder.HasOne(r => r.Vehicle)
                .WithMany()
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Services)
                .WithOne()
                .HasForeignKey(s => s.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(r => r.Services).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    internal sealed class ReservationServiceConfiguration : IEntityTypeConfiguration<ReservationService>
    {
        public void Configure(EntityTypeBuilder<ReservationService> builder)
        {
            builder.ToTable("ReservationServices");
            builder.HasKey(rs => new { rs.ReservationId, rs.ServiceTypeId });

            builder.HasOne(rs => rs.ServiceType)
                .WithMany()
                .HasForeignKey(rs => rs.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
