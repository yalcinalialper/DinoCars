using DinoCars.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DinoCars.Infrastructure.Data.Configurations
{
    internal sealed class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            builder.ToTable("ServiceTypes");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedNever();

            builder.Property(s => s.Code).HasMaxLength(50).IsRequired();
            builder.Property(s => s.Name).HasMaxLength(100).IsRequired();

            builder.HasIndex(s => s.Code).IsUnique();
        }
    }
}
