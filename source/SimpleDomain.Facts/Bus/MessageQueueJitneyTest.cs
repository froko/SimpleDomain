//-------------------------------------------------------------------------------
// <copyright file="MessageQueueJitneyTest.cs" company="frokonet.ch">
//   Copyright (c) 2016
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

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Bus.Pipeline.Outgoing;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class MessageQueueJitneyTest
    {
        private readonly IHaveJitneyConfiguration configuration;
        
        public MessageQueueJitneyTest()
        {
            this.configuration = A.Fake<IHaveJitneyConfiguration>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToInjectNullAsJitneyConfiguration()
        {
            Action action = () => { new SimpleJitney(null); };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async Task ConnectsToMessageQueueProvider_WhenStartingAsync()
        {
            var messageQueueProvider = A.Fake<IMessageQueueProvider>();
            var localEndpointAddress = new EndpointAddress("myQueue");

            A.CallTo(() => this.configuration.Get<IMessageQueueProvider>(MessageQueueJitney.MessageQueueProvider))
                .Returns(messageQueueProvider);

            A.CallTo(() => this.configuration.LocalEndpointAddress).Returns(localEndpointAddress);

            var testee = this.CreateTestee();

            await testee.StartAsync();

            A.CallTo(() => messageQueueProvider.Connect(localEndpointAddress, A<Func<Envelope, Task>>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public async Task FactMethodName()
        {
            var messageQueueProvider = A.Fake<IMessageQueueProvider>();
            var localEndpointAddress = new EndpointAddress("myQueue");
            var eventTypes = new[] { typeof(MyEvent), typeof(OtherEvent) };
            var outgoingPipeline = A.Fake<OutgoingPipeline>();

            A.CallTo(() => this.configuration.Get<IMessageQueueProvider>(MessageQueueJitney.MessageQueueProvider))
                .Returns(messageQueueProvider);

            A.CallTo(() => this.configuration.LocalEndpointAddress).Returns(localEndpointAddress);

            A.CallTo(() => this.configuration.CreateOutgoingPipeline(A<Func<Envelope, Task>>.Ignored))
                .Returns(outgoingPipeline);

            A.CallTo(() => this.configuration.Subscriptions.GetSubscribedEventTypes()).Returns(eventTypes);

            var testee = this.CreateTestee();

            await testee.StartAsync();

            A.CallTo(() => outgoingPipeline.InvokeAsync(A<SubscriptionMessage>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public async Task CallsOutgoingPipeline_WhenSendingCommand()
        {
            var testee = this.CreateTestee();
            var command = new ValueCommand(11);
            var outgoingPipeline = A.Fake<OutgoingPipeline>();

            A.CallTo(() => this.configuration.CreateOutgoingPipeline(A<Func<Envelope, Task>>.Ignored))
                .Returns(outgoingPipeline);

            await testee.SendAsync(command);

            A.CallTo(() => outgoingPipeline.InvokeAsync(command)).MustHaveHappened();
        }

        [Fact]
        public void ThrowsException_WhenTryingToSendNullAsCommand()
        {
            var testee = this.CreateTestee();

            Func<Task> action = () => testee.SendAsync<ValueCommand>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async Task CallsOutgoingPipeline_WhenPublishingEvent()
        {
            var testee = this.CreateTestee();
            var @event = new ValueEvent(11);
            var outgoingPipeline = A.Fake<OutgoingPipeline>();

            A.CallTo(() => this.configuration.CreateOutgoingPipeline(A<Func<Envelope, Task>>.Ignored))
                .Returns(outgoingPipeline);

            await testee.PublishAsync(@event);

            A.CallTo(() => outgoingPipeline.InvokeAsync(@event)).MustHaveHappened();
        }

        [Fact]
        public void ThrowsException_WhenTryingToPublishNullAsEvent()
        {
            var testee = this.CreateTestee();

            Func<Task> action = () => testee.PublishAsync<ValueEvent>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        private MessageQueueJitney CreateTestee()
        {
            return new MessageQueueJitney(this.configuration);
        }
    }
}