//-------------------------------------------------------------------------------
// <copyright file="FinalIncommingEnvelopeStepTest.cs" company="frokonet.ch">
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
    using System.Threading.Tasks;

    using FakeItEasy;

    using SimpleDomain.Bus.Configuration;

    using Xunit;

    public class FinalIncommingEnvelopeStepTest
    {
        [Fact]
        public async Task ShouldSetMessageOnContext()
        {
            var incommingEnvelopeContext = A.Fake<IncommingEnvelopeContext>();
            A.CallTo(() => incommingEnvelopeContext.Envelope).Returns(EnvelopeBuilder.Build());

            var testee = new FinalIncommingEnvelopeStep();

            await testee.InvokeAsync(incommingEnvelopeContext, null).ConfigureAwait(false);

            A.CallTo(() => incommingEnvelopeContext.SetMessage()).MustHaveHappened();
        }

        [Fact]
        public async Task ShouldPushCorrelationIfOfIncommingEnvelopeToTheStack()
        {
            var correlationId = Guid.NewGuid();
            var envelope = EnvelopeBuilder.Build(correlationId);

            var incommingEnvelopeContext = A.Fake<IncommingEnvelopeContext>();
            A.CallTo(() => incommingEnvelopeContext.Envelope).Returns(envelope);

            var configuration = A.Fake<IHavePipelineConfiguration>();
            A.CallTo(() => incommingEnvelopeContext.Configuration).Returns(configuration);

            var testee = new FinalIncommingEnvelopeStep();

            await testee.InvokeAsync(incommingEnvelopeContext, null).ConfigureAwait(false);

            A.CallTo(() => configuration.PushCorrelationId(correlationId)).MustHaveHappened();
        }

        [Fact]
        public async Task DoesNotCallNext()
        {
            var incommingEnvelopeContext = A.Fake<IncommingEnvelopeContext>();
            A.CallTo(() => incommingEnvelopeContext.Envelope).Returns(EnvelopeBuilder.Build());

            var next = A.Fake<Func<Task>>();
            var testee = new FinalIncommingEnvelopeStep();

            await testee.InvokeAsync(incommingEnvelopeContext, next).ConfigureAwait(false);

            A.CallTo(() => next.Invoke()).MustNotHaveHappened();
        }
    }
}