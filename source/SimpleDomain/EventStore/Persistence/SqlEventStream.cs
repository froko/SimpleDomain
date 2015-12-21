//-------------------------------------------------------------------------------
// <copyright file="SqlEventStream.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    /// <summary>
    /// The SQL event stream
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root</typeparam>
    public class SqlEventStream<TAggregate> : EventStream<TAggregate>
        where TAggregate : IEventSourcedAggregateRoot
    {
        private readonly DbConnectionFactory factory;
        private readonly string connectionStringName;

        /// <summary>
        /// Creates a new instance of <see cref="SqlEventStream{TAggregate}"/>
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="dispatchAsync">The action to dispatch an event asynchronously</param>
        /// <param name="factory">Dependency injection for <see cref="DbConnectionFactory"/></param>
        /// <param name="connectionStringName"></param>
        public SqlEventStream(Guid aggregateId, Func<IEvent, Task> dispatchAsync, DbConnectionFactory factory, string connectionStringName)
            : base(aggregateId, dispatchAsync)
        {
            this.factory = factory;
            this.connectionStringName = connectionStringName;
        }

        /// <inheritdoc />
        public override async Task SaveSnapshotAsync(ISnapshot snapshot)
        {
            var snapshotDescriptor = new SqlSnapshotDescriptor(this.AggregateType, this.AggregateId, snapshot);

            using (var connection = await this.factory.CreateAsync(this.connectionStringName).ConfigureAwait(false))
            using (var command = new SqlCommand(SqlCommands.InsertSnapshot, connection))
            {
                command.AddParameter("@AggregateType", snapshotDescriptor.AggregateType);
                command.AddParameter("@AggregateId", snapshotDescriptor.AggregateId);
                command.AddParameter("@Version", snapshotDescriptor.Version);
                command.AddParameter("@Timestamp", snapshotDescriptor.Timestamp);
                command.AddParameter("@SnapshotType", snapshotDescriptor.SnapshotType);
                command.AddParameter("@SnapshotData", snapshotDescriptor.SerializedSnapshot);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override async Task<bool> HasSnapshotAsync()
        {
            using (var connection = await this.factory.CreateAsync(this.connectionStringName).ConfigureAwait(false))
            using (var command = new SqlCommand(SqlCommands.GetSnapshotCount, connection))
            {
                command.AddParameter("@AggregateType", this.AggregateType);
                command.AddParameter("@AggregateId", this.AggregateId);

                var resultFromDb = await command.ExecuteScalarAsync().ConfigureAwait(false);
                var snapshotCount = (int)resultFromDb;

                return snapshotCount != 0;
            }
        }

        /// <inheritdoc />
        public override async Task<ISnapshot> GetLatestSnapshotAsync()
        {
            using (var connection = await this.factory.CreateAsync(this.connectionStringName).ConfigureAwait(false))
            using (var command = new SqlCommand(SqlCommands.GetLatestSnapshot, connection))
            {
                command.AddParameter("@AggregateType", this.AggregateType);
                command.AddParameter("@AggregateId", this.AggregateId);

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (reader.Read())
                    {
                        return reader.GetSnapshot();
                    }
                }

                return null;
            }
        }

        /// <inheritdoc />
        protected override async Task SaveAsync(VersionableEvent versionableEvent, IDictionary<string, object> headers)
        {
            var eventDescriptor = new SqlEventDescriptor(this.AggregateType, this.AggregateId, versionableEvent, headers);

            using (var connection = await this.factory.CreateAsync(this.connectionStringName).ConfigureAwait(false))
            using (var command = new SqlCommand(SqlCommands.InsertEvent, connection))
            {
                command.AddParameter("@AggregateType", eventDescriptor.AggregateType);
                command.AddParameter("@AggregateId", eventDescriptor.AggregateId);
                command.AddParameter("@Version", eventDescriptor.Version);
                command.AddParameter("@Timestamp", eventDescriptor.Timestamp);
                command.AddParameter("@EventType", eventDescriptor.EventType);
                command.AddParameter("@EventData", eventDescriptor.SerializedEvent);
                command.AddParameter("@Headers", eventDescriptor.SerializedHeaders);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        protected override async Task<EventHistory> ReplayAsync(int fromVersion, int toVersion)
        {
            using (var connection = await this.factory.CreateAsync(this.connectionStringName).ConfigureAwait(false))
            using (var command = new SqlCommand(SqlCommands.GetEventsByVersion, connection))
            {
                command.AddParameter("@AggregateType", this.AggregateType);
                command.AddParameter("@AggregateId", this.AggregateId);
                command.AddParameter("@VersionFrom", fromVersion);
                command.AddParameter("@VersionTo", toVersion);

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    return new EventHistory(ReadEvents(reader));
                }
            }
        }

        private static IEnumerable<IEvent> ReadEvents(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return reader.GetEvent();
            }
        }
    }
}