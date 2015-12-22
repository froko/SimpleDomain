//-------------------------------------------------------------------------------
// <copyright file="IncommingMessageContextTest.cs" company="frokonet.ch">
//   Copyright (c) 2015
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

namespace SimpleDomain.Bus.Pipeline.Incomming
{
    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class IncommingMessageContextTest
    {
        [Fact]
        public void IsPipelineContext()
        {
            var testee = new IncommingMessageContext(
                A.Fake<IMessage>(),
                A.Fake<IHavePipelineConfiguration>());

            testee.Should().BeAssignableTo<PipelineContext>();
        }

        [Fact]
        public void ShouldExposePipelineConfiguration()
        {
            var pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();
            var testee = new IncommingMessageContext(A.Fake<IMessage>(), pipelineConfiguration);

            testee.Configuration.Should().Be(pipelineConfiguration);
        }

        [Fact]
        public void ShouldExposeMessage()
        {
            var message = A.Fake<IMessage>();
            var testee = new IncommingMessageContext(message, A.Fake<IHavePipelineConfiguration>());

            testee.Message.Should().Be(message);
        }

        [Fact]
        public void ShouldExposeMessageIntentForCommands()
        {
            var message = A.Fake<ICommand>();
            var testee = new IncommingMessageContext(message, A.Fake<IHavePipelineConfiguration>());

            testee.MessageIntent.Should().Be(MessageIntent.Command);
        }

        [Fact]
        public void ShouldExposeMessageIntentForEvents()
        {
            var message = A.Fake<IEvent>();
            var testee = new IncommingMessageContext(message, A.Fake<IHavePipelineConfiguration>());

            testee.MessageIntent.Should().Be(MessageIntent.Event);
        }

        [Fact]
        public void ShouldExposeMessageIntentForSubscriptionMessages()
        {
            var message = new SubscriptionMessage(new EndpointAddress("recipient"), typeof(ValueCommand).FullName);
            var testee = new IncommingMessageContext(message, A.Fake<IHavePipelineConfiguration>());

            testee.MessageIntent.Should().Be(MessageIntent.SubscriptionMessage);
        }
    }
}