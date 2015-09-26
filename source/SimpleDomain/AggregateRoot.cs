//-------------------------------------------------------------------------------
// <copyright file="AggregateRoot.cs" company="frokonet.ch">
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
    using System.Collections.Generic;

    /// <summary>
    /// The base class for all aggregate roots
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly List<IEvent> uncommittedEvents = new List<IEvent>();

        /// <inheritdoc />
        public IEnumerable<IEvent> UncommittedEvents => this.uncommittedEvents;

        /// <inheritdoc />
        public void CommitEvents()
        {
            this.uncommittedEvents.Clear();
        }

        /// <summary>
        /// Applies an event tot he aggregate root
        /// </summary>
        /// <param name="event">The event</param>
        protected virtual void ApplyEvent(IEvent @event)
        {
            this.uncommittedEvents.Add(@event);
        }
    }
}