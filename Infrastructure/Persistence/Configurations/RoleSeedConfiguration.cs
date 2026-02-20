using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Role entity.
/// </summary>
public class RoleSeedConfiguration : IEntityTypeConfiguration<Role>
{
    public static readonly Guid AdminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid ManagerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid SellerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid ViewerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000004");
    public static readonly Guid GuestRoleId = Guid.Parse("10000000-0000-0000-0000-000000000005");

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new
            {
                Id = AdminRoleId,
                Name = "Admin",
                Description = "Administrador del sistema con acceso total",
                IsActive = true,
                CreatedAt = createdAt
            },
            new
            {
                Id = ManagerRoleId,
                Name = "Manager",
                Description = "Gerente con acceso a reportes y gestión de ventas",
                IsActive = true,
                CreatedAt = createdAt
            },
            new
            {
                Id = SellerRoleId,
                Name = "Seller",
                Description = "Vendedor con acceso a productos y registro de ventas",
                IsActive = true,
                CreatedAt = createdAt
            },
            new
            {
                Id = ViewerRoleId,
                Name = "Viewer",
                Description = "Usuario con acceso de solo lectura",
                IsActive = true,
                CreatedAt = createdAt
            },
            new
            {
                Id = GuestRoleId,
                Name = "Guest",
                Description = "Invitado con acceso limitado",
                IsActive = true,
                CreatedAt = createdAt
            }
        );
    }
}
