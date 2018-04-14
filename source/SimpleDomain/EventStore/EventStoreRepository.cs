//-------------------------------------------------------------------------------
// <copyright file="EventStoreRepository.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The repository which uses the event store as persistence engine
    /// </summary>
    public class EventStoreRepository : IEventSourcedRepository
    {
        private readonly IEventStore eventStore;
        private readonly List<SnapshotStrategy> typedSnapshotStrategies;
        private SnapshotStrategy globalSnapshotStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreRepository"/> class.
        /// </summary>
        /// <param name="eventStore">Dependency injection for <see cref="IEventStore"/></param>
        public EventStoreRepository(IEventStore eventStore)
        {
            this.eventStore = eventStore;
            this.typedSnapshotStrategies = new List<SnapshotStrategy>();
            this.globalSnapshotStrategy = new SnapshotStrategy(100);
        }

        /// <summary>
        /// Registers a global snapshot strategy which applies to all aggregate roots
        /// </summary>
        /// <param name="threshold">The version threshold on which a snapshot is taken</param>
        /// <returns>The event repository itself</returns>
        public EventStoreRepository WithGlobalSnapshotStrategy(int threshold)
        {
            this.globalSnapshotStrategy = new SnapshotStrategy(threshold);
            return this;
        }

        /// <summary>
        /// Registers a typed snapshot strategy which applies to a certain type of aggregate roots
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
        /// <param name="threshold">The version threshold on which a snapshot is taken</param>
        /// <returns>The event repository itself</returns>
        public EventStoreRepository WithSnapshotStrategyFor<TAggregateRoot>(int threshold) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            this.typedSnapshotStrategies.Add(new SnapshotStrategy(threshold, typeof(TAggregateRoot)));
            return this;
        }

        /// <inheritdoc />
        public async Task<TAggregateRoot> GetByIdAsync<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            var aggregateRoot = Activator.CreateInstance<TAggregateRoot>();

            using (var eventStream = await this.eventStore.OpenStreamAsync<TAggregateRoot>(aggregateId).ConfigureAwait(false))
            {
                if (await eventStream.HasSnapshotAsync().ConfigureAwait(false))
                {
                    var snapshot = await eventStream
                        .GetLatestSnapshotAsync()
                        .ConfigureAwait(false);

                    aggregateRoot.LoadFromSnapshot(snapshot);

                    var eventHistory = await eventStream
                        .ReplayAsyncFromSnapshot(snapshot)
                        .ConfigureAwait(false);

                    aggregateRoot.LoadFromEventHistory(eventHistory);
                }
                else
                {
                    var eventHistory = await eventStream.ReplayAsync().ConfigureAwait(false);
                    if (eventHistory.IsEmpty)
                    {
                        throw new AggregateRootNotFoundException(typeof(TAggregateRoot), aggregateId);
                    }

                    aggregateRoot.LoadFromEventHistory(eventHistory);
                }
            }

            return aggregateRoot;
        }

        /// <inheritdoc />
        public Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            return this.SaveAsync(aggregateRoot, new Dictionary<string, object>());
        }

        /// <inheritdoc />
        public async Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot, IDictionary<string, object> headers) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            using (var eventStream = await this.eventStore.OpenStreamAsync<TAggregateRoot>(aggregateRoot.Id).ConfigureAwait(false))
            {
                await eventStream
                    .SaveAsync(aggregateRoot.UncommittedEvents.OfType<VersionableEvent>(), aggregateRoot.Version, headers)
                    .ConfigureAwait(false);

                aggregateRoot.CommitEvents();
            }

            await this.SaveSnapshotAsyncIfNeeded(aggregateRoot)
                .ConfigureAwait(false);
        }

        private Task SaveSnapshotAsyncIfNeeded<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            if (this.typedSnapshotStrategies.Any(s => s.AppliesToThisAggregateRoot<TAggregateRoot>()))
            {
                if (this.typedSnapshotStrategies.Any(s => s.NeedsSnapshot(aggregateRoot)))
                {
                    return this.SaveSnapshotAsync<TAggregateRoot>(aggregateRoot.Id, aggregateRoot.CreateSnapshot());
                }
            }
            else
            {
                if (this.globalSnapshotStrategy.NeedsSnapshot(aggregateRoot))
                {
                    return this.SaveSnapshotAsync<TAggregateRoot>(aggregateRoot.Id, aggregateRoot.CreateSnapshot());
                }
            }

            return Task.CompletedTask;
        }

        private async Task SaveSnapshotAsync<TAggregateRoot>(Guid aggregateId, ISnapshot snapshot) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            if (snapshot == null)
            {
                return;
            }

            using (var eventStream = await this.eventStore.OpenStreamAsync<TAggregateRoot>(aggregateId).ConfigureAwait(false))
            {
                await eventStream.SaveSnapshotAsync(snapshot).ConfigureAwait(false);
            }
        }
    }
}