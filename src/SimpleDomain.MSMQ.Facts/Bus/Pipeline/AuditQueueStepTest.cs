//-------------------------------------------------------------------------------
// <copyright file="AuditQueueStepTest.cs" company="frokonet.ch">
//   Copyright (c) 2014-2017
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

namespace SimpleDomain.Bus.Pipeline
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class AuditQueueStepTest
    {
        private const string QueueName = "test.audit";

        private readonly ISendEnvelopesToMessageQueue messageQueueSender;
        private readonly Func<Task> nextStep;

        public AuditQueueStepTest()
        {
            this.messageQueueSender = A.Fake<ISendEnvelopesToMessageQueue>();
            this.nextStep = A.Fake<Func<Task>>();
        }

        [Fact]
        public void CanCreateInstanceWithQueueName()
        {
            var instance = new AuditQueueStep(QueueName);

            instance.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateInstanceWithQueueNameAndMessageQueueSender()
        {
            var instance = new AuditQueueStep(QueueName, this.messageQueueSender);

            instance.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithEmptyStringAsQueueName()
        {
            Action action = () => new AuditQueueStep(string.Empty);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsQueueName()
        {
            Action action = () => new AuditQueueStep(null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsAsMessageQueueSender()
        {
            Action action = () => new AuditQueueStep(QueueName, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldExposeStepNameWithQueueName()
        {
            var testee = new AuditQueueStep(QueueName);

            testee.Name.Should().Be($"Audit Queue Step (test.audit@{Environment.MachineName})");
        }

        [Fact]
        public async Task ShouldSendCommandToAuditQueue()
        {
            var context = CreateIncommingMessageContext(A.Fake<ICommand>());
            var testee = new AuditQueueStep(QueueName, this.messageQueueSender);

            await testee.InvokeAsync(context, this.nextStep).ConfigureAwait(false);

            A.CallTo(() => this.messageQueueSender.Send(A<Envelope>.Ignored, A<EndpointAddress>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task ShouldSendEventToAuditQueue()
        {
            var context = CreateIncommingMessageContext(A.Fake<IEvent>());
            var testee = new AuditQueueStep(QueueName, this.messageQueueSender);

            await testee.InvokeAsync(context, this.nextStep).ConfigureAwait(false);

            A.CallTo(() => this.messageQueueSender.Send(A<Envelope>.Ignored, A<EndpointAddress>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task MustNotSendSubscriptionMessageToAuditQueue()
        {
            var message = new SubscriptionMessage(new EndpointAddress("recipient"), typeof(ValueCommand).FullName);
            var context = CreateIncommingMessageContext(message);
            var testee = new AuditQueueStep(QueueName, this.messageQueueSender);

            await testee.InvokeAsync(context, this.nextStep).ConfigureAwait(false);

            A.CallTo(() => this.messageQueueSender.Send(A<Envelope>.Ignored, A<EndpointAddress>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ShouldSendSubscriptionMessageToAuditQueue_WhenAuditQueueStepInstanceIsToldToDoSo()
        {
            var message = new SubscriptionMessage(new EndpointAddress("recipient"), typeof(ValueCommand).FullName);
            var context = CreateIncommingMessageContext(message);
            var testee = new AuditQueueStep(QueueName, this.messageQueueSender).WithSubscriptionMessages();

            await testee.InvokeAsync(context, this.nextStep).ConfigureAwait(false);

            A.CallTo(() => this.messageQueueSender.Send(A<Envelope>.Ignored, A<EndpointAddress>.Ignored)).MustHaveHappened();
        }

        private static IncommingMessageContext CreateIncommingMessageContext(IMessage message)
        {
            var envelope = EnvelopeBuilder.Build(message);
            return new IncommingMessageContext(envelope, A.Fake<IHavePipelineConfiguration>());
        }
    }
}