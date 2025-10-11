namespace Foundry.Application.Abstractions.Mappings
{
    /// <summary>
    /// A generic contract for a mapper that converts an object of type TSource to TDestination.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TDestination">The destination object type.</typeparam>
    public interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
    }
}