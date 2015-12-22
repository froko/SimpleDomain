//-------------------------------------------------------------------------------
// <copyright file="OutgoingPipelineTest.cs" company="frokonet.ch">
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class OutgoingPipelineTest
    {
        private readonly IHavePipelineConfiguration configuration;
        private readonly FakeMessageStep messageStep;
        private readonly FakeEnvelopeStep envelopeStep;
        private readonly OutgoingPipeline testee;

        public OutgoingPipelineTest()
        {
            this.configuration = A.Fake<IHavePipelineConfiguration>();
            this.messageStep = new FakeMessageStep();
            this.envelopeStep = new FakeEnvelopeStep();

            var outgoingMessageSteps = new List<OutgoingMessageStep> { this.messageStep };
            var outgoingEnvelopeSteps = new List<OutgoingEnvelopeStep> { this.envelopeStep };

            this.testee = new OutgoingPipeline(this.configuration, outgoingMessageSteps, outgoingEnvelopeSteps);
        }

        [Fact]
        public void ThrowsException_WhenTryingToInvokePipelineAndLocalEndpointAddressIsNotSet()
        {
            Func<Task> action = () => this.testee.InvokeAsync(new ValueCommand(11));

            action.ShouldThrow<JitneyConfigurationException>();
        }

        [Fact]
        public async Task ShouldCallMessageStepsAndEnvelopeSteps()
        {
            A.CallTo(() => this.configuration.LocalEndpointAddress).Returns(new EndpointAddress("sender"));

            await this.testee.InvokeAsync(new ValueCommand(11)).ConfigureAwait(false);

            this.messageStep.HasBeenCalled.Should().BeTrue();
            this.envelopeStep.HasBeenCalled.Should().BeTrue();
        }

        private class FakeMessageStep : OutgoingMessageStep
        {
            public bool HasBeenCalled { get; private set; }

            public override string Name
            {
                get { return "FakeMessageStep"; } 
            }

            public override Task InvokeAsync(OutgoingMessageContext context, Func<Task> next)
            {
                context.CreateEnvelope(new EndpointAddress("recipient"));
                this.HasBeenCalled = true;
                return Task.CompletedTask;
            }
        }

        private class FakeEnvelopeStep : OutgoingEnvelopeStep
        {
            public bool HasBeenCalled { get; private set; }

            public override string Name
            {
                get { return "FakeEnvelopeStep"; }
            }

            public override Task InvokeAsync(OutgoingEnvelopeContext context, Func<Task> next)
            {
                this.HasBeenCalled = true;
                return Task.CompletedTask;
            }
        }
    }
}