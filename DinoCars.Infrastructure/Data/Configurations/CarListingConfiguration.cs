using DinoCars.Domain.Listings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DinoCars.Infrastructure.Data.Configurations
{
    internal sealed class CarListingConfiguration : IEntityTypeConfiguration<CarListing>
    {
        public void Configure(EntityTypeBuilder<CarListing> builder)
        {
            builder.ToTable("CarListings");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).ValueGeneratedNever();

            builder.Property(l => l.Reference).HasMaxLength(CarListing.ReferenceLength).IsRequired();
            builder.HasIndex(l => l.Reference).IsUnique();

            builder.Property(l => l.Make).HasMaxLength(CarListing.MaxNameLength).IsRequired();
            builder.Property(l => l.Model).HasMaxLength(CarListing.MaxNameLength).IsRequired();
            builder.Property(l => l.Version).HasMaxLength(CarListing.MaxVersionLength);
            builder.Property(l => l.Color).HasMaxLength(CarListing.MaxColorLength);
            builder.Property(l => l.Description).HasMaxLength(CarListing.MaxDescriptionLength);
            builder.Property(l => l.Price).HasPrecision(10, 2);

            // Enums as text: readable in the database, and safe if enum values are reordered later.
            builder.Property(l => l.FuelType).HasConversion<string>().HasMaxLength(20);
            builder.Property(l => l.Transmission).HasConversion<string>().HasMaxLength(20);
            builder.Property(l => l.Status).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(l => l.Status);

            // Calculated in code, not stored.
            builder.Ignore(l => l.Title);
            builder.Ignore(l => l.Slug);
            builder.Ignore(l => l.CoverPhoto);
            builder.Ignore(l => l.IsPublic);

            builder.HasMany(l => l.Photos)
                .WithOne()
                .HasForeignKey(p => p.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(l => l.Photos).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    internal sealed class ListingPhotoConfiguration : IEntityTypeConfiguration<ListingPhoto>
    {
        public void Configure(EntityTypeBuilder<ListingPhoto> builder)
        {
            builder.ToTable("ListingPhotos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.HasIndex(p => new { p.ListingId, p.SortOrder });
        }
    }
}
