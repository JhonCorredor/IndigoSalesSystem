namespace Shared.Utilities.Mappers;

/// <summary>
/// Extension methods for mapping collections.
/// </summary>
public static class MapperExtensions
{
    /// <summary>
    /// Maps a collection of source objects to destination objects.
    /// </summary>
    public static IEnumerable<TDestination> MapAll<TSource, TDestination>(
        this IMapper<TSource, TDestination> mapper,
        IEnumerable<TSource> sources)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(sources);

        return sources.Select(mapper.Map);
    }

    /// <summary>
    /// Maps a collection to a list.
    /// </summary>
    public static List<TDestination> MapToList<TSource, TDestination>(
        this IMapper<TSource, TDestination> mapper,
        IEnumerable<TSource> sources)
    {
        return mapper.MapAll(sources).ToList();
    }

    /// <summary>
    /// Maps a collection to a read-only list.
    /// </summary>
    public static IReadOnlyList<TDestination> MapToReadOnlyList<TSource, TDestination>(
        this IMapper<TSource, TDestination> mapper,
        IEnumerable<TSource> sources)
    {
        return mapper.MapAll(sources).ToList().AsReadOnly();
    }
}
