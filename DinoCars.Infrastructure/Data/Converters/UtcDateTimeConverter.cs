using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DinoCars.Infrastructure.Data.Converters
{
    internal sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
        toDb => toDb.Kind == DateTimeKind.Utc ? toDb : toDb.ToUniversalTime(),
        fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));
}
