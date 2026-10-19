using DinoCars.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DinoCars.Infrastructure.Data.Configurations
{
    internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(c => c.LastName).HasMaxLength(100).IsRequired();
            builder.Property(c => c.MobilePhoneNumber).HasMaxLength(30).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(256);

            builder.HasIndex(c => c.Email);
            builder.HasIndex(c => c.MobilePhoneNumber);

            builder.HasMany(c => c.Vehicles)
                .WithOne()
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Vehicles).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
