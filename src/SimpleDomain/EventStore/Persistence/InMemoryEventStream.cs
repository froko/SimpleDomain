//-------------------------------------------------------------------------------
// <copyright file="InMemoryEventStream.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The InMemory event stream
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
    public class InMemoryEventStream<TAggregateRoot> : EventStream<TAggregateRoot> where TAggregateRoot : IEventSourcedAggregateRoot
    {
        private readonly IList<EventDescriptor> eventDescriptors;
        private readonly IList<SnapshotDescriptor> snapshotDescriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEventStream{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="dispatchAsync">The action to dispatch an event asynchronously</param>
        /// <param name="eventDescriptors">A list of event descriptors as persistence backend</param>
        /// <param name="snapshotDescriptors">A list of snapshot descriptors as persistence backend</param>
        public InMemoryEventStream(
            Guid aggregateId,
            Func<IEvent, Task> dispatchAsync,
            IList<EventDescriptor> eventDescriptors,
            IList<SnapshotDescriptor> snapshotDescriptors) : base(aggregateId, dispatchAsync)
        {
            this.eventDescriptors = eventDescriptors;
            this.snapshotDescriptors = snapshotDescriptors;
        }

        /// <inheritdoc />
        public override Task<IEventStream> OpenAsync()
        {
            return Task.FromResult((IEventStream)this);
        }

        /// <inheritdoc />
        public override Task SaveSnapshotAsync(ISnapshot snapshot)
        {
            this.snapshotDescriptors.Add(new SnapshotDescriptor(this.AggregateType, this.AggregateId, snapshot));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task<bool> HasSnapshotAsync()
        {
            var hasSnapshot = this.snapshotDescriptors.Any(s => s.AggregateType == this.AggregateType && s.AggregateId == this.AggregateId);
            return Task.FromResult(hasSnapshot);
        }

        /// <inheritdoc />
        public override Task<ISnapshot> GetLatestSnapshotAsync()
        {
            var latestSnapshot = this.snapshotDescriptors.Last(s => s.AggregateType == this.AggregateType && s.AggregateId == this.AggregateId).Snapshot;
            return Task.FromResult(latestSnapshot);
        }

        /// <inheritdoc />
        protected override Task SaveAsync(VersionableEvent versionableEvent, IDictionary<string, object> headers)
        {
            this.eventDescriptors.Add(new EventDescriptor(this.AggregateType, this.AggregateId, versionableEvent, headers));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task<EventHistory> ReplayAsync(int fromVersion, int toVersion)
        {
            var events = this.eventDescriptors
                .Where(e => e.AggregateType == this.AggregateType && e.AggregateId == this.AggregateId && e.Version >= fromVersion && e.Version <= toVersion)
                .OrderBy(e => e.Version)
                .Select(e => e.Event);

            return Task.FromResult(new EventHistory(events));
        }
    }
}