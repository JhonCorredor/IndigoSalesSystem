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

        // Generamos un nombre único para el archivo
        var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";

        // Guardamos el nombre del archivo "subido"
        _uploadedFiles.Add(uniqueFileName);

        logger.LogInformation("Subida MOCK exitosa. Archivo: {FileName}", uniqueFileName);

        // Retornamos solo el nombre del archivo (no una URL)
        return uniqueFileName;
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

    public Task<string> GetImageSasUrlAsync(string fileName, int expirationMinutes = 60)
    {
        logger.LogInformation("Generando SAS URL MOCK para: {FileName} (expiración: {Minutes} min)", fileName, expirationMinutes);

        if (_uploadedFiles.Contains(fileName))
        {
            // Simulamos una URL con SAS token
            var sasToken = $"sv=2023-01-03&st={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}&se={DateTime.UtcNow.AddMinutes(expirationMinutes):yyyy-MM-ddTHH:mm:ssZ}&sr=b&sp=r&sig=MOCK-SIGNATURE";
            var mockSasUrl = $"https://indigomockstorage.blob.core.windows.net/product-images/{fileName}?{sasToken}";

            logger.LogInformation("SAS URL MOCK generada: {Url}", mockSasUrl);
            return Task.FromResult(mockSasUrl);
        }

        logger.LogWarning("Archivo MOCK no encontrado para SAS: {FileName}", fileName);
        throw new FileNotFoundException($"La imagen '{fileName}' no existe en el almacenamiento MOCK.");
    }

    public Task<Stream> DownloadImageAsync(string fileName)
    {
        logger.LogInformation("Descargando imagen MOCK: {FileName}", fileName);

        if (_uploadedFiles.Contains(fileName))
        {
            // Generamos contenido de imagen simulado (1x1 pixel PNG transparente)
            var mockImageBytes = Convert.FromBase64String(
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            logger.LogInformation("Descarga MOCK exitosa: {FileName}", fileName);
            return Task.FromResult<Stream>(new MemoryStream(mockImageBytes));
        }

        logger.LogWarning("Archivo MOCK no encontrado para descarga: {FileName}", fileName);
        throw new FileNotFoundException($"La imagen '{fileName}' no existe en el almacenamiento MOCK.");
    }
}