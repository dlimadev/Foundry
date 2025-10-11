namespace Foundry.Application.Abstractions.Services
{
    /// <summary>
    /// A scoped service that collects metadata during a request's lifecycle,
    /// to be included in the final API response, typically as headers.
    /// </summary>
    public interface IResponseMetaProvider
    {
        void Add(string key, object value);
        IReadOnlyDictionary<string, object> GetMetadata();
    }
}