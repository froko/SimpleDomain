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
        private readonly IEventStore testee;

        public SqlIntegrationTest()
        {
            this.aggregateId = Guid.NewGuid();
            this.bus = A.Fake<IDeliverMessages>();

            var factory = new EventStoreFactory();
            var configuration = new ContainerLessEventStoreConfiguration(factory);
            configuration.UseSqlEventStore();

            this.testee = factory.Create(configuration, bus);
        }

        [Fact, WithTransaction]
        public async Task DispatchesEventsAfterSavingThem()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId);
            aggregateRoot.ChangeValue(0);
            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(22);

            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
            {
                await eventStream.SaveAsync(
                    aggregateRoot.UncommittedEvents.OfType<VersionableEvent>(),
                    aggregateRoot.Version,
                    new Dictionary<string, object>());
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

            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
            {
                await eventStream.SaveAsync(
                    aggregateRoot.UncommittedEvents.OfType<VersionableEvent>(),
                    aggregateRoot.Version,
                    new Dictionary<string, object>());

                var eventHistory = await eventStream.ReplayAsync();

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

            await this.SaveEventsAsync(aggregateRoot);

            var firstSnapshot = aggregateRoot.CreateSnapshot();

            await this.SaveSnapshotAsync(firstSnapshot);

            aggregateRoot.ChangeValue(33);
            aggregateRoot.ChangeValue(44);

            await this.SaveEventsAsync(aggregateRoot);

            var secondSnapshot = aggregateRoot.CreateSnapshot();

            await this.SaveSnapshotAsync(secondSnapshot);

            aggregateRoot.ChangeValue(55);
            aggregateRoot.ChangeValue(66);
            aggregateRoot.ChangeValue(77);

            await this.SaveEventsAsync(aggregateRoot);

            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
            {
                var hasSnapshot = await eventStream.HasSnapshotAsync();
                var snapshotFromEventStore = await eventStream.GetLatestSnapshotAsync();

                hasSnapshot.Should().BeTrue();
                snapshotFromEventStore.Version.Should().Be(secondSnapshot.Version);

                var eventHistorySinceLatestSnapshot = await eventStream.ReplayAsyncFromSnapshot(snapshotFromEventStore);

                eventHistorySinceLatestSnapshot.Should().HaveCount(3);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 55);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 66);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 77);
            }
        }

        private async Task SaveEventsAsync(IEventSourcedAggregateRoot aggregateRoot)
        {
            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
            {
                await eventStream.SaveAsync(
                    aggregateRoot.UncommittedEvents.OfType<VersionableEvent>(),
                    aggregateRoot.Version,
                    new Dictionary<string, object>());
            }

            aggregateRoot.CommitEvents();
        }

        private async Task SaveSnapshotAsync(ISnapshot snapshot)
        {
            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
            {
                await eventStream.SaveSnapshotAsync(snapshot);
            }
        }
    }
}