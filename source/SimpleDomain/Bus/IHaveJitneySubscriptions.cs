namespace SimpleDomain.Bus
{
    using System.Collections.Generic;

    public interface IHaveJitneySubscriptions
    {
        Subscription GetCommandSubscription<TCommand>(TCommand command) where TCommand : ICommand;

        IEnumerable<Subscription> GetEventSubscriptions<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}