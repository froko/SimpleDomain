//-------------------------------------------------------------------------------
// <copyright file="AbstractEventStoreConfigurationExtensions.cs" company="frokonet.ch">
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
    using System.Collections.Generic;

    using SimpleDomain.EventStore.Persistence;

    /// <summary>
    /// Configuration extensions for the EventStore configuration base class
    /// </summary>
    public static class AbstractEventStoreConfigurationExtensions
    {
        /// <summary>
        /// Prepares the use of the InMemory EventStore without registering the EventStore itself
        /// </summary>
        public static void PrepareInMemoryEventStore(this AbstractEventStoreConfiguration configuration)
        {
            configuration.AddConfigurationItem(InMemoryEventStore.EventDescriptors, new List<EventDescriptor>());
            configuration.AddConfigurationItem(InMemoryEventStore.SnapshotDescriptors, new List<SnapshotDescriptor>());
        }

        /// <summary>
        /// Uses the InMemory EventStore
        /// </summary>
        public static void UseInMemoryEventStore(this AbstractEventStoreConfiguration configuration)
        {
            configuration.PrepareInMemoryEventStore();
            configuration.Register<InMemoryEventStore>();
        }

        /// <summary>
        /// Prepares the use of the SQL EventStore without registering the EventStore itself
        /// </summary>
        public static void PrepareSqlEventStore(this AbstractEventStoreConfiguration configuration)
        {
            configuration.AddConfigurationItem(SqlEventStore.ConnectionFactory, new DbConnectionFactory());
        }

        /// <summary>
        /// Uses the SQL EventStore
        /// </summary>
        public static void UseSqlEventStore(this AbstractEventStoreConfiguration configuration)
        {
            configuration.PrepareSqlEventStore();
            configuration.Register<SqlEventStore>();
        }
    }
}