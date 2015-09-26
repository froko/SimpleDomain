//-------------------------------------------------------------------------------
// <copyright file="StateBasedAggregateRootTest.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class StateBasedAggregateRootTest
    {
        private readonly MyStateBasedAggregateRoot testee;

        public StateBasedAggregateRootTest()
        {
            this.testee = new MyStateBasedAggregateRoot();
        }

        [Fact]
        public void CanCreateInstanceWithNoArguments()
        {
            var instance = new MyStateBasedAggregateRoot();

            instance.Value.Should().Be(0);
        }

        [Fact]
        public void CanCreateInstanceWithState()
        {
            var state = new MyState { Value = 42 };
            var instance = new MyStateBasedAggregateRoot(state);

            instance.Value.Should().Be(42);
        }

        [Fact]
        public void CanApplyChange()
        {
            this.testee.UpdateValue(11);

            this.testee.Value.Should().Be(11);
        }

        [Fact]
        public void AppliedChangeIsAddedToUncommittedEvents()
        {
            this.testee.UpdateValue(11);

            this.testee.UncommittedEvents.Should().Contain(e => (e as MyEvent).Value == 11);
        }

        [Fact]
        public void CanCommitUncommittedEvents()
        {
            this.testee.UpdateValue(11);

            this.testee.CommitEvents();

            this.testee.UncommittedEvents.Should().BeEmpty();
        }
    }
}