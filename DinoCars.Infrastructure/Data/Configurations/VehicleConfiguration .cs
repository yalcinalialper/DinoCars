using DinoCars.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DinoCars.Infrastructure.Data.Configurations
{
    internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).ValueGeneratedNever();

            builder.Property(v => v.Make).HasMaxLength(50).IsRequired();
            builder.Property(v => v.Model).HasMaxLength(50).IsRequired();
            builder.Property(v => v.LicensePlate).HasMaxLength(20).IsRequired();
            builder.Property(v => v.Vin).HasMaxLength(17);

            builder.HasIndex(v => v.LicensePlate);
            builder.HasIndex(v => new { v.CustomerId, v.LicensePlate }).IsUnique();
        }
    }
}
