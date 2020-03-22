﻿//-------------------------------------------------------------------------------
// <copyright file="IntegrationTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
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

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class IntegrationTest : IBoundedContext
    {
        private bool commandWasHandled;
        private bool eventWasHandled;

        public string Name => "IntegrationTest";

#pragma warning disable xUnit1013 // Public method should be marked as test
        public void Configure(
#pragma warning restore xUnit1013 // Public method should be marked as test
            ISubscribeMessageHandlers configuration,
            IFeatureSelector featureSelector,
            IDeliverMessages bus,
            IEventSourcedRepository repository)
        {
            configuration.SubscribeCommandHandler<MyCommand>(this.HandleMyCommand);
            configuration.SubscribeEventHandler<MyEvent>(this.HandleMyEvent);
        }

        [Fact(Skip = "Needs connection to local RabbitMQ server instance")]
        public async Task CanSendAndReceiveCommandsWithRabbitMq()
        {
            var compositionRoot = new CompositionRoot();

            compositionRoot.ConfigureJitney()
                .DefineLocalEndpointAddress("simpledomain.integrationtest")
                .MapContracts(typeof(MyCommand).Assembly).ToMe()
                .UseRabbitMqJitney();

            compositionRoot.Register(this);

            using (var executionContext = await compositionRoot.StartAsync().ConfigureAwait(false))
            {
                await executionContext.Bus.SendAsync(new MyCommand()).ConfigureAwait(false);
                await WaitForMessageHandlerToExecute().ConfigureAwait(false);

                this.commandWasHandled.Should().BeTrue();
            }
        }

        [Fact(Skip = "Needs connection to local RabbitMQ server instance")]
        public async Task CanSendAndReceiveEventsWithRabbitMq()
        {
            var compositionRoot = new CompositionRoot();

            compositionRoot.ConfigureJitney()
                .DefineLocalEndpointAddress("simpledomain.integrationtest")
                .MapContracts(typeof(MyEvent).Assembly).ToMe()
                .UseRabbitMqJitney();

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