//-------------------------------------------------------------------------------
// <copyright file="InMemoryEventStore.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The InMemory event store
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly Func<IEvent, Task> dispatchAsync;
        
        private readonly IList<EventDescriptor> eventDescriptors;
        private readonly IList<SnapshotDescriptor> snapshotDescriptors;

        /// <summary>
        /// Creates a new instance of <see cref="InMemoryEventStore"/>
        /// </summary>
        /// <param name="dispatchAsync">The action to dispatch an event asynchronously</param>
        public InMemoryEventStore(Func<IEvent, Task> dispatchAsync)
        {
            this.dispatchAsync = dispatchAsync;
            
            this.eventDescriptors = new List<EventDescriptor>();
            this.snapshotDescriptors = new List<SnapshotDescriptor>();
        }

        /// <inheritdoc />
        public IEventStream OpenStream<T>(Guid aggregateId) where T : IEventSourcedAggregateRoot
        {
            return new InMemoryEventStream<T>(
                aggregateId,
                this.dispatchAsync,
                this.eventDescriptors,
                this.snapshotDescriptors);
        }
    }
}