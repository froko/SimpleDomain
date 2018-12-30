//-------------------------------------------------------------------------------
// <copyright file="SqlSnapshotDescriptor.cs" company="frokonet.ch">
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

    using Newtonsoft.Json;

    /// <summary>
    /// This class describes a snapshot enriched with further attributes
    /// and serializes the snapshot itself
    /// </summary>
    public class SqlSnapshotDescriptor : SnapshotDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSnapshotDescriptor"/> class.
        /// </summary>
        public SqlSnapshotDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSnapshotDescriptor"/> class.
        /// </summary>
        /// <param name="aggregateType">The full CLR name of the aggregate root</param>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="snapshot">The snapshot</param>
        public SqlSnapshotDescriptor(string aggregateType, Guid aggregateId, ISnapshot snapshot) : base(aggregateType, aggregateId, snapshot)
        {
            this.SerializedSnapshot = JsonConvert.SerializeObject(snapshot);
        }

        /// <summary>
        /// Gets or sets the serialized snapshot
        /// </summary>
        public string SerializedSnapshot { get; set; }
    }
}