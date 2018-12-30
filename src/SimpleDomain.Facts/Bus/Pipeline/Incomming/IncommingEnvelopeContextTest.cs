//-------------------------------------------------------------------------------
// <copyright file="IncommingEnvelopeContextTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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

    public class IncommingEnvelopeContextTest
    {
        [Fact]
        public void IsPipelineContext()
        {
            var testee = new IncommingEnvelopeContext(
                A.Fake<Envelope>(),
                A.Fake<IHavePipelineConfiguration>());

            testee.Should().BeAssignableTo<PipelineContext>();
        }

        [Fact]
        public void ShouldExposePipelineConfiguration()
        {
            var pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();
            var testee = new IncommingEnvelopeContext(A.Fake<Envelope>(), pipelineConfiguration);

            testee.Configuration.Should().Be(pipelineConfiguration);
        }

        [Fact]
        public void CanSetMessage()
        {
            var message = new ValueCommand(11);
            var envelope = Envelope.Create(new EndpointAddress("sender"), new EndpointAddress("recipient"), message);
            var pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();
            var testee = new IncommingEnvelopeContext(envelope, pipelineConfiguration);

            testee.SetMessage();

            testee.Message.Should().Be(message);
        }
    }
}