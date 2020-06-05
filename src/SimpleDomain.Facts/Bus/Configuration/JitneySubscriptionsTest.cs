//-------------------------------------------------------------------------------
// <copyright file="JitneySubscriptionsTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class JitneySubscriptionsTest
    {
        private const int Value = 42;

        private readonly AbstractHandlerRegistry handlerRegistry;
        private readonly JitneySubscriptions testee;

        public JitneySubscriptionsTest()
        {
            this.handlerRegistry = A.Fake<AbstractHandlerRegistry>();
            var handlerInvocationCache = new HandlerInvocationCache();

            this.testee = new JitneySubscriptions(this.handlerRegistry, handlerInvocationCache);
        }

        [Fact]
        public async Task CanGetCommandSubscription_WhenItHasBeenSubscribedAsAsyncActionBefore()
        {
            var expectedValue = 0;
            var command = new ValueCommand(Value);

            var handler = new Func<ValueCommand, Task>(c =>
            {
                expectedValue = c.Value;
                return Task.CompletedTask;
            });

            this.testee.AddCommandHandler(handler);

            var subscription = this.testee.GetCommandSubscription(command);
            await subscription.HandleAsync(command).ConfigureAwait(false);

            expectedValue.Should().Be(Value);
        }

        [Fact]
        public async Task CanGetCommandSubscription_WhenItHasBeenSubscribedWithAssemblyScanningBefore()
        {
            Action<Type> register = t => { };

            var command = new ValueCommand(Value);
            var handler = new ValueCommandHandler();

            A.CallTo(() => this.handlerRegistry.GetCommandHandler(command)).Returns(handler);

            this.testee.ScanAssemblyForMessageHandlers(typeof(ValueCommand).Assembly, register);

            var subscription = this.testee.GetCommandSubscription(command);
            await subscription.HandleAsync(command).ConfigureAwait(false);

            handler.Value.Should().Be(Value);
        }

        [Fact]
        public async Task CanGetEventSubscriptions_WhenTheyHaveBeenSubscribedBefore()
        {
            Action<Type> register = t => { };

            var expectedValue = 0;
            var @event = new ValueEvent(Value);
            var eventHandlerInstance = new ValueEventHandler();
            var eventHandler = new Func<ValueEvent, Task>(e =>
            {
                expectedValue = e.Value;
                return Task.CompletedTask;
            });

            A.CallTo(() => this.handlerRegistry.GetEventHandlers(@event)).Returns(new[] { eventHandlerInstance });

            this.testee.AddEventHandler(eventHandler);
            this.testee.ScanAssemblyForMessageHandlers(typeof(ValueCommand).Assembly, register);

            var subscriptions = this.testee.GetEventSubscriptions(@event);
            var tasks = subscriptions.Select(s => s.HandleAsync(@event));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            expectedValue.Should().Be(Value);
            eventHandlerInstance.Value.Should().Be(Value);
        }

        [Fact]
        public void Throws_Exception_WhenTryingToSubscribeNullAsCommandHandler()
        {
            Action action = () => this.testee.AddCommandHandler<ValueCommand>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToSubscribeNullAsEventHandler()
        {
            Action action = () => this.testee.AddEventHandler<ValueEvent>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenNoCommandHandlerSubscriptionCanBeFound()
        {
            var valueCommand = new ValueCommand(Value);

            A.CallTo(() => this.handlerRegistry.GetCommandHandler(valueCommand)).Returns(null);

            Action action = () => this.testee.GetCommandSubscription(valueCommand);

            action.Should().Throw<NoSubscriptionException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToSubscribeCommandHandlerAsAsyncActionTwiceForSameCommand()
        {
            var firstHandler = new Func<ValueCommand, Task>(cmd => Task.CompletedTask);
            var secondHandler = new Func<ValueCommand, Task>(cmd => Task.CompletedTask);

            this.testee.AddCommandHandler(firstHandler);

            Action action = () => this.testee.AddCommandHandler(secondHandler);

            action.Should().Throw<CommandSubscriptionException<ValueCommand>>();
        }

        [Fact]
        public void CanGetSubscribedEventTypes()
        {
            Action<Type> register = t => { };

            this.testee.AddEventHandler<ValueEvent>(e => Task.CompletedTask);
            this.testee.ScanAssemblyForMessageHandlers(typeof(ValueCommand).Assembly, register);

            var subscribedEventTypes = this.testee.GetSubscribedEventTypes();

            subscribedEventTypes.Should()
                .HaveCount(2)
                .And.Contain(typeof(ValueEvent))
                .And.Contain(typeof(MyEvent));
        }
    }
}