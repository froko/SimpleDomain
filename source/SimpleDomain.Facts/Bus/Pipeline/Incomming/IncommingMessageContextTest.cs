//-------------------------------------------------------------------------------
// <copyright file="IncommingMessageContextTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Incomming
{
    using System.Collections.Generic;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class IncommingMessageContextTest
    {
        private readonly Dictionary<string, object> headers;
        private readonly IHavePipelineConfiguration pipelineConfiguration;

        public IncommingMessageContextTest()
        {
            this.headers = new Dictionary<string, object> { { HeaderKeys.Sender, new EndpointAddress("sender") } };
            this.pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();
        }

        [Fact]
        public void IsPipelineContext()
        {
            var testee = this.CreateTestee(A.Fake<IMessage>());

            testee.Should().BeAssignableTo<PipelineContext>();
        }

        [Fact]
        public void ShouldExposePipelineConfiguration()
        {
            var testee = this.CreateTestee(A.Fake<IMessage>());

            testee.Configuration.Should().Be(this.pipelineConfiguration);
        }

        [Fact]
        public void ShouldExposeOriginatingEnvelopeWithHeaders()
        {
            var testee = this.CreateTestee(A.Fake<IMessage>());

            testee.Envelope.Should().NotBeNull();
            testee.Envelope.Headers.Should().BeSameAs(this.headers);
        }

        [Fact]
        public void ShouldExposeMessage()
        {
            var message = A.Fake<IMessage>();
            var testee = this.CreateTestee(message);

            testee.Message.Should().Be(message);
        }

        [Fact]
        public void ShouldExposeMessageIntentForCommands()
        {
            var message = A.Fake<ICommand>();
            var testee = this.CreateTestee(message);

            testee.MessageIntent.Should().Be(MessageIntent.Command);
        }

        [Fact]
        public void ShouldExposeMessageIntentForEvents()
        {
            var message = A.Fake<IEvent>();
            var testee = this.CreateTestee(message);

            testee.MessageIntent.Should().Be(MessageIntent.Event);
        }

        [Fact]
        public void ShouldExposeMessageIntentForSubscriptionMessages()
        {
            var message = new SubscriptionMessage(new EndpointAddress("recipient"), typeof(ValueCommand).FullName);
            var testee = this.CreateTestee(message);

            testee.MessageIntent.Should().Be(MessageIntent.SubscriptionMessage);
        }

        private IncommingMessageContext CreateTestee(IMessage message)
        {
            return new IncommingMessageContext(new Envelope(this.headers, message), this.pipelineConfiguration);
        }
    }
}