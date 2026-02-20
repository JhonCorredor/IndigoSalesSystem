using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Product entity.
/// </summary>
public class ProductSeedConfiguration : IEntityTypeConfiguration<Product>
{
    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid Product1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Product2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Product3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid Product4Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid Product5Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid Product6Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static readonly Guid Product7Id = Guid.Parse("77777777-7777-7777-7777-777777777777");

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasData(
            CreateProduct(Product1Id, "Laptop HP Pavilion 15", 1299.99m, 25, "https://images.unsplash.com/photo-1496181133206-80ce9b88a853"),
            CreateProduct(Product2Id, "Mouse Logitech MX Master 3", 99.99m, 50, "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46"),
            CreateProduct(Product3Id, "Teclado Mecánico Corsair K70", 149.99m, 35, "https://images.unsplash.com/photo-1511467687858-23d96c32e4ae"),
            CreateProduct(Product4Id, "Monitor Samsung 27\" 4K", 449.99m, 15, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf"),
            CreateProduct(Product5Id, "Audífonos Sony WH-1000XM4", 349.99m, 40, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e"),
            CreateProduct(Product6Id, "Webcam Logitech C920 HD", 79.99m, 60, "https://images.unsplash.com/photo-1587826080692-f439cd0b70da"),
            CreateProduct(Product7Id, "SSD Samsung 1TB NVMe", 129.99m, 45, "https://images.unsplash.com/photo-1597872200969-2b65d56bd16b")
        );
    }

    private static object CreateProduct(Guid id, string name, decimal price, int stock, string imageUrl)
    {
        return new
        {
            Id = id,
            Name = name,
            Price = price,
            Stock = stock,
            ImageUrl = imageUrl,
            CreatedAt = SeedDate,
            IsActive = true
        };
    }
}
