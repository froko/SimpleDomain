//-------------------------------------------------------------------------------
// <copyright file="SnapshotDescriptor.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System;

    /// <summary>
    /// This class describes a snapshot enriched with further attributes
    /// </summary>
    public class SnapshotDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotDescriptor"/> class.
        /// </summary>
        public SnapshotDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotDescriptor"/> class.
        /// </summary>
        /// <param name="aggregateType">The full CLR name of the aggregate root</param>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="snapshot">The snapshot</param>
        public SnapshotDescriptor(string aggregateType, Guid aggregateId, ISnapshot snapshot)
        {
            this.AggregateType = aggregateType;
            this.AggregateId = aggregateId;
            this.Version = snapshot.Version;
            this.Timestamp = DateTime.Now;
            this.SnapshotType = snapshot.GetType().FullName;
            this.Snapshot = snapshot;
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
        /// Gets or sets the actual version of the aggregate root
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the snapshot
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the full CLR name of the snapshot
        /// </summary>
        public string SnapshotType { get; set; }

        /// <summary>
        /// Gets or sets the snapshot itself
        /// </summary>
        public ISnapshot Snapshot { get; set; }
    }
}