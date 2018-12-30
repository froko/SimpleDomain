//-------------------------------------------------------------------------------
// <copyright file="SqlEventDescriptor.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// This class describes an event enriched with further attributes
    /// and serializes the event itself as well as the headers
    /// </summary>
    public class SqlEventDescriptor : EventDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEventDescriptor"/> class.
        /// </summary>
        public SqlEventDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEventDescriptor"/> class.
        /// </summary>
        /// <param name="aggregateType">The full CLR name of the aggregate root</param>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="versionableEvent">The versionable event</param>
        /// <param name="headers">A list of arbitrary headers</param>
        public SqlEventDescriptor(string aggregateType, Guid aggregateId, VersionableEvent versionableEvent, IDictionary<string, object> headers)
            : base(aggregateType, aggregateId, versionableEvent, headers)
        {
            this.SerializedEvent = JsonConvert.SerializeObject(versionableEvent.InnerEvent);
            this.SerializedHeaders = JsonConvert.SerializeObject(headers);
        }

        /// <summary>
        /// Gets or sets the serialized event
        /// </summary>
        public string SerializedEvent { get; set; }

        /// <summary>
        /// Gets or sets the serialized headers
        /// </summary>
        public string SerializedHeaders { get; set; }
    }
}