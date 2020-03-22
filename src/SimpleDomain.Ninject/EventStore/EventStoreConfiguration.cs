//-------------------------------------------------------------------------------
// <copyright file="EventStoreConfiguration.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace SimpleDomain.EventStore
{
    using System;
    using System.Threading.Tasks;

    using Ninject;

    using SimpleDomain.EventStore.Configuration;

    /// <summary>
    /// The EventStore configuration
    /// </summary>
    public class EventStoreConfiguration : AbstractIoCContainerEventStoreConfiguration
    {
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreConfiguration"/> class.
        /// </summary>
        /// <param name="kernel">Dependency injection for <see cref="IKernel"/></param>
        public EventStoreConfiguration(IKernel kernel)
        {
            this.kernel = kernel;
            this.DefineAsyncEventDispatching<IDeliverMessages>((bus, @event) => bus.PublishAsync(@event));
        }

        /// <inheritdoc />
        public override void DefineAsyncEventDispatching<TBus>(Func<TBus, IEvent, Task> dispatchEventsUsingResolvedBus)
        {
            this.DispatchEvents = @event => dispatchEventsUsingResolvedBus(this.kernel.Get<TBus>(), @event);
        }

        /// <inheritdoc />
        public override void Register(Func<IHaveEventStoreConfiguration, IEventStore> createEventStore)
        {
            this.kernel.Bind<IEventStore>().ToConstant(createEventStore(this));
            this.kernel.Bind<IEventSourcedRepository>().To<EventStoreRepository>().InTransientScope();
        }
    }
}