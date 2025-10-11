namespace Foundry.Domain.Model
{
    /// <summary>
    /// Defines the contract for a snapshot, which is a serialized,
    /// point-in-time representation of an event-sourced aggregate's state.
    /// The name makes its purpose explicit to the Event Sourcing strategy.
    /// </summary>
    public interface IEventSourcedSnapshot
    {
        /// <summary>
        /// The Id of the aggregate this snapshot belongs to.
        /// </summary>
        Guid AggregateId { get; }

        /// <summary>
        /// The version of the aggregate when this snapshot was taken.
        /// </summary>
        int Version { get; }
    }
}