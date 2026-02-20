using Core.Application.Interfaces;
using Core.Domain.Entities;

namespace Core.Application.Services;

public class ProductService(IProductRepository productRepository, IFileStorageService fileStorageService) : IProductService
{
    public async Task<Product> CreateProductAsync(string name, decimal price, int stock, Stream? imageStream, string? fileName)
    {
        string? imageUrl = null;

        // 1. Lógica de subida de archivo
        if (imageStream is not null && !string.IsNullOrWhiteSpace(fileName))
        {
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            imageUrl = await fileStorageService.UploadImageAsync(uniqueFileName, imageStream);
        }

        // 2. Creación de la entidad de Dominio
        var product = new Product(Guid.NewGuid(), name, price, stock, imageUrl);

        // 3. Persistencia
        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();

        return product;
    }
}