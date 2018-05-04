//-------------------------------------------------------------------------------
// <copyright file="IntegrationTest.cs" company="frokonet.ch">
//   Copyright (c) 2014-2018
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
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.Bus.Pipeline;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class IntegrationTest : IBoundedContext
    {
        private bool commandWasHandled;
        private bool eventWasHandled;

        public string Name => "IntegrationTest";

        public void Configure(ISubscribeMessageHandlers configuration, IDeliverMessages bus, IEventSourcedRepository repository)
        {
            configuration.SubscribeCommandHandler<MyCommand>(this.HandleMyCommand);
            configuration.SubscribeEventHandler<MyEvent>(this.HandleMyEvent);
        }

        [Fact]
        public async Task CanSendAndReceiveCommandsWithMsmq()
        {
            var compositionRoot = new CompositionRoot();

            compositionRoot.ConfigureJitney()
                .DefineLocalEndpointAddress("simpledomain.integrationtest")
                .AddPipelineStep(new AuditQueueStep("simpledomain.integrationtest.audit"))
                .MapContracts(typeof(MyCommand).Assembly).ToMe()
                .UseMsmqJitney();

            compositionRoot.Register(this);

            using (var executionContext = await compositionRoot.StartAsync().ConfigureAwait(false))
            {
                await executionContext.Bus.SendAsync(new MyCommand()).ConfigureAwait(false);
                await WaitForMessageHandlerToExecute().ConfigureAwait(false);

                this.commandWasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public async Task CanSendAndReceiveEventsWithMsmq()
        {
            var compositionRoot = new CompositionRoot();

            compositionRoot.ConfigureJitney()
                .DefineLocalEndpointAddress("simpledomain.integrationtest")
                .AddPipelineStep(new AuditQueueStep("simpledomain.integrationtest.audit"))
                .MapContracts(typeof(MyEvent).Assembly).ToMe()
                .UseMsmqJitney();

            compositionRoot.Register(this);

            using (var executionContext = await compositionRoot.StartAsync().ConfigureAwait(false))
            {
                // Wait for subscription message
                await WaitForMessageHandlerToExecute().ConfigureAwait(false);

                await executionContext.Bus.PublishAsync(new MyEvent()).ConfigureAwait(false);
                await WaitForMessageHandlerToExecute().ConfigureAwait(false);

                this.eventWasHandled.Should().BeTrue();
            }
        }

        private static async Task WaitForMessageHandlerToExecute()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
        }

        private Task HandleMyCommand(MyCommand arg)
        {
            this.commandWasHandled = true;
            return Task.CompletedTask;
        }

        private Task HandleMyEvent(MyEvent arg)
        {
            this.eventWasHandled = true;
            return Task.CompletedTask;
        }
    }
}