namespace Foundry.Api.BuildingBlocks.ApiContracts.Responses
{
    /// <summary>
    /// Represents a single error object, compliant with the JSON:API specification.
    /// </summary>
    /// <param name="Status">The HTTP status code applicable to this problem.</param>
    /// <param name="Title">A short, human-readable summary of the problem (e.g., the error code).</param>
    /// <param name="Detail">A human-readable explanation specific to this occurrence of the problem.</param>
    public record JsonApiError(string Status, string Title, string Detail);
}