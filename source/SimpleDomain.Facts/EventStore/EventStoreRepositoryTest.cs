//-------------------------------------------------------------------------------
// <copyright file="EventStoreRepositoryTest.cs" company="frokonet.ch">
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class EventStoreRepositoryTest
    {
        private readonly Guid aggregateId;
        private readonly IEventStore eventStore;
        private readonly IEventStream eventStream;
        private readonly EventStoreRepository testee;

        public EventStoreRepositoryTest()
        {
            this.aggregateId = Guid.NewGuid();
            this.eventStore = A.Fake<IEventStore>();
            this.eventStream = A.Fake<IEventStream>();
            this.testee = new EventStoreRepository(this.eventStore);

            A.CallTo(() => this.eventStore.OpenStreamAsync<MyDynamicEventSourcedAggregateRoot>(this.aggregateId)).Returns(this.eventStream);
        }

        [Fact]
        public async Task CanGetAggregateRootById_ByReplayingAllEvents()
        {
            var eventHistory = EventHistory.Create(new ValueEvent(0), new ValueEvent(11), new ValueEvent(22));
            A.CallTo(() => this.eventStream.ReplayAsync()).Returns(eventHistory);

            var aggregateRoot = await this.testee
                .GetByIdAsync<MyDynamicEventSourcedAggregateRoot>(this.aggregateId)
                .ConfigureAwait(false);

            aggregateRoot.Version.Should().Be(2);
            aggregateRoot.Value.Should().Be(22);
        }

        [Fact]
        public async Task CanGetAggregateRootById_ByReplayingAllEventsFromSnapshot()
        {
            var snapshot = new MySnapshot(22).WithVersion(2);
            var eventsSinceSnapshot = EventHistory.Create(new ValueEvent(33), new ValueEvent(44));

            A.CallTo(() => this.eventStream.HasSnapshotAsync()).Returns(true);
            A.CallTo(() => this.eventStream.GetLatestSnapshotAsync()).Returns(snapshot);
            A.CallTo(() => this.eventStream.ReplayAsyncFromSnapshot(snapshot)).Returns(eventsSinceSnapshot);

            var aggregateRoot = await this.testee
                .GetByIdAsync<MyDynamicEventSourcedAggregateRoot>(this.aggregateId)
                .ConfigureAwait(false);

            aggregateRoot.Version.Should().Be(4);
            aggregateRoot.Value.Should().Be(44);
        }

        [Fact]
        public void ThrowsException_WhenGettingAggregateRootByIdAndThereAreNoEvents()
        {
            A.CallTo(() => this.eventStream.ReplayAsync()).Returns(EventHistory.Create());
            
            Func<Task> action = async () =>
            {
                await this.testee
                    .GetByIdAsync<MyDynamicEventSourcedAggregateRoot>(this.aggregateId)
                    .ConfigureAwait(false);
            };

            action.ShouldThrow<AggregateRootNotFoundException>();
        }

        [Fact]
        public async Task CanSaveEventsWithoutHeaders()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId);
            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(22);

            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveAsync(A<IEnumerable<VersionableEvent>>.Ignored, 1, A<IDictionary<string, object>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task CanSaveEventsWithHeaders()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId);
            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(22);

            var headers = new Dictionary<string, object> { { "UserName", "Patrick" }, { "MagicNumber", 42 } };

            await this.testee.SaveAsync(aggregateRoot, headers).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveAsync(A<IEnumerable<VersionableEvent>>.Ignored, 1, headers)).MustHaveHappened();
        }

        [Fact]
        public async Task CommitsUncommittedEventsOnAggregateRootAfterSavingUncommittedEvents()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId);
            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(22);

            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            aggregateRoot.UncommittedEvents.Should().BeEmpty();
        }

        [Fact]
        public async Task BuiltInGlobalSnapshotStrategyHasAThresholdOf100()
        {
            const int Version = 100;
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId).WithVersion(Version);

            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveSnapshotAsync(A<MySnapshot>.That.Matches(s => s.Version == Version))).MustHaveHappened();
        }

        [Fact]
        public async Task CanSetAndUseGlobalSnapshotStrategy()
        {
            const int Version = 90;
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId).WithVersion(Version);

            this.testee.WithGlobalSnapshotStrategy(30);
            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveSnapshotAsync(A<MySnapshot>.That.Matches(s => s.Version == Version))).MustHaveHappened();
        }

        [Fact]
        public async Task CanSetAndUseTypedSnapshotStrategy()
        {
            const int Version = 20;
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId).WithVersion(Version);
            
            this.testee.WithSnapshotStrategyFor<MyDynamicEventSourcedAggregateRoot>(10);
            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveSnapshotAsync(A<MySnapshot>.That.Matches(s => s.Version == Version))).MustHaveHappened();
        }

        [Fact]
        public async Task GlobalSnapshotStrategyDoesNotApplyAnymore_WhenThereIsATypedSnapshotStrategy()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId).WithVersion(100);

            this.testee.WithSnapshotStrategyFor<MyDynamicEventSourcedAggregateRoot>(30);
            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveSnapshotAsync(A<MySnapshot>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GlobalSnapshotStrategyDoesNotApply_WhenAggregateVersionIsZero()
        {
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(this.aggregateId).WithVersion(0);

            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveSnapshotAsync(A<ISnapshot>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task NoSnapshotStrategyApplies_WhenSnapshotIsNull()
        {
            var aggregateRoot = new OtherDynamicEventSourcedAggregateRoot().WithVersion(100);

            await this.testee.SaveAsync(aggregateRoot).ConfigureAwait(false);

            A.CallTo(() => this.eventStream.SaveSnapshotAsync(A<ISnapshot>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.eventStream.SaveSnapshotAsync(null)).MustNotHaveHappened();
        }
    }
}