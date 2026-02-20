using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Seed data configuration for User entity.
/// Passwords are hashed using BCrypt.
/// </summary>
public class UserSeedConfiguration : IEntityTypeConfiguration<User>
{
    // Pre-generated BCrypt hash for "admin123"
    // Generated with: BCrypt.Net.BCrypt.HashPassword("admin123")
    private const string DefaultPasswordHash = "$2a$10$8xQxK5ZxZnA3zQzL7Nz8UeVc3nG5QjJ4X7yD6WqS8FvPzB9tR5mNy";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Username = "admin",
                Email = "admin@indigosales.com",
                PasswordHash = DefaultPasswordHash,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                CreatedAt = createdAt,
                RoleId = RoleSeedConfiguration.AdminRoleId
            },
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Username = "manager",
                Email = "manager@indigosales.com",
                PasswordHash = DefaultPasswordHash,
                FirstName = "John",
                LastName = "Manager",
                IsActive = true,
                CreatedAt = createdAt,
                RoleId = RoleSeedConfiguration.ManagerRoleId
            },
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                Username = "seller1",
                Email = "seller1@indigosales.com",
                PasswordHash = DefaultPasswordHash,
                FirstName = "Maria",
                LastName = "Garcia",
                IsActive = true,
                CreatedAt = createdAt,
                RoleId = RoleSeedConfiguration.SellerRoleId
            },
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                Username = "seller2",
                Email = "seller2@indigosales.com",
                PasswordHash = DefaultPasswordHash,
                FirstName = "Carlos",
                LastName = "Rodriguez",
                IsActive = true,
                CreatedAt = createdAt,
                RoleId = RoleSeedConfiguration.SellerRoleId
            },
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                Username = "viewer",
                Email = "viewer@indigosales.com",
                PasswordHash = DefaultPasswordHash,
                FirstName = "Ana",
                LastName = "Martinez",
                IsActive = true,
                CreatedAt = createdAt,
                RoleId = RoleSeedConfiguration.ViewerRoleId
            },
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000006"),
                Username = "guest",
                Email = "guest@indigosales.com",
                PasswordHash = DefaultPasswordHash,
                FirstName = "Guest",
                LastName = "User",
                IsActive = true,
                CreatedAt = createdAt,
                RoleId = RoleSeedConfiguration.GuestRoleId
            }
        );
    }
}
