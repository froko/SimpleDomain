//-------------------------------------------------------------------------------
// <copyright file="SnapshotDescriptors_ByAggregateIdAndVersion.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.RavenIndexes
{
    using System.Linq;
    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;

    /// <summary>
    /// Defines a RavenDB index for snapshot descriptors by aggregate id and version
    /// </summary>
    public class SnapshotDescriptors_ByAggregateIdAndVersion : AbstractIndexCreationTask<SnapshotDescriptor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SnapshotDescriptors_ByAggregateIdAndVersion"/>
        /// </summary>
        public SnapshotDescriptors_ByAggregateIdAndVersion()
        {
            this.Map = snapshotDescriptors => 
                from snapshotDescriptor in snapshotDescriptors
                select new
                {
                    snapshotDescriptor.AggregateId, snapshotDescriptor.Version
                };

            this.Index(s => s.AggregateId, FieldIndexing.Analyzed);
            this.Index(s => s.Version, FieldIndexing.Analyzed);
        }
    }
}