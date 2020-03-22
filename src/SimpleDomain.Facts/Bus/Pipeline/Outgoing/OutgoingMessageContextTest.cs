//-------------------------------------------------------------------------------
// <copyright file="OutgoingMessageContextTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class OutgoingMessageContextTest
    {
        [Fact]
        public void IsPipelineContext()
        {
            var testee = new OutgoingMessageContext(
                A.Fake<IMessage>(),
                A.Fake<IHavePipelineConfiguration>());

            testee.Should().BeAssignableTo<PipelineContext>();
        }

        [Fact]
        public void ShouldExposePipelineConfiguration()
        {
            var pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();

            var testee = new OutgoingMessageContext(A.Fake<IMessage>(), pipelineConfiguration);

            testee.Configuration.Should().Be(pipelineConfiguration);
        }

        [Fact]
        public void ShouldExposeMessage()
        {
            var message = A.Fake<IMessage>();

            var testee = new OutgoingMessageContext(message, A.Fake<IHavePipelineConfiguration>());

            testee.Message.Should().Be(message);
        }

        [Fact]
        public void CanCreateEnvelope()
        {
            var message = A.Fake<IMessage>();
            var pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();
            A.CallTo(() => pipelineConfiguration.LocalEndpointAddress).Returns(new EndpointAddress("sender"));

            var testee = new OutgoingMessageContext(message, pipelineConfiguration);

            testee.CreateEnvelope(new EndpointAddress("recipient"));

            testee.Envelopes.Should().HaveCount(1);
            testee.Envelopes.Should().Contain(e => IsValidEnvelope(e, message));
        }

        [Fact]
        public void CanCreateEnvelopeWithCorrelationId()
        {
            var correlationId = Guid.NewGuid();
            var message = A.Fake<IMessage>();
            var pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();
            A.CallTo(() => pipelineConfiguration.LocalEndpointAddress).Returns(new EndpointAddress("sender"));
            A.CallTo(() => pipelineConfiguration.HasCorrelationId).Returns(true);
            A.CallTo(() => pipelineConfiguration.PeekCorrelationId()).Returns(correlationId);

            var testee = new OutgoingMessageContext(message, pipelineConfiguration);

            testee.CreateEnvelope(new EndpointAddress("recipient"));

            testee.Envelopes.Should().HaveCount(1);
            testee.Envelopes.Should().Contain(e => IsValidEnvelope(e, message, correlationId));
        }

        private static bool IsValidEnvelope(Envelope envelope, IMessage message)
        {
            return envelope.Body.Equals(message)
                && ((EndpointAddress)envelope.Headers[HeaderKeys.Sender]).QueueName == "sender"
                && ((EndpointAddress)envelope.Headers[HeaderKeys.Recipient]).QueueName == "recipient";
        }

        private static bool IsValidEnvelope(Envelope envelope, IMessage message, Guid correlationId)
        {
            return envelope.Body.Equals(message)
                   && ((EndpointAddress)envelope.Headers[HeaderKeys.Sender]).QueueName == "sender"
                   && ((EndpointAddress)envelope.Headers[HeaderKeys.Recipient]).QueueName == "recipient"
                   && envelope.CorrelationId == correlationId;
        }
    }
}