namespace Foundry.Domain.Services
{
    /// <summary>
    /// Defines a contract for a service that provides information about the current user.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the unique identifier of the current user.
        /// </summary>
        string? UserId { get; }
    }
}