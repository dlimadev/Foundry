namespace Foundry.Application.Abstractions.Services
{
    public class ResponseMetaProvider : IResponseMetaProvider
    {
        private readonly Dictionary<string, object> _metadata = new();
        public void Add(string key, object value) => _metadata[key] = value;
        public IReadOnlyDictionary<string, object> GetMetadata() => _metadata;
    }
}