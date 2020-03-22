namespace SimpleDomain.EventStore.Configuration
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The event store configuration base class for IoC containers
    /// </summary>
    public abstract class AbstractIoCContainerEventStoreConfiguration : AbstractEventStoreConfiguration
    {
        /// <summary>
        /// Defines the action how to resolve a bus and asynchronously publish events over this bus
        /// </summary>
        /// <typeparam name="TBus">The type of the bus</typeparam>
        /// <param name="dispatchEventsUsingResolvedBus">The async resolve and publish action</param>
        public abstract void DefineAsyncEventDispatching<TBus>(Func<TBus, IEvent, Task> dispatchEventsUsingResolvedBus);
    }
}