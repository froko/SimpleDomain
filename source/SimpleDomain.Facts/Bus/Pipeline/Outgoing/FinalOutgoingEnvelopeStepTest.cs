//-------------------------------------------------------------------------------
// <copyright file="FinalOutgoingEnvelopeStepTest.cs" company="frokonet.ch">
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
    using System.Threading.Tasks;

    using FakeItEasy;
    
    using Xunit;

    public class FinalOutgoingEnvelopeStepTest
    {
        [Fact]
        public async Task ShouldInvokeGivenFinalActionForEnvelope()
        {
            var finalActionForEnvelope = A.Fake<Func<Envelope, Task>>();
            var testee = new FinalOutgoingEnvelopeStep(finalActionForEnvelope);

            await testee.InvokeAsync(A.Fake<OutgoingEnvelopeContext>(), null);

            A.CallTo(() => finalActionForEnvelope.Invoke(A<Envelope>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task DoesNotCallNext()
        {
            var finalActionForEnvelope = A.Fake<Func<Envelope, Task>>();
            var next = A.Fake<Func<Task>>();
            var testee = new FinalOutgoingEnvelopeStep(finalActionForEnvelope);

            await testee.InvokeAsync(A.Fake<OutgoingEnvelopeContext>(), next);

            A.CallTo(() => next.Invoke()).MustNotHaveHappened();
        }
    }
}