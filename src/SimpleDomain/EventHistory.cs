//-------------------------------------------------------------------------------
// <copyright file="EventHistory.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Represents the history of events used to bring
    /// an Aggregate Root in its desired state
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Name is intended")]
    public class EventHistory : IEnumerable<IEvent>
    {
        private readonly IReadOnlyCollection<IEvent> events;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHistory"/> class.
        /// </summary>
        /// <param name="events">All events that have been applied to the Aggregate Root in the past</param>
        public EventHistory(IEnumerable<IEvent> events)
        {
            this.events = new List<IEvent>(events);
        }

        /// <summary>
        /// Gets a value indicating whether no events have been found/loaded
        /// </summary>
        public bool IsEmpty => !this.events.Any();

        /// <summary>
        /// Factory method to create a new instance of <see cref="EventHistory"/>
        /// </summary>
        /// <param name="events">All events that have been applied to the Aggregate Root in the past</param>
        /// <returns>A new instance of <see cref="EventHistory"/></returns>
        public static EventHistory Create(params IEvent[] events)
        {
            return new EventHistory(events);
        }

        /// <inheritdoc />
        public IEnumerator<IEvent> GetEnumerator()
        {
            return this.events.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.events.GetEnumerator();
        }
    }
}