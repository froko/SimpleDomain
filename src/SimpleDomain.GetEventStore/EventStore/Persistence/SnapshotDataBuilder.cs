//-------------------------------------------------------------------------------
// <copyright file="SnapshotDataBuilder.cs" company="frokonet.ch">
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

    using global::EventStore.ClientAPI;

    /// <summary>
    /// The snapshot data builder
    /// </summary>
    public class SnapshotDataBuilder
    {
        private readonly ISnapshot snapshot;
        private readonly Guid snapshotId;
        private readonly string snapshotName;
        private readonly IDictionary<string, object> snapshotHeaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotDataBuilder"/> class.
        /// </summary>
        /// <param name="snapshot">The snapshot</param>
        private SnapshotDataBuilder(ISnapshot snapshot)
        {
            this.snapshot = snapshot;
            this.snapshotId = Guid.NewGuid();
            this.snapshotName = snapshot.GetType().Name;
            this.snapshotHeaders = new Dictionary<string, object>
            {
                {
                    Conventions.EventClrTypeHeader,
                    snapshot.GetType().AssemblyQualifiedName
                }
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="SnapshotDataBuilder"/> with an enclosed snapshot
        /// </summary>
        /// <param name="snapshot">The snapshot</param>
        /// <returns>An instance fo the <see cref="SnapshotDataBuilder"/></returns>
        public static SnapshotDataBuilder Initialize(ISnapshot snapshot)
        {
            return new SnapshotDataBuilder(snapshot);
        }

        /// <summary>
        /// Builds and returns an instance of <see cref="EventData"/>
        /// </summary>
        /// <returns>An instance of <see cref="EventData"/></returns>
        public EventData Build()
        {
            return new EventData(
                this.snapshotId,
                this.snapshotName,
                true,
                this.snapshot.AsByteArray(),
                this.snapshotHeaders.AsByteArray());
        }
    }
}