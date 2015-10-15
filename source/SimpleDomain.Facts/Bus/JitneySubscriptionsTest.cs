//-------------------------------------------------------------------------------
// <copyright file="JitneySubscriptionsTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
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
        
        private readonly IResolveTypes typeResolver;
        private readonly JitneySubscriptions testee;

        public JitneySubscriptionsTest()
        {
            this.typeResolver = A.Fake<IResolveTypes>();
            this.testee = new JitneySubscriptions(this.typeResolver);
        }
        
        [Fact]
        public async Task CanGetCommandSubscription_WhenItHasBeenSubscribedAsAsyncActionBefore()
        {
            var expectedValue = 0;
            var valueCommand = new ValueCommand(Value);

            var handler = new Func<ValueCommand, Task>(cmd =>
            {
                expectedValue = cmd.Value;
                return Task.FromResult(0);
            });

            this.testee.SubscribeCommandHandler(handler);

            var subscription = this.testee.GetCommandSubscription(valueCommand);
            await subscription.HandleAsync(valueCommand);

            expectedValue.Should().Be(Value);
        }
        
        [Fact]
        public async Task CanGetCommandSubscription_WhenItHasBeenSubscribedWithTypesBefore()
        {
            var valueCommand = new ValueCommand(Value);
            var commandHandlerInstance = A.Fake<IHandleAsync<ValueCommand>>();

            A.CallTo(() => this.typeResolver.Resolve<IHandleAsync<ValueCommand>>()).Returns(commandHandlerInstance);
            
            var subscription = this.testee.GetCommandSubscription(valueCommand);
            await subscription.HandleAsync(valueCommand);
            
            A.CallTo(() => commandHandlerInstance.HandleAsync(valueCommand)).MustHaveHappened();
        }

        [Fact]
        public async Task CanGetEventSubscriptions_WhenTheyHaveBeenSubscribedBefore()
        {
            var expectedValue = 0;
            var valueEvent = new ValueEvent(Value);
            var eventHandlerInstance = A.Fake<IHandleAsync<ValueEvent>>();
            var eventHandler = new Func<ValueEvent, Task>(@event =>
            {
                expectedValue = @event.Value;
                return Task.FromResult(0);
            });

            A.CallTo(() => this.typeResolver.ResolveAll<IHandleAsync<ValueEvent>>()).Returns(new[] { eventHandlerInstance });

            this.testee.SubscribeEventHandler(eventHandler);

            var subscriptions = this.testee.GetEventSubscriptions(valueEvent);
            var tasks = subscriptions.Select(s => s.HandleAsync(valueEvent));

            await Task.WhenAll(tasks);

            expectedValue.Should().Be(Value);
            A.CallTo(() => eventHandlerInstance.HandleAsync(valueEvent)).MustHaveHappened();
        }

        [Fact]
        public void Throws_Exception_WhenTryingToSubscribeNullAsCommandHandler()
        {
            Action action = () => this.testee.SubscribeCommandHandler<ValueCommand>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToSubscribeNullAsEventHandler()
        {
            Action action = () => this.testee.SubscribeEventHandler<ValueEvent>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenNoCommandHandlerSubscriptionCanBeFound()
        {
            var valueCommand = new ValueCommand(Value);

            A.CallTo(() => this.typeResolver.Resolve<IHandleAsync<ValueCommand>>()).Returns(null);

            Action action = () => this.testee.GetCommandSubscription(valueCommand);

            action.ShouldThrow<NoSubscriptionException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToSubscribeCommandHandlerAsAsyncActionTwiceForSameCommand()
        {
            var firstHandler = new Func<ValueCommand, Task>(cmd => Task.FromResult(0));
            var secondHandler = new Func<ValueCommand, Task>(cmd => Task.FromResult(0));

            this.testee.SubscribeCommandHandler(firstHandler);

            Action action = () => this.testee.SubscribeCommandHandler(secondHandler);

            action.ShouldThrow<CommandSubscriptionException<ValueCommand>>();
        }
    }
}