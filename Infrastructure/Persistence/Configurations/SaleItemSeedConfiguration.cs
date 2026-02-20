using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Seed data configuration for SaleItem entity.
/// </summary>
public class SaleItemSeedConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasData(
            // Venta 1: Laptop + Mouse
            new
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                SaleId = SaleSeedConfiguration.Sale1Id,
                ProductId = ProductSeedConfiguration.Product1Id,
                Quantity = 1,
                UnitPrice = 1299.99m
            },
            new
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                SaleId = SaleSeedConfiguration.Sale1Id,
                ProductId = ProductSeedConfiguration.Product2Id,
                Quantity = 1,
                UnitPrice = 99.99m
            },
            // Venta 2: Teclado + Monitor
            new
            {
                Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                SaleId = SaleSeedConfiguration.Sale2Id,
                ProductId = ProductSeedConfiguration.Product3Id,
                Quantity = 1,
                UnitPrice = 149.99m
            },
            new
            {
                Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                SaleId = SaleSeedConfiguration.Sale2Id,
                ProductId = ProductSeedConfiguration.Product4Id,
                Quantity = 1,
                UnitPrice = 449.99m
            },
            // Venta 3: Audífonos + Webcam
            new
            {
                Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                SaleId = SaleSeedConfiguration.Sale3Id,
                ProductId = ProductSeedConfiguration.Product5Id,
                Quantity = 1,
                UnitPrice = 349.99m
            },
            new
            {
                Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                SaleId = SaleSeedConfiguration.Sale3Id,
                ProductId = ProductSeedConfiguration.Product6Id,
                Quantity = 1,
                UnitPrice = 79.99m
            },
            // Venta 4: Laptop + SSD + Teclado
            new
            {
                Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                SaleId = SaleSeedConfiguration.Sale4Id,
                ProductId = ProductSeedConfiguration.Product1Id,
                Quantity = 1,
                UnitPrice = 1299.99m
            },
            new
            {
                Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                SaleId = SaleSeedConfiguration.Sale4Id,
                ProductId = ProductSeedConfiguration.Product7Id,
                Quantity = 1,
                UnitPrice = 129.99m
            },
            new
            {
                Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                SaleId = SaleSeedConfiguration.Sale4Id,
                ProductId = ProductSeedConfiguration.Product3Id,
                Quantity = 1,
                UnitPrice = 149.99m
            },
            // Venta 5: 2 Monitores
            new
            {
                Id = Guid.Parse("e1111111-1111-1111-1111-111111111111"),
                SaleId = SaleSeedConfiguration.Sale5Id,
                ProductId = ProductSeedConfiguration.Product4Id,
                Quantity = 2,
                UnitPrice = 449.99m
            }
        );
    }
}
