//-------------------------------------------------------------------------------
// <copyright file="EventStoreFactory.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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
    using System.Collections.Generic;

    using SimpleDomain.EventStore;
    using SimpleDomain.EventStore.Persistence;

    /// <summary>
    /// The event store factory
    /// </summary>
    public class EventStoreFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreFactory"/> class.
        /// </summary>
        public EventStoreFactory()
        {
            this.Create = (config, bus) =>
            {
                config.DefineAsyncEventDispatching(bus.PublishAsync);
                config.AddConfigurationItem(InMemoryEventStore.EventDescriptors, new List<EventDescriptor>());
                config.AddConfigurationItem(InMemoryEventStore.SnapshotDescriptors, new List<SnapshotDescriptor>());
                return new InMemoryEventStore(config);
            };
        }

        /// <summary>
        /// Gets the function to create an event store using a configuration and an instance of a Jitney bus
        /// </summary>
        public Func<AbstractEventStoreConfiguration, IDeliverMessages, IEventStore> Create { get; private set; }

        /// <summary>
        /// Registers the function to create an event store using a configuration
        /// </summary>
        /// <param name="create">A function to create the event store with a given configuration</param>
        public void Register(Func<IHaveEventStoreConfiguration, IEventStore> create)
        {
            this.Create = (config, bus) =>
            {
                config.DefineAsyncEventDispatching(bus.PublishAsync);
                return create(config);
            };
        }
    }
}