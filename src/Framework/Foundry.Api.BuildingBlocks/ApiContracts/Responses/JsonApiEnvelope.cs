using System.Text.Json.Serialization;

namespace Foundry.Api.BuildingBlocks.ApiContracts.Responses
{
    /// <summary>
    /// A generic response envelope that follows the JSON:API specification structure.
    /// All successful API responses should use this wrapper for consistency.
    /// </summary>
    public class JsonApiEnvelope<T>
    {
        /// <summary>
        /// The response's primary data.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        /// <summary>
        /// A list of error objects. Should only be populated in case of failure.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<JsonApiError>? Errors { get; set; }

        public JsonApiEnvelope() { }

        /// <summary>
        /// Creates a success envelope containing the data.
        /// </summary>
        public static JsonApiEnvelope<T> Success(T data) => new() { Data = data };

        /// <summary>
        /// Creates an error envelope containing a list of errors.
        /// Note: The generic type is object to allow a common error shape.
        /// </summary>
        public static JsonApiEnvelope<object> Error(IEnumerable<JsonApiError> errors) => new() { Errors = errors };
    }
}