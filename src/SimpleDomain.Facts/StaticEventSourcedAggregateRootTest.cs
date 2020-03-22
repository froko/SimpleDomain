//-------------------------------------------------------------------------------
// <copyright file="StaticEventSourcedAggregateRootTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
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

namespace SimpleDomain
{
    using System.Linq;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class StaticEventSourcedAggregateRootTest
    {
        private readonly MyStaticEventSourcedAggregateRoot testee;

        public StaticEventSourcedAggregateRootTest()
        {
            this.testee = new MyStaticEventSourcedAggregateRoot();
        }

        [Fact]
        public void InitialVersionEqualsMinusOne()
        {
            this.testee.Version.Should().Be(-1);
        }

        [Fact]
        public void CanApplyChange()
        {
            var @event = new ValueEvent(11);
            this.testee.ApplyEvent(@event);

            this.testee.Value.Should().Be(11);
        }

        [Fact]
        public void AppliedChangeIsAddedToUncommittedEvents()
        {
            var @event = new ValueEvent(11);
            this.testee.ApplyEvent(@event);

            this.testee.UncommittedEvents.OfType<VersionableEvent>().Should().Contain(e => e.InnerEvent == @event);
        }

        [Fact]
        public void AggregateVersionIsIncremented_WhenChangeIsApplied()
        {
            var firstEvent = new ValueEvent(11);
            var secondEvent = new ValueEvent(22);

            this.testee.ApplyEvent(firstEvent);
            this.testee.ApplyEvent(secondEvent);

            this.testee.Version.Should().Be(1);
        }

        [Fact]
        public void IncrementedAggregateVersionIsAppliedToEvent()
        {
            var firstEvent = new ValueEvent(11);
            var secondEvent = new ValueEvent(22);

            this.testee.ApplyEvent(firstEvent);
            this.testee.ApplyEvent(secondEvent);

            this.testee.UncommittedEvents.OfType<VersionableEvent>().First().Version.Should().Be(0);
            this.testee.UncommittedEvents.OfType<VersionableEvent>().Last().Version.Should().Be(1);
        }

        [Fact]
        public void CanCommitUncommittedEvents()
        {
            var @event = new ValueEvent(11);
            this.testee.ApplyEvent(@event);

            this.testee.CommitEvents();

            this.testee.UncommittedEvents.Should().BeEmpty();
        }

        [Fact]
        public void CanLoadFromEventHistory()
        {
            var eventHistory = EventHistory.Create(new ValueEvent(11), new ValueEvent(22), new ValueEvent(33));
            this.testee.LoadFromEventHistory(eventHistory);

            this.testee.Value.Should().Be(33);
        }
    }
}