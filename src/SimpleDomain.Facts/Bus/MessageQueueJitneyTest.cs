//-------------------------------------------------------------------------------
// <copyright file="MessageQueueJitneyTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Bus.Pipeline.Outgoing;
    using SimpleDomain.Common;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class MessageQueueJitneyTest
    {
        private readonly IHaveJitneyConfiguration configuration;
        private readonly IMessageQueueProvider messageQueueProvider;
        private readonly OutgoingPipeline outgoingPipeline;
        private readonly EndpointAddress localEndpointAddress;
        private readonly MessageQueueJitney testee;

        public MessageQueueJitneyTest()
        {
            Trace.Listeners.Add(InMemoryTraceListener.Instance);

            this.configuration = A.Fake<IHaveJitneyConfiguration>();
            this.messageQueueProvider = A.Fake<IMessageQueueProvider>();
            this.outgoingPipeline = A.Fake<OutgoingPipeline>();
            this.localEndpointAddress = new EndpointAddress("myQueue");

            A.CallTo(() => this.configuration.LocalEndpointAddress).Returns(this.localEndpointAddress);
            A.CallTo(() => this.configuration.Get<IMessageQueueProvider>(MessageQueueJitney.MessageQueueProvider))
                .Returns(this.messageQueueProvider);
            A.CallTo(() => this.configuration.CreateOutgoingPipeline(A<Func<Envelope, Task>>.Ignored))
                .Returns(this.outgoingPipeline);
            A.CallTo(() => this.messageQueueProvider.TransportMediumName).Returns("Foo");

            this.testee = new MessageQueueJitney(this.configuration);
        }

        [Fact]
        public void ThrowsException_WhenTryingToInjectNullAsJitneyConfiguration()
        {
            Action action = () => { new SimpleJitney(null); };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task LogsConfiguration_WhenStartingAsync()
        {
            A.CallTo(() => this.configuration.GetSummary(typeof(MessageQueueJitney)))
                .Returns("Some useful configuration info");

            await this.testee.StartAsync().ConfigureAwait(false);

            "Some useful configuration info".Should().HaveBeenLogged().WithDebugLevel();
        }

        [Fact]
        public async Task ConnectsToMessageQueueProvider_WhenStartingAsync()
        {
            await this.testee.StartAsync().ConfigureAwait(false);

            A.CallTo(() => this.messageQueueProvider.Connect(this.localEndpointAddress, A<Func<Envelope, Task>>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendsSubscriptionMessages_WhenStartingAsync()
        {
            var subscriptions = A.Fake<IHaveJitneySubscriptions>();

            A.CallTo(() => subscriptions.GetSubscribedEventTypes()).Returns(new[] { typeof(MyEvent), typeof(OtherEvent) });
            A.CallTo(() => this.configuration.Subscriptions).Returns(subscriptions);

            await this.testee.StartAsync().ConfigureAwait(false);

            A.CallTo(() => this.outgoingPipeline.InvokeAsync(A<SubscriptionMessage>.Ignored))
                .MustHaveHappened(2, Times.Exactly);
        }

        [Fact]
        public async Task LogsStart_WhenStartingAsync()
        {
            await this.testee.StartAsync().ConfigureAwait(false);

            "MessageQueueJitney has been started with Foo as transport medium".Should().HaveBeenLogged().WithInfoLevel();
        }

        [Fact]
        public async Task CallsOutgoingPipeline_WhenSendingCommand()
        {
            var command = new ValueCommand(11);

            await this.testee.SendAsync(command).ConfigureAwait(false);

            A.CallTo(() => this.outgoingPipeline.InvokeAsync(command)).MustHaveHappened();
        }

        [Fact]
        public void ThrowsException_WhenTryingToSendNullAsCommand()
        {
            Func<Task> action = () => this.testee.SendAsync<ValueCommand>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task CallsOutgoingPipeline_WhenPublishingEvent()
        {
            var @event = new ValueEvent(11);

            await this.testee.PublishAsync(@event).ConfigureAwait(false);

            A.CallTo(() => this.outgoingPipeline.InvokeAsync(@event)).MustHaveHappened();
        }

        [Fact]
        public void ThrowsException_WhenTryingToPublishNullAsEvent()
        {
            Func<Task> action = () => this.testee.PublishAsync<ValueEvent>(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}