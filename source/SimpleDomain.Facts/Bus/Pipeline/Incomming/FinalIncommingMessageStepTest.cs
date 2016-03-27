//-------------------------------------------------------------------------------
// <copyright file="FinalIncommingMessageStepTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Incomming
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

    public class FinalIncommingMessageStepTest : IDisposable
    {
        private readonly Func<ICommand, Task> finalActionForCommand;
        private readonly Func<IEvent, Task> finalActionForEvent;
        private readonly Func<SubscriptionMessage, Task> finalActionForSubscriptionMessage;
        private readonly FinalIncommingMessageStep testee;

        public FinalIncommingMessageStepTest()
        {
            Trace.Listeners.Add(InMemoryTraceListener.Instance);

            this.finalActionForCommand = A.Fake<Func<ICommand, Task>>();
            this.finalActionForEvent = A.Fake<Func<IEvent, Task>>();
            this.finalActionForSubscriptionMessage = A.Fake<Func<SubscriptionMessage, Task>>();

            this.testee = new FinalIncommingMessageStep(
                this.finalActionForCommand,
                this.finalActionForEvent,
                this.finalActionForSubscriptionMessage);
        }

        public void Dispose()
        {
            InMemoryTraceListener.ClearLogMessages();
            Trace.Listeners.Remove(InMemoryTraceListener.Instance);
        }

        [Fact]
        public async Task ShouldInvokeGivenFinalActionForCommand()
        {
            var message = new ValueCommand(11);
            var incommingMessageContext = CreateIncommingMessageContext(message);
            
            await this.testee.InvokeAsync(incommingMessageContext, null);

            A.CallTo(() => this.finalActionForCommand.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task ShouldInvokeGivenFinalActionForEvent()
        {
            var message = new ValueEvent(11);
            var incommingMessageContext = CreateIncommingMessageContext(message);

            await this.testee.InvokeAsync(incommingMessageContext, null);

            A.CallTo(() => this.finalActionForEvent.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task ShouldInvokeGivenFinalActionForSubscriptionMessage()
        {
            var message = new SubscriptionMessage(new EndpointAddress("recipient"), typeof(ValueCommand).FullName);
            var incommingMessageContext = CreateIncommingMessageContext(message);

            await this.testee.InvokeAsync(incommingMessageContext, null);

            A.CallTo(() => this.finalActionForSubscriptionMessage.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task DoesNotCallNext()
        {
            var message = new ValueCommand(11);
            var incommingMessageContext = CreateIncommingMessageContext(message);
            var next = A.Fake<Func<Task>>();
            
            await this.testee.InvokeAsync(incommingMessageContext, next);

            A.CallTo(() => next.Invoke()).MustNotHaveHappened();
        }

        [Fact]
        public async Task LogsReceptionOfMessage()
        {
            var message = new ValueCommand(11);
            var incommingMessageContext = CreateIncommingMessageContext(message);
            
            await this.testee.InvokeAsync(incommingMessageContext, null);

            "Received Command of type SimpleDomain.TestDoubles.ValueCommand from sender".Should().HaveBeenLogged().WithInfoLevel();
        }

        private static IncommingMessageContext CreateIncommingMessageContext(IMessage message)
        {
            var headers = new Dictionary<string, object> { { HeaderKeys.Sender, new EndpointAddress("sender") } };
            return new IncommingMessageContext(message, headers, A.Fake<IHavePipelineConfiguration>());
        }
    }
}