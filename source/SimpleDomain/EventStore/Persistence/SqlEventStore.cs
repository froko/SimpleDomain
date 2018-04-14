//-------------------------------------------------------------------------------
// <copyright file="SqlEventStore.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The SQL event store
    /// </summary>
    public class SqlEventStore : IEventStore
    {
        /// <summary>
        /// Gets the connection factory configuration key
        /// </summary>
        public const string ConnectionFactory = "ConnectionFactory";

        /// <summary>
        /// Gets the eventstore connection string name
        /// </summary>
        private const string EventStoreConnectionStringName = "EventStore";

        private readonly IHaveEventStoreConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEventStore"/> class.
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveEventStoreConfiguration"/></param>
        public SqlEventStore(IHaveEventStoreConfiguration configuration)
        {
            this.configuration = configuration;

            this.CreateEventStoreTableIfNeeded();
            this.CreateSnapshotTableIfNeeded();
        }

        private DbConnectionFactory Factory => this.configuration.Get<DbConnectionFactory>(ConnectionFactory);

        /// <inheritdoc />
        public Task<IEventStream> OpenStreamAsync<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            var eventStream = new SqlEventStream<TAggregateRoot>(
                aggregateId,
                this.configuration.DispatchEvents,
                () => this.Factory.CreateAsync(EventStoreConnectionStringName));

            return eventStream.OpenAsync();
        }

        /// <inheritdoc />
        public async Task ReplayAllAsync()
        {
            const int BatchSize = 1000;

            var numberOfEvents = await this.GetNumberOfEventsAsync().ConfigureAwait(false);
            for (var lowerBound = 0; lowerBound < numberOfEvents; lowerBound += BatchSize)
            {
                var upperBound = lowerBound + BatchSize;
                await this.ReplayBatchAsync(lowerBound, upperBound).ConfigureAwait(false);
            }
        }

        private static IEnumerable<IEvent> ReadEvents(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return reader.GetEvent();
            }
        }

        private async Task<int> GetNumberOfEventsAsync()
        {
            using (var connection = await this.Factory.CreateAsync(EventStoreConnectionStringName).ConfigureAwait(false))
            using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo.Events", connection))
            {
                return (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
            }
        }

        private async Task ReplayBatchAsync(int lowerBound, int upperBound)
        {
            using (var connection = await this.Factory.CreateAsync(EventStoreConnectionStringName).ConfigureAwait(false))
            using (var command = new SqlCommand(SqlCommands.GetAllEvents, connection))
            {
                command.AddParameter("@LowerBound", lowerBound);
                command.AddParameter("@UpperBound", upperBound);

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    await Task
                        .WhenAll(ReadEvents(reader).ToList().Select(this.configuration.DispatchEvents))
                        .ConfigureAwait(false);
                }
            }
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