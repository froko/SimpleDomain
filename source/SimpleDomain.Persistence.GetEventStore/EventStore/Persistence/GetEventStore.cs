//-------------------------------------------------------------------------------
// <copyright file="GetEventStore.cs" company="frokonet.ch">
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

    using global::EventStore.ClientAPI;

    /// <summary>
    /// The GetEventStore event store
    /// </summary>
    public class GetEventStore : IEventStore
    {
        /// <summary>
        /// Gets the connection builder configuration key
        /// </summary>
        public const string ConnectionBuilder = "ConnectionBuilder";

        private readonly IHaveEventStoreConfiguration configuration;

        /// <summary>
        /// Creates a new instance of <see cref="GetEventStore"/>
        /// </summary>
        /// <param name="configuration">Dependency injeciton for <see cref="GetEventStore"/></param>
        public GetEventStore(IHaveEventStoreConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private IConnectionBuilder EventStoreConnectionBuilder
            => this.configuration.Get<IConnectionBuilder>(ConnectionBuilder);

        /// <inheritdoc />
        public Task<IEventStream> OpenStreamAsync<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            var eventStream = new GetEventStoreStream<TAggregateRoot>(
                aggregateId,
                this.configuration.DispatchEvents,
                () => this.EventStoreConnectionBuilder.BuildAsync());

            return eventStream.OpenAsync();
        }

        /// <inheritdoc />
        public async Task ReplayAllAsync()
        {
            using (var connection = await this.EventStoreConnectionBuilder.BuildAsync().ConfigureAwait(false))
            {
                await this.ReplayAllAsync(connection);
            }
        }

        private async Task ReplayAllAsync(IEventStoreConnection connection)
        {
            AllEventsSlice currentSlice;
            var nextSliceStart = Position.Start;
            do
            {
                currentSlice = await connection.ReadAllEventsForwardAsync(nextSliceStart, 1000, false).ConfigureAwait(false);
                nextSliceStart = currentSlice.NextPosition;
                
                await Task
                    .WhenAll(currentSlice.Events.Deserialize().Select(this.configuration.DispatchEvents))
                    .ConfigureAwait(false);
            }
            while (!currentSlice.IsEndOfStream);
        }
    }
}