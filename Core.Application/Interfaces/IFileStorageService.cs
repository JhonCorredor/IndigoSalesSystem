namespace Core.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Sube un archivo de imagen al servicio de almacenamiento.
    /// </summary>
    /// <param name="fileName">Nombre único del archivo con su extensión.</param>
    /// <param name="fileStream">Flujo de datos (Stream) del archivo físico.</param>
    /// <returns>El nombre del archivo almacenado.</returns>
    Task<string> UploadImageAsync(string fileName, Stream fileStream);

    /// <summary>
    /// Elimina un archivo de imagen del servicio de almacenamiento.
    /// </summary>
    /// <param name="fileName">Nombre del archivo a eliminar.</param>
    /// <returns>Un booleano que indica si la eliminación fue exitosa.</returns>
    Task<bool> DeleteImageAsync(string fileName);

    /// <summary>
    /// Obtiene la URL pública de una imagen almacenada.
    /// </summary>
    /// <param name="fileName">Nombre del archivo cuya URL se desea obtener.</param>
    /// <returns>La URL pública de la imagen.</returns>
    Task<string> GetImageUrlAsync(string fileName);

    /// <summary>
    /// Genera una URL temporal con SAS (Shared Access Signature) para acceder a una imagen privada.
    /// </summary>
    /// <param name="fileName">Nombre del archivo.</param>
    /// <param name="expirationMinutes">Tiempo de validez del SAS en minutos. Por defecto 60 minutos.</param>
    /// <returns>URL firmada con SAS token que permite acceso temporal de lectura.</returns>
    Task<string> GetImageSasUrlAsync(string fileName, int expirationMinutes = 60);

    /// <summary>
    /// Descarga el contenido de una imagen como Stream.
    /// </summary>
    /// <param name="fileName">Nombre del archivo a descargar.</param>
    /// <returns>Stream con el contenido de la imagen.</returns>
    Task<Stream> DownloadImageAsync(string fileName);
}