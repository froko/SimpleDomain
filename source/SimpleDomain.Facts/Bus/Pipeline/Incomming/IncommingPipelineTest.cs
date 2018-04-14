//-------------------------------------------------------------------------------
// <copyright file="IncommingPipelineTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class IncommingPipelineTest
    {
        private readonly FakeEnvelopeStep envelopeStep;
        private readonly FakeMessageStep messageStep;
        private readonly IncommingPipeline testee;

        public IncommingPipelineTest()
        {
            this.envelopeStep = new FakeEnvelopeStep();
            this.messageStep = new FakeMessageStep();

            var incommingEnvelopeSteps = new List<IncommingEnvelopeStep> { this.envelopeStep };
            var incommingMessageSteps = new List<IncommingMessageStep> { this.messageStep };

            this.testee = new IncommingPipeline(
                A.Fake<IHavePipelineConfiguration>(),
                incommingEnvelopeSteps,
                incommingMessageSteps);
        }

        [Fact]
        public async Task ShouldCallEnvelopeStepsAndMessageSteps()
        {
            var message = new ValueCommand(11);
            var envelope = Envelope.Create(new EndpointAddress("sender"), new EndpointAddress("recipient"), message);

            await this.testee.InvokeAsync(envelope).ConfigureAwait(false);

            this.envelopeStep.HasBeenCalled.Should().BeTrue();
            this.messageStep.HasBeenCalled.Should().BeTrue();
        }

        private class FakeEnvelopeStep : IncommingEnvelopeStep
        {
            public bool HasBeenCalled { get; private set; }

            public override string Name
            {
                get { return "FakeEnvelopeStep"; }
            }

            public override Task InvokeAsync(IncommingEnvelopeContext context, Func<Task> next)
            {
                context.SetMessage();
                this.HasBeenCalled = true;
                return Task.CompletedTask;
            }
        }

        private class FakeMessageStep : IncommingMessageStep
        {
            public bool HasBeenCalled { get; private set; }

            public override string Name
            {
                get { return "FakeMessageStep"; }
            }

            public override Task InvokeAsync(IncommingMessageContext context, Func<Task> next)
            {
                this.HasBeenCalled = true;
                return Task.CompletedTask;
            }
        }
    }
}