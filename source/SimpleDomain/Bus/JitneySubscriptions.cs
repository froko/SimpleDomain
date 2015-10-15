namespace SimpleDomain.Bus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    using SimpleDomain.Common;

    public class JitneySubscriptions : IHaveJitneySubscriptions
    {
        private readonly IResolveTypes typeResolver;
        private readonly IList<Subscription> commandSubscriptions;
        private readonly IList<Subscription> eventSubscriptions;

        public JitneySubscriptions(IResolveTypes typeResolver)
        {
            this.typeResolver = typeResolver;
            
            this.commandSubscriptions = new List<Subscription>();
            this.eventSubscriptions = new List<Subscription>();
        }

        public void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
        {
            Guard.NotNull(() => handler);

            if (this.commandSubscriptions.Any(s => s.CanHandle<TCommand>()))
            {
                throw new CommandSubscriptionException<TCommand>();
            }

            this.commandSubscriptions.Add(new CommandSubscription<TCommand>(handler));
        }

        public void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Guard.NotNull(() => handler);

            this.eventSubscriptions.Add(new EventSubscription<TEvent>(handler));
        }

        public virtual Subscription GetCommandSubscription<TCommand>(TCommand command) where TCommand : ICommand
        {
            var subscription = this.commandSubscriptions.SingleOrDefault(s => s.CanHandle<TCommand>());
            if (subscription != null)
            {
                return subscription;
            }

            var handler = this.typeResolver.Resolve<IHandleAsync<TCommand>>();
            if (handler != null)
            {
                return new CommandSubscription<TCommand>(handler.HandleAsync);
            }

            throw new NoSubscriptionException(command);
        }

        public virtual IEnumerable<Subscription> GetEventSubscriptions<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var subscriptions = this.eventSubscriptions.Where(s => s.CanHandle<TEvent>());
            var handlers = this.typeResolver.ResolveAll<IHandleAsync<TEvent>>();

            return subscriptions.Union(handlers.Select(h => new EventSubscription<TEvent>(h.HandleAsync)));
        }
    }
}