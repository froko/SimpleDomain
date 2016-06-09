//-------------------------------------------------------------------------------
// <copyright file="DocumentStoreSetup.cs" company="frokonet.ch">
//   Copyright (c) 2014-2016
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
    using Raven.Abstractions.Util;
    using Raven.Client;
    using Raven.Client.Indexes;

    using SimpleDomain.EventStore.RavenIndexes;

    /// <summary>
    /// Static helper class to set up the RavenDB document store
    /// </summary>
    public static class DocumentStoreSetup
    {
        /// <summary>
        /// Creates or updates indexes on the document store
        /// </summary>
        /// <param name="documentStore">The document store</param>
        public static void CreateIndexes(IDocumentStore documentStore)
        {
            IndexCreation.CreateIndexes(typeof(EventDescriptors_ByAggregateIdAndVersion).Assembly, documentStore);
        }

        /// <summary>
        /// Registers Id generation conventions for EventDescriptors and SnapshotDescriptors
        /// in the document store
        /// </summary>
        /// <param name="documentStore">The document store</param>
        public static void RegisterIdConventions(IDocumentStore documentStore)
        {
            documentStore.Conventions.RegisterAsyncIdConvention<EventDescriptor>((dbname, commands, eventdescriptor) 
                => new CompletedTask<string>($"EventDescriptors/{eventdescriptor.AggregateId}/{eventdescriptor.Version}"));
            
            documentStore.Conventions.RegisterAsyncIdConvention<SnapshotDescriptor>((dbname, commands, snapshotdescriptor) 
                => new CompletedTask<string>($"SnapshotDescriptors/{snapshotdescriptor.AggregateId}/{snapshotdescriptor.Version}"));
        }
    }
}