namespace Shared.Utilities.TimeZone;

/// <summary>
/// Servicio para manejo de zona horaria de Colombia (UTC-5).
/// Las fechas se almacenan en UTC en la base de datos y se convierten a hora de Colombia para la API.
/// </summary>
public static class ColombiaTimeZone
{
    // Colombia está en UTC-5 (America/Bogota) sin cambios de horario de verano
    private static readonly TimeZoneInfo _colombiaTimeZone = 
        TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); // UTC-5 para Windows
    
    /// <summary>
    /// Obtiene la zona horaria de Colombia.
    /// </summary>
    public static TimeZoneInfo TimeZone => _colombiaTimeZone;

    /// <summary>
    /// Convierte una fecha UTC a hora de Colombia.
    /// </summary>
    public static DateTime ConvertFromUtc(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            // Si no está marcada como UTC, asumimos que lo es
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _colombiaTimeZone);
    }

    /// <summary>
    /// Convierte una fecha de Colombia a UTC.
    /// </summary>
    public static DateTime ConvertToUtc(DateTime colombiaDateTime)
    {
        // Si ya es UTC, devolver tal cual
        if (colombiaDateTime.Kind == DateTimeKind.Utc)
        {
            return colombiaDateTime;
        }

        // Si no tiene Kind especificado, asumimos que es hora de Colombia
        if (colombiaDateTime.Kind == DateTimeKind.Unspecified)
        {
            colombiaDateTime = DateTime.SpecifyKind(colombiaDateTime, DateTimeKind.Unspecified);
        }

        return TimeZoneInfo.ConvertTimeToUtc(colombiaDateTime, _colombiaTimeZone);
    }

    /// <summary>
    /// Obtiene la fecha/hora actual en Colombia.
    /// </summary>
    public static DateTime Now => ConvertFromUtc(DateTime.UtcNow);

    /// <summary>
    /// Obtiene solo la fecha actual en Colombia (sin hora).
    /// </summary>
    public static DateTime Today => Now.Date;

    /// <summary>
    /// Obtiene la hora UTC actual.
    /// </summary>
    public static DateTime UtcNow => DateTime.UtcNow;
}
