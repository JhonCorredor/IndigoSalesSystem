namespace Core.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Sube un archivo de imagen al servicio de almacenamiento.
    /// </summary>
    /// <param name="fileName">Nombre único del archivo con su extensión.</param>
    /// <param name="fileStream">Flujo de datos (Stream) del archivo físico.</param>
    /// <returns>La URL pública donde quedó alojada la imagen.</returns>
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
}