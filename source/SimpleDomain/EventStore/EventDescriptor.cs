//-------------------------------------------------------------------------------
// <copyright file="EventDescriptor.cs" company="frokonet.ch">
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

    /// <summary>
    /// This class describes an event enriched with further attributes
    /// </summary>
    public class EventDescriptor
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventDescriptor"/>
        /// </summary>
        public EventDescriptor()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="EventDescriptor"/>
        /// </summary>
        /// <param name="aggregateType">The full CLR name of the aggregate root</param>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="versionableEvent">The versionable event</param>
        /// <param name="headers">A list of arbitrary headers</param>
        public EventDescriptor(string aggregateType, Guid aggregateId, VersionableEvent versionableEvent, IDictionary<string, object> headers)
        {
            this.AggregateType = aggregateType;
            this.AggregateId = aggregateId;
            this.Version = versionableEvent.Version;
            this.Timestamp = DateTime.Now;
            this.EventType = versionableEvent.InnerEvent.GetFullName();
            this.Event = versionableEvent.InnerEvent;
            this.Headers = new Dictionary<string, object>(headers);
        }

        /// <summary>
        /// Gets or sets the full CLR name of the aggregate root
        /// </summary>
        public string AggregateType { get; set; }

        /// <summary>
        /// Gets or sets the id of the aggregate root
        /// </summary>
        public Guid AggregateId { get; set; }

        /// <summary>
        /// Gets or sets the actual version of the event
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the date and time of when the event took place
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the full CLR name of the event
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the event itself
        /// </summary>
        public IEvent Event { get; set; }

        /// <summary>
        /// Gets or sets a list of arbitrary headers
        /// </summary>
        public Dictionary<string, object> Headers { get; set; }
    }
}