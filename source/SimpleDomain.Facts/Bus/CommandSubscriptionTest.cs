//-------------------------------------------------------------------------------
// <copyright file="CommandSubscriptionTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class CommandSubscriptionTest
    {
        [Fact]
        public void ShouldDecideIfSubscriptionCanHandleCommandByInstance()
        {
            var commandOfSameType = new MyCommand();
            var commandOfOtherType = new OtherCommand();

            var handler = new Func<MyCommand, Task>(cmd => Task.CompletedTask);
            var testee = new CommandSubscription<MyCommand>(handler);

            testee.CanHandle(commandOfSameType).Should().BeTrue();
            testee.CanHandle(commandOfOtherType).Should().BeFalse();
        }

        [Fact]
        public void ShouldDecideIfSubscriptionCanHandleCommandByType()
        {
            var handler = new Func<MyCommand, Task>(cmd => Task.CompletedTask);
            var testee = new CommandSubscription<MyCommand>(handler);

            testee.CanHandle<MyCommand>().Should().BeTrue();
            testee.CanHandle<OtherCommand>().Should().BeFalse();
        }

        [Fact]
        public async Task ShouldHandleCommand()
        {
            const int Value = 42;

            var expectedValue = 0;
            var valueCommand = new ValueCommand(Value);

            var handler = new Func<ValueCommand, Task>(cmd => 
            {
                expectedValue = cmd.Value;
                return Task.CompletedTask;
            });

            var testee = new CommandSubscription<ValueCommand>(handler);

            await testee.HandleAsync(valueCommand);

            expectedValue.Should().Be(Value);
        }

        [Fact]
        public void ThrowsException_WhenTryingToHandleNullAsCommand()
        {
            var handler = new Func<ValueCommand, Task>(cmd => Task.CompletedTask);

            var testee = new CommandSubscription<ValueCommand>(handler);

            Func<Task> func = () => testee.HandleAsync(null);

            func.ShouldThrow<ArgumentNullException>();
        }
    }
}