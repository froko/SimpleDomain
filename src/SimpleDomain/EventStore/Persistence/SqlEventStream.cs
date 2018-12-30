//-------------------------------------------------------------------------------
// <copyright file="SqlEventStream.cs" company="frokonet.ch">
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
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    /// <summary>
    /// The SQL event stream
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
    public class SqlEventStream<TAggregateRoot> : EventStream<TAggregateRoot>
        where TAggregateRoot : IEventSourcedAggregateRoot
    {
        private readonly Func<Task<SqlConnection>> createConnectionAsync;
        private SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEventStream{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate root</param>
        /// <param name="dispatchAsync">The action to dispatch an event asynchronously</param>
        /// <param name="createConnectionAsync">The action to create a SQL connection</param>
        public SqlEventStream(Guid aggregateId, Func<IEvent, Task> dispatchAsync, Func<Task<SqlConnection>> createConnectionAsync)
            : base(aggregateId, dispatchAsync)
        {
            this.createConnectionAsync = createConnectionAsync;
        }

        /// <inheritdoc />
        public override async Task<IEventStream> OpenAsync()
        {
            this.connection = await this.createConnectionAsync().ConfigureAwait(false);
            return this;
        }

        /// <inheritdoc />
        public override async Task SaveSnapshotAsync(ISnapshot snapshot)
        {
            var snapshotDescriptor = new SqlSnapshotDescriptor(this.AggregateType, this.AggregateId, snapshot);

            using (var command = new SqlCommand(SqlCommands.InsertSnapshot, this.connection))
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
            using (var command = new SqlCommand(SqlCommands.GetSnapshotCount, this.connection))
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
            using (var command = new SqlCommand(SqlCommands.GetLatestSnapshot, this.connection))
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

            using (var command = new SqlCommand(SqlCommands.InsertEvent, this.connection))
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
            using (var command = new SqlCommand(SqlCommands.GetEventsByVersion, this.connection))
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

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this.connection.Dispose();
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