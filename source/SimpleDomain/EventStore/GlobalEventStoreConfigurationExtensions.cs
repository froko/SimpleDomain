//-------------------------------------------------------------------------------
// <copyright file="GlobalEventStoreConfigurationExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System.Collections.Generic;

    using SimpleDomain.EventStore.Persistence;

    /// <summary>
    /// Configuration extensions for the EventStore configuration base class
    /// </summary>
    public static class GlobalEventStoreConfigurationExtensions
    {
        /// <summary>
        /// Registers the InMemory EventStore
        /// </summary>
        /// <param name="configuration">The event store configuration</param>
        public static void UseInMemoryEventStore(this IConfigureThisEventStore configuration)
        {
            configuration.AddConfigurationItem(InMemoryEventStore.EventDescriptors, new List<EventDescriptor>());
            configuration.AddConfigurationItem(InMemoryEventStore.SnapshotDescriptors, new List<SnapshotDescriptor>());
            configuration.Register(config => new InMemoryEventStore(config));
        }

        /// <summary>
        /// Registers the SQL EventStore
        /// </summary>
        /// <param name="configuration">The event store configuration</param>
        public static void UseSqlEventStore(this IConfigureThisEventStore configuration)
        {
            configuration.AddConfigurationItem(SqlEventStore.ConnectionFactory, new DbConnectionFactory());
            configuration.Register(config => new SqlEventStore(config));
        }
    }
}