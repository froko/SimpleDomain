//-------------------------------------------------------------------------------
// <copyright file="FinalIncommingEnvelopeStepTest.cs" company="frokonet.ch">
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
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Xunit;

    public class FinalIncommingEnvelopeStepTest
    {
        [Fact]
        public async Task ShouldSetMessageOnContext()
        {
            var incommingEnvelopeContext = A.Fake<IncommingEnvelopeContext>();
            var testee = new FinalIncommingEnvelopeStep();

            await testee.InvokeAsync(incommingEnvelopeContext, null);

            A.CallTo(() => incommingEnvelopeContext.SetMessage()).MustHaveHappened();
        }

        [Fact]
        public async Task DoesNotCallNext()
        {
            var incommingEnvelopeContext = A.Fake<IncommingEnvelopeContext>();
            var next = A.Fake<Func<Task>>();
            var testee = new FinalIncommingEnvelopeStep();

            await testee.InvokeAsync(incommingEnvelopeContext, next);

            A.CallTo(() => next.Invoke()).MustNotHaveHappened();
        }
    }
}