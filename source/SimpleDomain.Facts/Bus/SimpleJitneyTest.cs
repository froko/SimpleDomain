//-------------------------------------------------------------------------------
// <copyright file="SimpleJitneyTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Bus.Pipeline.Outgoing;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class SimpleJitneyTest
    {
        private readonly IHaveJitneyConfiguration configuration;
        private readonly SimpleJitney testee;

        public SimpleJitneyTest()
        {
            this.configuration = A.Fake<IHaveJitneyConfiguration>();
            this.testee = new SimpleJitney(this.configuration);
        }

        [Fact]
        public async Task CallsOutgoingPipeline_WhenSendingCommand()
        {
            var command = new ValueCommand(11);
            var outgoingPipeline = A.Fake<OutgoingPipeline>();
            A.CallTo(() => this.configuration.CreateOutgoingPipeline(A<Func<Envelope, Task>>.Ignored))
                .Returns(outgoingPipeline);
            
            await this.testee.SendAsync(command);

            A.CallTo(() => outgoingPipeline.InvokeAsync(command)).MustHaveHappened();
        }

        [Fact]
        public async Task CallsOutgoingPipeline_WhenPublishingEvent()
        {
            var @event = new ValueEvent(11);
            var outgoingPipeline = A.Fake<OutgoingPipeline>();
            A.CallTo(() => this.configuration.CreateOutgoingPipeline(A<Func<Envelope, Task>>.Ignored))
                .Returns(outgoingPipeline);

            await this.testee.PublishAsync(@event);

            A.CallTo(() => outgoingPipeline.InvokeAsync(@event)).MustHaveHappened();
        }
    }
}