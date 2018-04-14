//-------------------------------------------------------------------------------
// <copyright file="FinalOutgoingEnvelopeStepTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Common;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class FinalOutgoingEnvelopeStepTest
    {
        private readonly OutgoingEnvelopeContext outgoingEnvelopeContext;
        private readonly Func<Envelope, Task> finalActionForEnvelope;
        private readonly FinalOutgoingEnvelopeStep testee;

        public FinalOutgoingEnvelopeStepTest()
        {
            Trace.Listeners.Add(InMemoryTraceListener.Instance);

            var headers = new Dictionary<string, object> { { HeaderKeys.Recipient, "recipient" } };
            var body = new MyCommand();
            var envelope = new Envelope(headers, body);
            var configuration = A.Fake<IHavePipelineConfiguration>();

            this.outgoingEnvelopeContext = new OutgoingEnvelopeContext(envelope, configuration);
            this.finalActionForEnvelope = A.Fake<Func<Envelope, Task>>();
            this.testee = new FinalOutgoingEnvelopeStep(this.finalActionForEnvelope);
        }

        [Fact]
        public async Task ShouldInvokeGivenFinalActionForEnvelope()
        {
            await this.testee.InvokeAsync(this.outgoingEnvelopeContext, null).ConfigureAwait(false);

            A.CallTo(() => this.finalActionForEnvelope.Invoke(A<Envelope>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task DoesNotCallNext()
        {
            var next = A.Fake<Func<Task>>();

            await this.testee.InvokeAsync(this.outgoingEnvelopeContext, next).ConfigureAwait(false);

            A.CallTo(() => next.Invoke()).MustNotHaveHappened();
        }

        [Fact]
        public async Task LogsDistributionOfMessage()
        {
            await this.testee.InvokeAsync(this.outgoingEnvelopeContext, null).ConfigureAwait(false);

            "Sending Command of type SimpleDomain.TestDoubles.MyCommand to recipient".Should().HaveBeenLogged().WithInfoLevel();
        }
    }
}