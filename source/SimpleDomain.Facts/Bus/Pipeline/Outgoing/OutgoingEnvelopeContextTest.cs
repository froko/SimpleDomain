//-------------------------------------------------------------------------------
// <copyright file="OutgoingEnvelopeContextTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class OutgoingEnvelopeContextTest
    {
        [Fact]
        public void IsPipelineContext()
        {
            var testee = new OutgoingEnvelopeContext(
                A.Fake<Envelope>(),
                A.Fake<IHavePipelineConfiguration>());

            testee.Should().BeAssignableTo<PipelineContext>();
        }

        [Fact]
        public void ShouldExposePipelineConfiguration()
        {
            var pipelineConfiguration = A.Fake<IHavePipelineConfiguration>();
            var testee = new OutgoingEnvelopeContext(A.Fake<Envelope>(), pipelineConfiguration);

            testee.Configuration.Should().Be(pipelineConfiguration);
        }

        [Fact]
        public void ShouldExposeEnvelope()
        {
            var envelope = A.Fake<Envelope>();
            var testee = new OutgoingEnvelopeContext(envelope, A.Fake<IHavePipelineConfiguration>());

            testee.Envelope.Should().Be(envelope);
        }
    }
}