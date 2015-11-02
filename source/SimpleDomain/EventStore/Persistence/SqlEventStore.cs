//-------------------------------------------------------------------------------
// <copyright file="SqlEventStore.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Data.SqlClient;
    
    public class SqlEventStore : IEventStore
    {
        public const string ConnectionFactory = "ConnectionFactory";

        private const string EventStoreConnectionStringName = "EventStore";

        private readonly IHaveEventStoreConfiguration configuration;

        /// <summary>
        /// Creates a new instance of <see cref="SqlEventStore"/>
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveEventStoreConfiguration"/></param>
        public SqlEventStore(IHaveEventStoreConfiguration configuration)
        {
            this.configuration = configuration;

            this.CreateEventStoreTableIfNeeded();
            this.CreateSnapshotTableIfNeeded();
        }

        private DbConnectionFactory Factory
        {
            get { return this.configuration.Get<DbConnectionFactory>(ConnectionFactory); }
        }

        /// <inheritdoc />
        public IEventStream OpenStream<T>(Guid aggregateId) where T : IEventSourcedAggregateRoot
        {
            return new SqlEventStream<T>(
                aggregateId, 
                this.configuration.DispatchEvents,
                this.Factory, 
                EventStoreConnectionStringName);
        }

        private void CreateEventStoreTableIfNeeded()
        {
            using (var connection = this.Factory.Create(EventStoreConnectionStringName))
            using (var command = new SqlCommand(SqlCommands.CreateEventsTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void CreateSnapshotTableIfNeeded()
        {
            using (var connection = this.Factory.Create(EventStoreConnectionStringName))
            using (var command = new SqlCommand(SqlCommands.CreateSnapshotsTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}