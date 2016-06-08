//-------------------------------------------------------------------------------
// <copyright file="EventStoreConfigurationExtensions.cs" company="frokonet.ch">
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
    using Raven.Client;
    using Raven.Client.Document;

    using SimpleDomain.EventStore.Configuration;
    using SimpleDomain.EventStore.Persistence;

    /// <summary>
    /// Configuration extensions for the EventStore configuration base class
    /// </summary>
    public static class EventStoreConfigurationExtensions
    {
        /// <summary>
        /// Registers the RavenDB EventStore
        /// </summary>
        /// <param name="configuration">The abstract EventStore configuration</param>
        public static void UseRavenEventStore(this AbstractEventStoreConfiguration configuration)
        {
            configuration.UseRavenEventStore(CreateDocumentStore());
        }

        /// <summary>
        /// Registers the RavenDB EventStore
        /// </summary>
        /// <param name="configuration">The abstract EventStore configuration</param>
        /// <param name="documentStore">The RavenDB document store</param>
        public static void UseRavenEventStore(
            this AbstractEventStoreConfiguration configuration,
            IDocumentStore documentStore)
        {
            DocumentStoreSetup.CreateIndexes(documentStore);
            DocumentStoreSetup.RegisterIdConventions(documentStore);

            configuration.AddConfigurationItem(RavenEventStore.DocumentStore, documentStore);
            configuration.Register(config => new RavenEventStore(config));
        }

        private static IDocumentStore CreateDocumentStore()
        {
            var documentStore = new DocumentStore { ConnectionStringName = "EventStore" };
            documentStore.Initialize();

            return documentStore;
        }
    }
}