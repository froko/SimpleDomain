//-------------------------------------------------------------------------------
// <copyright file="BlockingJitneyTest.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class BlockingJitneyTest
    {
        private const int Value = 42;

        private readonly IRegisterTypes typeRegistrar;
        private readonly IResolveTypes typeResolver;
        private readonly JitneySubscriptions jitneySubscriptions;
        private readonly SimpleJitney testee;

        public BlockingJitneyTest()
        {
            this.typeRegistrar = A.Fake<IRegisterTypes>();
            this.typeResolver = A.Fake<IResolveTypes>();
            this.jitneySubscriptions = new JitneySubscriptions(this.typeRegistrar, this.typeResolver);
            this.testee = new SimpleJitney(this.jitneySubscriptions);
        }

        [Fact]
        public async Task CanSendCommand()
        {
            var composer = new CommandComposer();
            var command = new ValueCommand(Value);

            this.testee.Load(composer);

            await this.testee.SendAsync(command);

            composer.ValueFromCommand.Should().Be(Value);
        }

        [Fact]
        public async Task CanPublishEvent()
        {
            var composer = new EventComposer();
            var @event = new ValueEvent(Value);

            this.testee.Load(composer);

            await this.testee.PublishAsync(@event);

            composer.ValueFromEvent.Should().Be(Value);
        }

        private class CommandComposer : JitneyComposer
        {
            private readonly Func<ValueCommand, Task> handler;

            public CommandComposer()
            {
                this.handler = cmd => Task.Run(() => this.ValueFromCommand = cmd.Value);
            }

            public int ValueFromCommand { get; private set; }

            public override void Subscribe(ISubscribeHandlers bus)
            {
                bus.SubscribeCommandHandler(this.handler);
            }
        }

        private class EventComposer : JitneyComposer
        {
            private readonly Func<ValueEvent, Task> handler;

            public EventComposer()
            {
                this.handler = @event => Task.Run(() => this.ValueFromEvent = @event.Value);
            }

            public int ValueFromEvent { get; private set; }

            public override void Subscribe(ISubscribeHandlers bus)
            {
                bus.SubscribeEventHandler(this.handler);
            }
        }
    }
}