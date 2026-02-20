using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Application.Interfaces;

namespace Infrastructure.Services;

public class AzureBlobStorageService(string connectionString, string containerName) : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient = new(connectionString);

    public async Task<string> UploadImageAsync(string fileName, Stream fileStream)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Aseguramos que el contenedor exista y sea de acceso público para lectura de imágenes
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

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

        // Retornamos la URL pública
        return blobClient.Uri.ToString();
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
}