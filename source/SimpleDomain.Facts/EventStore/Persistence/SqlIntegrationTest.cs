//-------------------------------------------------------------------------------
// <copyright file="SqlIntegrationTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.EventStore.Configuration;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class SqlIntegrationTest
    {
        private readonly Guid aggregateId;
        private readonly IDeliverMessages bus;
        private readonly ContainerLessEventStoreConfiguration configuration;
        private readonly IEventStore testee;

        public SqlIntegrationTest()
        {
            var factory = new EventStoreFactory();

            this.aggregateId = Guid.NewGuid();
            this.bus = A.Fake<IDeliverMessages>();
            this.configuration = new ContainerLessEventStoreConfiguration(factory);
            this.configuration.UseSqlEventStore();

            this.testee = factory.Create(configuration, bus);
        }

        [Fact, WithTransaction]
        public async Task DispatchesEventsAfterSavingThem()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId);
            aggregateRoot.ChangeValue(0);
            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(22);

            using (var eventStream = await this.CreateEventStreamAsync().ConfigureAwait(false))
            {
                await eventStream.SaveAsync(
                    aggregateRoot.UncommittedEvents.OfType<VersionableEvent>(),
                    aggregateRoot.Version,
                    new Dictionary<string, object>()).ConfigureAwait(false);
            }

            A.CallTo(() => this.bus.PublishAsync<IEvent>(A<ValueEvent>.That.Matches(e => e.Value == 0))).MustHaveHappened();
            A.CallTo(() => this.bus.PublishAsync<IEvent>(A<ValueEvent>.That.Matches(e => e.Value == 11))).MustHaveHappened();
            A.CallTo(() => this.bus.PublishAsync<IEvent>(A<ValueEvent>.That.Matches(e => e.Value == 22))).MustHaveHappened();
        }

        [Fact, WithTransaction]
        public async Task CanSaveAndReplayAllEvents()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId);
            aggregateRoot.ChangeValue(0);
            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(22);

            using (var eventStream = await this.CreateEventStreamAsync().ConfigureAwait(false))
            {
                await eventStream.SaveAsync(
                    aggregateRoot.UncommittedEvents.OfType<VersionableEvent>(),
                    aggregateRoot.Version,
                    new Dictionary<string, object>()).ConfigureAwait(false);

                var eventHistory = await eventStream.ReplayAsync().ConfigureAwait(false);

                eventHistory.Should().HaveCount(3);
                eventHistory.Should().Contain(e => (e as ValueEvent).Value == 0);
                eventHistory.Should().Contain(e => (e as ValueEvent).Value == 11);
                eventHistory.Should().Contain(e => (e as ValueEvent).Value == 22);
            }
        }

        [Fact, WithTransaction]
        public async Task CanSaveAndReplayEventsSinceLatestSnapshot()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId);
            aggregateRoot.ChangeValue(0);
            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(22);

            await this.SaveEventsAsync(aggregateRoot).ConfigureAwait(false);

            var firstSnapshot = aggregateRoot.CreateSnapshot();

            await this.SaveSnapshotAsync(firstSnapshot).ConfigureAwait(false);

            aggregateRoot.ChangeValue(33);
            aggregateRoot.ChangeValue(44);

            await this.SaveEventsAsync(aggregateRoot).ConfigureAwait(false);

            var secondSnapshot = aggregateRoot.CreateSnapshot();

            await this.SaveSnapshotAsync(secondSnapshot).ConfigureAwait(false);

            aggregateRoot.ChangeValue(55);
            aggregateRoot.ChangeValue(66);
            aggregateRoot.ChangeValue(77);

            await this.SaveEventsAsync(aggregateRoot).ConfigureAwait(false);

            using (var eventStream = await this.CreateEventStreamAsync().ConfigureAwait(false))
            {
                var hasSnapshot = await eventStream.HasSnapshotAsync().ConfigureAwait(false);
                var snapshotFromEventStore = await eventStream.GetLatestSnapshotAsync().ConfigureAwait(false);

                hasSnapshot.Should().BeTrue();
                snapshotFromEventStore.Version.Should().Be(secondSnapshot.Version);

                var eventHistorySinceLatestSnapshot = await eventStream
                    .ReplayAsyncFromSnapshot(snapshotFromEventStore)
                    .ConfigureAwait(false);

                eventHistorySinceLatestSnapshot.Should().HaveCount(3);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 55);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 66);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 77);
            }
        }

        [Fact, WithTransaction]
        public async Task CanReplayAllEvents()
        {
            const int NumberOfEvents = 1024;

            await this.CreateTestEventsAsync(NumberOfEvents);

            await this.testee.ReplayAllAsync().ConfigureAwait(false);

            A.CallTo(() => this.bus.PublishAsync(A<IEvent>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(NumberOfEvents));
        }

        private Task<IEventStream> CreateEventStreamAsync()
        {
            return this.testee.OpenStreamAsync<MyDynamicEventSourcedAggregateRoot>(this.aggregateId);
        }

        private async Task SaveEventsAsync(IEventSourcedAggregateRoot aggregateRoot)
        {
            using (var eventStream = await this.CreateEventStreamAsync().ConfigureAwait(false))
            {
                await eventStream.SaveAsync(
                    aggregateRoot.UncommittedEvents.OfType<VersionableEvent>(),
                    aggregateRoot.Version,
                    new Dictionary<string, object>()).ConfigureAwait(false);
            }

            aggregateRoot.CommitEvents();
        }

        private async Task SaveSnapshotAsync(ISnapshot snapshot)
        {
            using (var eventStream = await this.CreateEventStreamAsync().ConfigureAwait(false))
            {
                await eventStream.SaveSnapshotAsync(snapshot).ConfigureAwait(false);
            }
        }

        private async Task CreateTestEventsAsync(int numberOfEvents)
        {
            var factory = this.configuration.Get<DbConnectionFactory>(SqlEventStore.ConnectionFactory);
            
            for (var version = 0; version < numberOfEvents; version++)
            {
                var @event = new ValueEvent(version);
                await SaveEventAsync(factory, new VersionableEvent(@event).WithVersion(version)).ConfigureAwait(false);
            }
        }

        private async Task SaveEventAsync(DbConnectionFactory factory, VersionableEvent versionableEvent)
        {
            const string ConnectionStringName = "EventStore";

            var aggregateType = typeof(MyDynamicEventSourcedAggregateRoot).FullName;
            var headers = new Dictionary<string, object>();
            var eventDescriptor = new SqlEventDescriptor(aggregateType, this.aggregateId, versionableEvent, headers);

            using (var connection = await factory.CreateAsync(ConnectionStringName).ConfigureAwait(false))
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
    }
}