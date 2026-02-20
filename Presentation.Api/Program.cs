using System.Text;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Utilities.Extensions;
using Shared.Utilities.Mappers;
using Shared.Utilities.Mappers.Implementations;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registro de Mappers
builder.Services.AddScoped<IMapper<Product, ProductDto>, ProductMapper>();
builder.Services.AddScoped<IMapper<Sale, SaleDto>, SaleMapper>();
builder.Services.AddScoped<IMapper<User, UserDto>, UserMapper>();
builder.Services.AddScoped<IMapper<Role, RoleDto>, RoleMapper>();

// 3. Registro de Repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// 4. Registro de Servicios
builder.Services.AddScoped<IProductService, Core.Application.Services.ProductService>();
builder.Services.AddScoped<ISaleService, Core.Application.Services.SaleService>();

// 5. Configuración de File Storage (Azure Blob o Mock)
var blobConnectionString = builder.Configuration.GetConnectionString("BlobStorage");

if (!string.IsNullOrEmpty(blobConnectionString))
{
    // Producción: Usa Azure Blob Storage
    builder.Services.AddSingleton<IFileStorageService>(sp =>
        new Infrastructure.Services.AzureBlobStorageService(blobConnectionString, "product-images"));
}
else
{
    // Desarrollo: Usa Mock (sin Azure)
    builder.Services.AddScoped<IFileStorageService, Infrastructure.Services.MockBlobStorageService>();
}

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Configuración de Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar serialización de fechas en formato ISO 8601
        options.JsonSerializerOptions.WriteIndented = false;
    });

builder.Services.AddEndpointsApiExplorer();

// 3. Configurar Swagger para que acepte el Token JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IndiGO Sales API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa 'Bearer' [espacio] y luego tu token. Ejemplo: 'Bearer eyJhb...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware global de manejo de excepciones
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANTE: UseCors debe ir ANTES de UseAuthentication/UseAuthorization
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();