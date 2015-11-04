//-------------------------------------------------------------------------------
// <copyright file="RavenEventStreamTest.cs" company="frokonet.ch">
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
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class RavenEventStreamTest : EmbeddedRavenDbTest
    {
        private readonly RavenEventStream<MyStaticEventSourcedAggregateRoot> testee;

        public RavenEventStreamTest()
        {
            var aggregateId = Guid.NewGuid();
            Func<IEvent, Task> dispatchAsync = @event => Task.FromResult(0);

            DocumentStoreSetup.CreateIndexes(this.DocumentStore);
            DocumentStoreSetup.RegisterIdConventions(this.DocumentStore);

            this.testee = new RavenEventStream<MyStaticEventSourcedAggregateRoot>(aggregateId, dispatchAsync, this.DocumentStore.OpenAsyncSession());
        }

        [Fact]
        public async Task CanSaveAndReplayEvents()
        {
            var firstEvent = new VersionableEvent(new ValueEvent(11)).With(1);
            var secondEvent = new VersionableEvent(new ValueEvent(22)).With(2);

            var events = new[] { firstEvent, secondEvent };

            await this.testee.SaveAsync(events, 1, new Dictionary<string, object>());

            var replayedEvents = await this.testee.ReplayAsync();

            replayedEvents.Should().HaveCount(2);
            replayedEvents.Should().Contain(e => (e as ValueEvent).Value == 11);
            replayedEvents.Should().Contain(e => (e as ValueEvent).Value == 22);
        }

        [Fact]
        public async Task CanSaveSnapshots()
        {
            bool hasSnapshot;

            hasSnapshot = await this.testee.HasSnapshotAsync();
            hasSnapshot.Should().BeFalse();

            var snapshot = new MySnapshot(11).WithVersion(0);

            await this.testee.SaveSnapshotAsync(snapshot);

            hasSnapshot = await this.testee.HasSnapshotAsync();
            hasSnapshot.Should().BeTrue();
        }

        [Fact]
        public async Task CanGetLatestSnapshot()
        {
            var firstSnapshot = new MySnapshot(11).WithVersion(0);
            var secondSnapshot = new MySnapshot(22).WithVersion(1);

            await this.testee.SaveSnapshotAsync(firstSnapshot);
            await this.testee.SaveSnapshotAsync(secondSnapshot);

            var latestSnapshot = await this.testee.GetLatestSnapshotAsync();

            latestSnapshot.Should().BeAssignableTo<MySnapshot>();

            var myLatestSnapshot = latestSnapshot as MySnapshot;
            if (myLatestSnapshot != null)
            {
                myLatestSnapshot.Value.Should().Be(22);
                myLatestSnapshot.Version.Should().Be(1);
            }
        }
    }
}