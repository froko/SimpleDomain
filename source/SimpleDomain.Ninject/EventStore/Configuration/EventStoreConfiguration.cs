//-------------------------------------------------------------------------------
// <copyright file="EventStoreConfiguration.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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

namespace SimpleDomain.EventStore.Configuration
{
    using System;
    using System.Threading.Tasks;

    using Ninject;

    /// <summary>
    /// The EventStore configuration
    /// </summary>
    public class EventStoreConfiguration : AbstractEventStoreConfiguration
    {
        private readonly IKernel kernel;
        
        /// <summary>
        /// Creates a new instance of <see cref="EventStoreConfiguration"/>
        /// </summary>
        /// <param name="kernel">Dependency injection for <see cref="IKernel"/></param>
        public EventStoreConfiguration(IKernel kernel)
        {
            this.kernel = kernel;
            this.kernel.Bind<IHaveEventStoreConfiguration>().ToConstant(this).InSingletonScope();
            this.kernel.Bind<IEventSourcedRepository>().To<EventStoreRepository>().InTransientScope();

            this.DispatchEvents = @event => Task.CompletedTask;
        }

        /// <summary>
        /// Defines the action how to resolve a bus and asynchronously publish events over this bus
        /// </summary>
        /// <param name="dispatchEventsUsingResolvedBus">The async resolve and publish action</param>
        public void DefineAsyncEventDispatching<TBus>(Func<TBus, IEvent, Task> dispatchEventsUsingResolvedBus)
        {
            this.DispatchEvents = @event => dispatchEventsUsingResolvedBus(this.kernel.Get<TBus>(), @event);
        }

        /// <inheritdoc />
        public override void Register<TEventStore>()
        {
            this.kernel.Bind<IEventStore>().To<TEventStore>().InSingletonScope();
        }
    }
}