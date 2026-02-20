using Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class MockBlobStorageService(ILogger<MockBlobStorageService> logger) : IFileStorageService
{
    private readonly HashSet<string> _uploadedFiles = [];

    public async Task<string> UploadImageAsync(string fileName, Stream fileStream)
    {
        logger.LogInformation("Iniciando subida simulada (MOCK) a Blob Storage para el archivo: {FileName}", fileName);

        // Simulamos la latencia de red de subir un archivo a la nube (ej. 500ms)
        await Task.Delay(500);

        // Generamos un nombre único para la URL pública falsa
        var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";
        var mockUrl = $"https://indigomockstorage.blob.core.windows.net/product-images/{uniqueFileName}";

        // Guardamos el nombre del archivo "subido"
        _uploadedFiles.Add(uniqueFileName);

        logger.LogInformation("Subida MOCK exitosa. URL generada: {Url}", mockUrl);

        return mockUrl;
    }

    public Task<bool> DeleteImageAsync(string fileName)
    {
        logger.LogInformation("Eliminando imagen MOCK: {FileName}", fileName);

        var removed = _uploadedFiles.Remove(fileName);

        logger.LogInformation("Eliminación MOCK {Status}: {FileName}", 
            removed ? "exitosa" : "fallida (no encontrada)", fileName);

        return Task.FromResult(removed);
    }

    public Task<string> GetImageUrlAsync(string fileName)
    {
        logger.LogInformation("Obteniendo URL MOCK para: {FileName}", fileName);

        if (_uploadedFiles.Contains(fileName))
        {
            var mockUrl = $"https://indigomockstorage.blob.core.windows.net/product-images/{fileName}";
            logger.LogInformation("URL MOCK encontrada: {Url}", mockUrl);
            return Task.FromResult(mockUrl);
        }

        logger.LogWarning("Archivo MOCK no encontrado: {FileName}", fileName);
        throw new FileNotFoundException($"La imagen '{fileName}' no existe en el almacenamiento MOCK.");
    }
}