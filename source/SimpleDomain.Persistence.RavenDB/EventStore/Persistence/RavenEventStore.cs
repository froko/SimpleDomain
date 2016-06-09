//-------------------------------------------------------------------------------
// <copyright file="RavenEventStore.cs" company="frokonet.ch">
//   Copyright (c) 2014-2016
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Raven.Client;
    
    /// <summary>
    /// The RavenDB event store
    /// </summary>
    public class RavenEventStore : IEventStore
    {
        /// <summary>
        /// Gets the configuration key for the Raven DocumentStore
        /// </summary>
        public const string DocumentStore = "DocumentStore";

        private readonly IHaveEventStoreConfiguration configuration;

        /// <summary>
        /// Creates a new instance of <see cref="RavenEventStore"/>
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveEventStoreConfiguration"/></param>
        public RavenEventStore(IHaveEventStoreConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc />
        public IEventStream OpenStream<T>(Guid aggregateId) where T : IEventSourcedAggregateRoot
        {
            return new RavenEventStream<T>(
                aggregateId, 
                this.configuration.DispatchEvents,
                this.configuration.Get<IDocumentStore>(DocumentStore).OpenAsyncSession());
        }

        /// <inheritdoc />
        public async Task ReplayAllAsync()
        {
            using (var session = this.configuration.Get<IDocumentStore>(DocumentStore).OpenAsyncSession())
            {
                var events = await session
                    .Query<EventDescriptor>(EventStoreIndexes.EventDescriptorsByTimestamp)
                    .GetAllEventsAsync()
                    .ConfigureAwait(false);

                await Task.WhenAll(events.ToList().Select(this.configuration.DispatchEvents)).ConfigureAwait(false);
            }
        }
    }
}