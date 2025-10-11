namespace Foundry.Api.BuildingBlocks.Localization
{
    /// <summary>
    /// Defines a contract for a service that provides localized strings.
    /// </summary>
    public interface ILocalizerService
    {
        /// <summary>
        /// Gets the localized string for a given key, formatting it with the provided arguments.
        /// </summary>
        /// <param name="key">The key of the string to retrieve (e.g., "orders.notFound").</param>
        /// <param name="args">The arguments to format the string with.</param>
        /// <returns>The localized and formatted string.</returns>
        string GetLocalizedString(string key, params object[] args);
    }
}