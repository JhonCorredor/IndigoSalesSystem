using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Core.Application.Interfaces;

namespace Infrastructure.Services;

public class AzureBlobStorageService(string connectionString, string containerName) : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient = new(connectionString);

    public async Task<string> UploadImageAsync(string fileName, Stream fileStream)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Aseguramos que el contenedor exista en modo privado (sin acceso público)
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobClient = blobContainerClient.GetBlobClient(fileName);

        // Determinar ContentType dinámicamente basado en la extensión
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };

        // Subimos el archivo
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

        // Retornamos solo el nombre del archivo (no la URL pública)
        return fileName;
    }

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetImageUrlAsync(string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        // Verificar si el blob existe
        if (await blobClient.ExistsAsync())
        {
            return blobClient.Uri.ToString();
        }

        throw new FileNotFoundException($"La imagen '{fileName}' no existe en el almacenamiento.");
    }

    public async Task<string> GetImageSasUrlAsync(string fileName, int expirationMinutes = 60)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        // Verificar si el blob existe
        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"La imagen '{fileName}' no existe en el almacenamiento.");
        }

        // Generar SAS token con permisos de lectura
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = fileName,
            Resource = "b", // b = blob
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Margen de 5 min por diferencias de reloj
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
        };

        // Permiso de lectura
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Generar la URL firmada con SAS
        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }

    public async Task<Stream> DownloadImageAsync(string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        // Verificar si el blob existe
        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"La imagen '{fileName}' no existe en el almacenamiento.");
        }

        // Descargar el contenido del blob
        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }
}