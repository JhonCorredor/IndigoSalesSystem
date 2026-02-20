using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Sale entity.
/// </summary>
public class SaleSeedConfiguration : IEntityTypeConfiguration<Sale>
{
    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid Sale1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid Sale2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid Sale3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid Sale4Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid Sale5Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasData(
            new
            {
                Id = Sale1Id,
                Date = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                Total = 1449.98m,
                CreatedAt = SeedDate,
                IsActive = true
            },
            new
            {
                Id = Sale2Id,
                Date = new DateTime(2024, 1, 16, 14, 45, 0, DateTimeKind.Utc),
                Total = 599.98m,
                CreatedAt = SeedDate,
                IsActive = true
            },
            new
            {
                Id = Sale3Id,
                Date = new DateTime(2024, 1, 17, 9, 15, 0, DateTimeKind.Utc),
                Total = 429.98m,
                CreatedAt = SeedDate,
                IsActive = true
            },
            new
            {
                Id = Sale4Id,
                Date = new DateTime(2024, 1, 18, 16, 20, 0, DateTimeKind.Utc),
                Total = 1579.97m,
                CreatedAt = SeedDate,
                IsActive = true
            },
            new
            {
                Id = Sale5Id,
                Date = new DateTime(2024, 1, 19, 11, 0, 0, DateTimeKind.Utc),
                Total = 899.97m,
                CreatedAt = SeedDate,
                IsActive = true
            }
        );
    }
}
