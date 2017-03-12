//-------------------------------------------------------------------------------
// <copyright file="RavenEventStream.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Raven.Client;
    using Raven.Client.Linq;

    /// <summary>
    /// The RavenDB event stream
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
    public class RavenEventStream<TAggregateRoot> : EventStream<TAggregateRoot> where TAggregateRoot : IEventSourcedAggregateRoot
    {
        private readonly Func<Task<IAsyncDocumentSession>> createSessionAsync;
        private IAsyncDocumentSession session;

        /// <summary>
        /// Creates a new instance of <see cref="RavenEventStream{TAggregateRoot}"/>
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="dispatchAsync">The action to dispatch an event asynchronously</param>
        /// <param name="createSessionAsync">The action to create a session</param>
        public RavenEventStream(Guid aggregateId, Func<IEvent, Task> dispatchAsync, Func<Task<IAsyncDocumentSession>> createSessionAsync)
            : base(aggregateId, dispatchAsync)
        {
            this.createSessionAsync = createSessionAsync;
        }

        /// <inheritdoc />
        public override async Task<IEventStream> OpenAsync()
        {
            this.session = await this.createSessionAsync().ConfigureAwait(false);
            return this;
        }

        /// <inheritdoc />
        public override async Task SaveSnapshotAsync(ISnapshot snapshot)
        {
            var snapshotDescriptor = new SnapshotDescriptor(this.AggregateType, this.AggregateId, snapshot);

            await this.session.StoreAsync(snapshotDescriptor).ConfigureAwait(false);
            await this.session.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<bool> HasSnapshotAsync()
        {
            var snapshots = await this.GetSnapshotsAsync().ConfigureAwait(false);
            return snapshots.Any();
        }

        /// <inheritdoc />
        public override async Task<ISnapshot> GetLatestSnapshotAsync()
        {
            var snapshots = await this.GetSnapshotsAsync().ConfigureAwait(false);
            return snapshots.Last();
        }

        /// <inheritdoc />
        protected override async Task SaveAsync(VersionableEvent versionableEvent, IDictionary<string, object> headers)
        {
            var eventDescriptor = new EventDescriptor(this.AggregateType, this.AggregateId, versionableEvent, headers);

            await this.session.StoreAsync(eventDescriptor).ConfigureAwait(false);
            await this.session.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override async Task<EventHistory> ReplayAsync(int fromVersion, int toVersion)
        {
            var events = await this.session
                .Query<EventDescriptor>(EventStoreIndexes.EventDescriptorsByAggregateIdAndVersion)
                .Where(e => e.AggregateId == this.AggregateId && e.Version >= fromVersion && e.Version <= toVersion)
                .OrderBy(e => e.Version)
                .GetAllEventsAsync()
                .ConfigureAwait(false);

            return new EventHistory(events);
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this.session.Dispose();
        }

        private async Task<IList<ISnapshot>> GetSnapshotsAsync()
        {
            return await this.session
                .Query<SnapshotDescriptor>(EventStoreIndexes.SnapshotDescriptorsByAggregateIdAndVersion)
                .Where(s => s.AggregateId == this.AggregateId)
                .OrderBy(s => s.Version)
                .GetAllSnapshotsAsync()
                .ConfigureAwait(false);
        }
    }
}