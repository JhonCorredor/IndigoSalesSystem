namespace Core.Application.Interfaces;

public interface IProductService
{
    // Usamos un Stream para no acoplar la capa de aplicación a tecnologías web (IFormFile)
    Task<Core.Domain.Entities.Product> CreateProductAsync(string name, decimal price, int stock, Stream? imageStream, string? fileName);
}