//-------------------------------------------------------------------------------
// <copyright file="EventDataBuilder.cs" company="frokonet.ch">
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

    using global::EventStore.ClientAPI;

    /// <summary>
    /// The event data builder
    /// </summary>
    public class EventDataBuilder
    {
        private readonly IEvent @event;
        private readonly Guid eventId;
        private readonly string eventName;
        private readonly IDictionary<string, object> eventHeaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDataBuilder"/> class with an enclosed event.
        /// </summary>
        /// <param name="event">The event</param>
        private EventDataBuilder(IEvent @event)
        {
            this.@event = @event;
            this.eventId = Guid.NewGuid();
            this.eventName = @event.GetType().Name;
            this.eventHeaders = new Dictionary<string, object>
            {
                {
                    Conventions.EventClrTypeHeader,
                    @event.GetType().AssemblyQualifiedName
                }
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="EventDataBuilder"/> with an enclosed event
        /// </summary>
        /// <param name="event">The event</param>
        /// <returns>An instance of the <see cref="EventDataBuilder"/></returns>
        public static EventDataBuilder Initialize(IEvent @event)
        {
            return new EventDataBuilder(@event);
        }

        /// <summary>
        /// Adds a key/value pair to the header dictionary
        /// </summary>
        /// <param name="key">The key of the header</param>
        /// <param name="value">The value of the header</param>
        /// <returns>The event data builder itself</returns>
        public EventDataBuilder AddHeader(string key, object value)
        {
            this.eventHeaders.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds a list of headers to the header dictionary
        /// </summary>
        /// <param name="headers">The list of headers</param>
        /// <returns>The event data builder itself</returns>
        public EventDataBuilder AddHeaders(IDictionary<string, object> headers)
        {
            foreach (var header in headers.Where(header => !this.eventHeaders.Contains(header)))
            {
                this.eventHeaders.Add(header);
            }

            return this;
        }

        /// <summary>
        /// Builds and returns an instance of <see cref="EventData"/>
        /// </summary>
        /// <returns>An instance of <see cref="EventData"/></returns>
        public EventData Build()
        {
            return new EventData(
                this.eventId,
                this.eventName,
                true,
                this.@event.AsByteArray(),
                this.eventHeaders.AsByteArray());
        }
    }
}