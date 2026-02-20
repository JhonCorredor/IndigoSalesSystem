using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "ImageUrl", "IsActive", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1496181133206-80ce9b88a853", true, "Laptop HP Pavilion 15", 1299.99m, 25, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46", true, "Mouse Logitech MX Master 3", 99.99m, 50, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1511467687858-23d96c32e4ae", true, "Teclado Mecánico Corsair K70", 149.99m, 35, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf", true, "Monitor Samsung 27\" 4K", 449.99m, 15, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1505740420928-5e560c06d30e", true, "Audífonos Sony WH-1000XM4", 349.99m, 40, null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1587826080692-f439cd0b70da", true, "Webcam Logitech C920 HD", 79.99m, 60, null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1597872200969-2b65d56bd16b", true, "SSD Samsung 1TB NVMe", 129.99m, 45, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrador del sistema con acceso total", true, "Admin", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gerente con acceso a reportes y gestión de ventas", true, "Manager", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vendedor con acceso a productos y registro de ventas", true, "Seller", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Usuario con acceso de solo lectura", true, "Viewer", null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Invitado con acceso limitado", true, "Guest", null }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "CreatedAt", "Date", "IsActive", "Total", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Utc), true, 1449.98m, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 16, 14, 45, 0, 0, DateTimeKind.Utc), true, 599.98m, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 17, 9, 15, 0, 0, DateTimeKind.Utc), true, 429.98m, null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 18, 16, 20, 0, 0, DateTimeKind.Utc), true, 1579.97m, null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 19, 11, 0, 0, 0, DateTimeKind.Utc), true, 899.97m, null }
                });

            migrationBuilder.InsertData(
                table: "SaleItems",
                columns: new[] { "Id", "ProductId", "Quantity", "SaleId", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 1299.99m },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 99.99m },
                    { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("33333333-3333-3333-3333-333333333333"), 1, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 149.99m },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), new Guid("44444444-4444-4444-4444-444444444444"), 1, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 449.99m },
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("55555555-5555-5555-5555-555555555555"), 1, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 349.99m },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("66666666-6666-6666-6666-666666666666"), 1, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 79.99m },
                    { new Guid("d1111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), 1, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 1299.99m },
                    { new Guid("d2222222-2222-2222-2222-222222222222"), new Guid("77777777-7777-7777-7777-777777777777"), 1, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 129.99m },
                    { new Guid("d3333333-3333-3333-3333-333333333333"), new Guid("33333333-3333-3333-3333-333333333333"), 1, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 149.99m },
                    { new Guid("e1111111-1111-1111-1111-111111111111"), new Guid("44444444-4444-4444-4444-444444444444"), 2, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 449.99m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastLoginAt", "LastName", "PasswordHash", "RoleId", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@indigosales.com", "System", true, null, "Administrator", "$2a$10$8xQxK5ZxZnA3zQzL7Nz8UeVc3nG5QjJ4X7yD6WqS8FvPzB9tR5mNy", new Guid("10000000-0000-0000-0000-000000000001"), null, "admin" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager@indigosales.com", "John", true, null, "Manager", "$2a$10$8xQxK5ZxZnA3zQzL7Nz8UeVc3nG5QjJ4X7yD6WqS8FvPzB9tR5mNy", new Guid("10000000-0000-0000-0000-000000000002"), null, "manager" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seller1@indigosales.com", "Maria", true, null, "Garcia", "$2a$10$8xQxK5ZxZnA3zQzL7Nz8UeVc3nG5QjJ4X7yD6WqS8FvPzB9tR5mNy", new Guid("10000000-0000-0000-0000-000000000003"), null, "seller1" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seller2@indigosales.com", "Carlos", true, null, "Rodriguez", "$2a$10$8xQxK5ZxZnA3zQzL7Nz8UeVc3nG5QjJ4X7yD6WqS8FvPzB9tR5mNy", new Guid("10000000-0000-0000-0000-000000000003"), null, "seller2" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "viewer@indigosales.com", "Ana", true, null, "Martinez", "$2a$10$8xQxK5ZxZnA3zQzL7Nz8UeVc3nG5QjJ4X7yD6WqS8FvPzB9tR5mNy", new Guid("10000000-0000-0000-0000-000000000004"), null, "viewer" },
                    { new Guid("20000000-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "guest@indigosales.com", "Guest", true, null, "User", "$2a$10$8xQxK5ZxZnA3zQzL7Nz8UeVc3nG5QjJ4X7yD6WqS8FvPzB9tR5mNy", new Guid("10000000-0000-0000-0000-000000000005"), null, "guest" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId",
                table: "SaleItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
