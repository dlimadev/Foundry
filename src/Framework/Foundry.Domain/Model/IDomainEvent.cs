using MediatR;
namespace Foundry.Domain.Model
{
    /// <summary>
    /// A marker interface for a domain event, inheriting from MediatR's INotification.
    /// </summary>
    public interface IDomainEvent : INotification { }
}