//-------------------------------------------------------------------------------
// <copyright file="GetEventStoreStream.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::EventStore.ClientAPI;

    /// <summary>
    /// The GetEventStore event stream
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
    public class GetEventStoreStream<TAggregateRoot> : EventStream<TAggregateRoot> where TAggregateRoot : IEventSourcedAggregateRoot
    {
        private const int MaxItemCount = 4095;

        private readonly Func<Task<IEventStoreConnection>> createConnectionAsync;
        private IEventStoreConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEventStoreStream{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="dispatchAsync">The action to dispatch an event asynchronously</param>
        /// <param name="createConnectionAsync">The action to create a GetEventStore connection</param>
        public GetEventStoreStream(Guid aggregateId, Func<IEvent, Task> dispatchAsync, Func<Task<IEventStoreConnection>> createConnectionAsync)
            : base(aggregateId, dispatchAsync)
        {
            this.createConnectionAsync = createConnectionAsync;
        }

        /// <inheritdoc />
        public override async Task<IEventStream> OpenAsync()
        {
            this.connection = await this.createConnectionAsync().ConfigureAwait(false);
            return this;
        }

        /// <inheritdoc />
        public override async Task SaveSnapshotAsync(ISnapshot snapshot)
        {
            var streamName = GetSnapshotStreamName(this.AggregateId);
            var serializedSnapshot = snapshot.Serialize();

            await this.connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, serializedSnapshot).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<bool> HasSnapshotAsync()
        {
            var streamName = GetSnapshotStreamName(this.AggregateId);
            var snapshotSlice = await this.connection
                .ReadStreamEventsForwardAsync(streamName, 0, MaxItemCount, false)
                .ConfigureAwait(false);

            return snapshotSlice.Status != SliceReadStatus.StreamNotFound;
        }

        /// <inheritdoc />
        public override async Task<ISnapshot> GetLatestSnapshotAsync()
        {
            var streamName = GetSnapshotStreamName(this.AggregateId);
            var snapshotSlice = await this.connection
                .ReadStreamEventsForwardAsync(streamName, 0, MaxItemCount, false)
                .ConfigureAwait(false);

            return snapshotSlice.Events.Last().AsSnapshot();
        }

        /// <inheritdoc />
        protected override async Task<EventHistory> ReplayAsync(int fromVersion, int toVersion)
        {
            var streamName = GetStreamName(this.AggregateId);
            var eventsSlice = await this.connection
                .ReadStreamEventsForwardAsync(streamName, fromVersion, MaxItemCount, false)
                .ConfigureAwait(false);

            if (eventsSlice.Status == SliceReadStatus.StreamNotFound)
            {
                throw new AggregateRootNotFoundException(typeof(TAggregateRoot), this.AggregateId);
            }

            return eventsSlice.Events.AsEventHistory();
        }

        /// <inheritdoc />
        protected override async Task SaveAsync(VersionableEvent versionableEvent, IDictionary<string, object> headers)
        {
            var streamName = GetStreamName(this.AggregateId);
            var expectedVersion = versionableEvent.Version == 0 ? ExpectedVersion.NoStream : versionableEvent.Version - 1;
            var serializedEvent = versionableEvent.SerializeInnerEvent(headers);

            await this.connection.AppendToStreamAsync(streamName, expectedVersion, serializedEvent).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this.connection.Dispose();
        }

        private static string GetStreamName(Guid aggregateId)
        {
            return $"{typeof(TAggregateRoot).Name}-{aggregateId}";
        }

        private static string GetSnapshotStreamName(Guid aggregateId)
        {
            return $"{typeof(TAggregateRoot).Name}-{aggregateId}-Snapshot";
        }
    }
}