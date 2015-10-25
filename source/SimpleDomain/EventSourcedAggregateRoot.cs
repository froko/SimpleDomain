//-------------------------------------------------------------------------------
// <copyright file="EventSourcedAggregateRoot.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System;

    /// <summary>
    /// Base class for all event sourced aggregate roots
    /// </summary>
    public abstract class EventSourcedAggregateRoot : AggregateRoot, IEventSourcedAggregateRoot
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventSourcedAggregateRoot"/>
        /// </summary>
        protected EventSourcedAggregateRoot()
        {
            this.Version = -1;
        }

        /// <inheritdoc />
        public Guid Id { get; protected set; }

        /// <inheritdoc />
        public int Version { get; protected set; }
        
        /// <inheritdoc />
        public virtual ISnapshot CreateSnapshot()
        {
            return null;
        }

        /// <inheritdoc />
        public virtual void LoadFromSnapshot(ISnapshot snapshot)
        {
        }

        /// <inheritdoc />
        public void LoadFromEventHistory(EventHistory eventHistory)
        {
            foreach (var @event in eventHistory)
            {
                this.ApplyEvent(@event, false);
            }
        }

        /// <summary>
        /// Applies a change expressed as event
        /// </summary>
        /// <param name="event">The event</param>
        protected override void ApplyEvent(IEvent @event)
        {
            this.ApplyEvent(@event, true);
        }

        /// <summary>
        /// Executes a state transition on a derived aggregate root
        /// </summary>
        /// <param name="event">The event</param>
        protected abstract void DoTransition(IEvent @event);

        private void ApplyEvent(IEvent @event, bool isNew)
        {
            this.Version++;
            this.DoTransition(@event);

            if (!isNew)
            {
                return;
            }

            base.ApplyEvent(new VersionableEvent(@event).With(this.Version));
        }
    }
}