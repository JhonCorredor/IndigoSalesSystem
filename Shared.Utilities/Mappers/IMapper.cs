namespace Shared.Utilities.Mappers;

/// <summary>
/// Interface for mapping between source and destination types.
/// </summary>
/// <typeparam name="TSource">Source type.</typeparam>
/// <typeparam name="TDestination">Destination type.</typeparam>
public interface IMapper<in TSource, out TDestination>
{
    /// <summary>
    /// Maps a source object to a destination object.
    /// </summary>
    TDestination Map(TSource source);
}

/// <summary>
/// Interface for bi-directional mapping.
/// </summary>
/// <typeparam name="TSource">Source type.</typeparam>
/// <typeparam name="TDestination">Destination type.</typeparam>
public interface IBidirectionalMapper<TSource, TDestination>
    : IMapper<TSource, TDestination>
{
    /// <summary>
    /// Maps a destination object back to a source object.
    /// </summary>
    TSource MapBack(TDestination destination);
}
