//-------------------------------------------------------------------------------
// <copyright file="ErrorQueueStepTest.cs" company="frokonet.ch">
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
    using FluentAssertions.Common;

    using SimpleDomain.Bus.Pipeline.Incomming;

    using Xunit;

    public class ErrorQueueStepTest
    {
        private const string QueueName = "test.error";

        private readonly ISendEnvelopesToMessageQueue messageQueueSender;
        private readonly Func<Task> nextStep;

        public ErrorQueueStepTest()
        {
            this.messageQueueSender = A.Fake<ISendEnvelopesToMessageQueue>();
            this.nextStep = A.Fake<Func<Task>>();
        }

        [Fact]
        public void CanCreateInstanceWithQueueName()
        {
            var instance = new ErrorQueueStep(QueueName);

            instance.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateInstanceWithQueueNameAndMessageQueueSender()
        {
            var instance = new ErrorQueueStep(QueueName, this.messageQueueSender);

            instance.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithEmptyStringAsQueueName()
        {
            Action action = () => new ErrorQueueStep(string.Empty);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsQueueName()
        {
            Action action = () => new ErrorQueueStep(null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsAsMessageQueueSender()
        {
            Action action = () => new ErrorQueueStep(QueueName, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldExposeStepNameWithQueueName()
        {
            var testee = new ErrorQueueStep(QueueName);

            testee.Name.Should().Be($"Error Queue Step (test.error@{Environment.MachineName})");
        }

        [Fact]
        public async Task MustNotSendMessageToErrorQueue_WhenNextStepSucceeds()
        {
            var context = CreateIncommingMessageContext(A.Fake<IMessage>());
            var testee = new ErrorQueueStep(QueueName, this.messageQueueSender);

            await testee.InvokeAsync(context, this.nextStep).ConfigureAwait(false);

            A.CallTo(() => this.messageQueueSender.Send(A<Envelope>.Ignored, A<EndpointAddress>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void ShouldSendMessageToErrorQueueAndRethrowException_WhenNextStepFails()
        {
            var exception = new ApplicationException("Somthing went wrong...");
            A.CallTo(() => this.nextStep.Invoke()).Throws(exception);

            var context = CreateIncommingMessageContext(A.Fake<IMessage>());
            var testee = new ErrorQueueStep(QueueName, this.messageQueueSender);

            Func<Task> action = () => testee.InvokeAsync(context, this.nextStep);

            action.Should().Throw<ApplicationException>().Which.IsSameOrEqualTo(exception);

            A.CallTo(() => this.messageQueueSender.Send(A<Envelope>.Ignored, A<EndpointAddress>.Ignored)).MustHaveHappened();
        }

        private static IncommingMessageContext CreateIncommingMessageContext(IMessage message)
        {
            var envelope = EnvelopeBuilder.Build(message);
            return new IncommingMessageContext(envelope, A.Fake<IHavePipelineConfiguration>());
        }
    }
}
