//-------------------------------------------------------------------------------
// <copyright file="EventSubscriptionTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class EventSubscriptionTest
    {
        [Fact]
        public void ShouldDecideIfSubscriptionCanHandleEventByInstance()
        {
            var eventOfSameType = new MyEvent();
            var eventOfOtherType = new OtherEvent();

            var handler = new Func<MyEvent, Task>(cmd => Task.CompletedTask);
            var testee = new EventSubscription<MyEvent>(handler);

            testee.CanHandle(eventOfSameType).Should().BeTrue();
            testee.CanHandle(eventOfOtherType).Should().BeFalse();
        }

        [Fact]
        public void ShouldDecideIfSubscriptionCanHandleEventByType()
        {
            var handler = new Func<MyEvent, Task>(cmd => Task.CompletedTask);
            var testee = new EventSubscription<MyEvent>(handler);

            testee.CanHandle<MyEvent>().Should().BeTrue();
            testee.CanHandle<OtherEvent>().Should().BeFalse();
        }

        [Fact]
        public async Task ShouldHandleEvent()
        {
            const int Value = 42;

            var expectedValue = 0;
            var valueEvent = new ValueEvent(Value);

            var handler = new Func<ValueEvent, Task>(cmd =>
            {
                expectedValue = cmd.Value;
                return Task.CompletedTask;
            });

            var testee = new EventSubscription<ValueEvent>(handler);

            await testee.HandleAsync(valueEvent).ConfigureAwait(false);

            expectedValue.Should().Be(Value);
        }

        [Fact]
        public void ThrowsException_WhenTryingToHandleNullAsEvent()
        {
            var handler = new Func<ValueEvent, Task>(cmd => Task.CompletedTask);

            var testee = new EventSubscription<ValueEvent>(handler);

            Func<Task> func = () => testee.HandleAsync(null);

            func.Should().Throw<ArgumentNullException>();
        }
    }
}