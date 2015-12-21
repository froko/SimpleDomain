//-------------------------------------------------------------------------------
// <copyright file="EventStream.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SimpleDomain.Common;

    /// <summary>
    /// The abstract base class of an event stream
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root</typeparam>
    public abstract class EventStream<TAggregate> : Disposable, IEventStream where TAggregate : IEventSourcedAggregateRoot
    {
        private readonly Func<IEvent, Task> dispatchAsync;

        /// <summary>
        /// Creates a new instance of <see cref="EventStream{TAggregate}"/>
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="dispatchAsync">The action to dispatch an event asynchronously</param>
        protected EventStream(Guid aggregateId, Func<IEvent, Task> dispatchAsync)
        {
            this.AggregateType = typeof(TAggregate).FullName;
            this.AggregateId = aggregateId;
            this.dispatchAsync = dispatchAsync;
        }

        /// <summary>
        /// Gets the full CLR name of the aggregate root
        /// </summary>
        protected string AggregateType { get; private set; }

        /// <summary>
        /// Gets the id of the aggregate root
        /// </summary>
        protected Guid AggregateId { get; private set; }

        /// <inheritdoc />
        public async Task SaveAsync(IEnumerable<VersionableEvent> events, int expectedVersion, IDictionary<string, object> headers)
        {
            var eventsToSave = events.ToList();
            var originalVersion = expectedVersion - eventsToSave.Count;

            this.CheckForConcurrencyProblems(originalVersion);

            foreach (var @event in eventsToSave)
            {
                await this.SaveAsync(@event, headers).ConfigureAwait(false);
                await this.dispatchAsync(@event.InnerEvent).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public abstract Task SaveSnapshotAsync(ISnapshot snapshot);

        /// <inheritdoc />
        public Task<EventHistory> ReplayAsync()
        {
            return this.ReplayAsync(0, int.MaxValue);
        }

        /// <inheritdoc />
        public Task<EventHistory> ReplayAsyncFromSnapshot(ISnapshot snapshot)
        {
            return this.ReplayAsync(snapshot.Version + 1, int.MaxValue);
        }

        /// <inheritdoc />
        public abstract Task<bool> HasSnapshotAsync();

        /// <inheritdoc />
        public abstract Task<ISnapshot> GetLatestSnapshotAsync();

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
        }

        /// <summary>
        /// Persists a single versionable event
        /// </summary>
        /// <param name="versionableEvent">The versionable event</param>
        /// <param name="headers">A list of arbitrary headers</param>
        protected abstract Task SaveAsync(VersionableEvent versionableEvent, IDictionary<string, object> headers);

        /// <summary>
        /// Replays all events of an aggregate root between two given version boundaries (inclusive)
        /// </summary>
        /// <param name="fromVersion">The lower version boundary (inclusive)</param>
        /// <param name="toVersion">The upper version boundary (inclusive)</param>
        /// <returns>A list of events</returns>
        protected abstract Task<EventHistory> ReplayAsync(int fromVersion, int toVersion);

        /// <summary>
        /// When overridden, checks for concurrency problems by comparing the last persisted version with the given original version
        /// </summary>
        /// <param name="originalVersion">The original version, calculated as expected version minus the number of events that are going to be persisted</param>
        protected virtual void CheckForConcurrencyProblems(int originalVersion)
        {
        }
    }
}