//-------------------------------------------------------------------------------
// <copyright file="InMemoryIntegrationTest.cs" company="frokonet.ch">
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

    public class InMemoryIntegrationTest
    {
        private readonly Guid aggregateId;
        private readonly IDeliverMessages bus;
        private readonly ContainerLessEventStoreConfiguration configuration;
        private readonly IEventStore testee;

        public InMemoryIntegrationTest()
        {
            var factory = new EventStoreFactory();

            this.aggregateId = Guid.NewGuid();
            this.bus = A.Fake<IDeliverMessages>();
            this.configuration = new ContainerLessEventStoreConfiguration(factory);
            this.configuration.UseInMemoryEventStore();

            this.testee = factory.Create(this.configuration, this.bus);
        }

        [Fact]
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
                    new Dictionary<string, object>()).ConfigureAwait(false);
            }

            A.CallTo(() => this.bus.PublishAsync<IEvent>(A<ValueEvent>.That.Matches(e => e.Value == 0))).MustHaveHappened();
            A.CallTo(() => this.bus.PublishAsync<IEvent>(A<ValueEvent>.That.Matches(e => e.Value == 11))).MustHaveHappened();
            A.CallTo(() => this.bus.PublishAsync<IEvent>(A<ValueEvent>.That.Matches(e => e.Value == 22))).MustHaveHappened();
        }

        [Fact]
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
                    new Dictionary<string, object>()).ConfigureAwait(false);

                var eventHistory = await eventStream.ReplayAsync().ConfigureAwait(false);

                eventHistory.Should().HaveCount(3);
                eventHistory.Should().Contain(e => (e as ValueEvent).Value == 0);
                eventHistory.Should().Contain(e => (e as ValueEvent).Value == 11);
                eventHistory.Should().Contain(e => (e as ValueEvent).Value == 22);
            }
        }

        [Fact]
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

            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
            {
                var hasSnapshot = await eventStream.HasSnapshotAsync();
                var snapshotFromEventStore = await eventStream.GetLatestSnapshotAsync();

                hasSnapshot.Should().BeTrue();
                snapshotFromEventStore.Should().Be(secondSnapshot);

                var eventHistorySinceLatestSnapshot = await eventStream
                    .ReplayAsyncFromSnapshot(snapshotFromEventStore)
                    .ConfigureAwait(false);

                eventHistorySinceLatestSnapshot.Should().HaveCount(3);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 55);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 66);
                eventHistorySinceLatestSnapshot.Should().Contain(e => (e as ValueEvent).Value == 77);
            }
        }

        [Fact]
        public async Task CanReplayAllEvents()
        {
            const int NumberOfEvents = 100;

            var eventDescriptors = this.configuration.Get<List<EventDescriptor>>(InMemoryEventStore.EventDescriptors);
            var aggregateType = typeof(MyDynamicEventSourcedAggregateRoot).FullName;
            var headers = new Dictionary<string, object>();

            for (var version = 0; version < NumberOfEvents; version++)
            {
                eventDescriptors.Add(new EventDescriptor(
                    aggregateType,
                    this.aggregateId,
                    new VersionableEvent(new MyEvent()).WithVersion(version),
                    headers));
            }

            await this.testee.ReplayAllAsync().ConfigureAwait(false);

            A.CallTo(() => this.bus.PublishAsync(A<IEvent>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(NumberOfEvents));
        }
        
        private async Task SaveEventsAsync(IEventSourcedAggregateRoot aggregateRoot)
        {
            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
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
            using (var eventStream = this.testee.OpenStream<MyDynamicEventSourcedAggregateRoot>(this.aggregateId))
            {
                await eventStream.SaveSnapshotAsync(snapshot).ConfigureAwait(false);
            }
        }
    }
}