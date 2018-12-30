//-------------------------------------------------------------------------------
// <copyright file="CorrelationIdIntegrationTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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

namespace SimpleDomain.Bus.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.Bus.Pipeline.Outgoing;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class CorrelationIdIntegrationTest
    {
        private readonly RegisterIncommingCorrelationIdStep registerIncommingCorrelationIdStep;
        private readonly RegisterOutgoingCorrelationIdStep registerOutgoingCorrelationIdStep;
        private readonly CompositionRoot compositionRoot;

        public CorrelationIdIntegrationTest()
        {
            this.registerIncommingCorrelationIdStep = new RegisterIncommingCorrelationIdStep();
            this.registerOutgoingCorrelationIdStep = new RegisterOutgoingCorrelationIdStep();
            this.compositionRoot = new CompositionRoot();
            this.compositionRoot.Register(new TestContext());
        }

        [Fact]
        public async Task ShouldUseSameCorrelationIdForAllMessagesInSameContext_WhenUsingMsmqJitney()
        {
            this.compositionRoot.ConfigureJitney()
                .DefineLocalEndpointAddress("simpledomain.correlationid.test")
                .AddPipelineStep(this.registerIncommingCorrelationIdStep)
                .AddPipelineStep(this.registerOutgoingCorrelationIdStep)
                .MapContracts(typeof(ValueCommand).Assembly).ToMe()
                .UseMsmqJitney();

            using (var context = await this.compositionRoot.StartAsync().ConfigureAwait(false))
            {
                // Wait for subscription message
                await WaitForMessageHandlerToExecute().ConfigureAwait(false);

                await context.Bus.SendAsync(new ValueCommand(42)).ConfigureAwait(false);
                await WaitForMessageHandlerToExecute().ConfigureAwait(false);

                var incommingCorrelationId = this.registerIncommingCorrelationIdStep.CorrelationId;
                var outgoingCorrelationIds = this.registerOutgoingCorrelationIdStep.CorrelationIds;

                outgoingCorrelationIds.Should().HaveCount(2);
                outgoingCorrelationIds.All(id => id == incommingCorrelationId).Should().BeTrue();
            }
        }

        private static async Task WaitForMessageHandlerToExecute()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
        }

        private class RegisterIncommingCorrelationIdStep : IncommingMessageStep
        {
            public override string Name => "Register Incomming CorrelationId Step";

            public Guid CorrelationId { get; private set; }

            public override Task InvokeAsync(IncommingMessageContext context, Func<Task> next)
            {
                if (context.Message is ValueCommand)
                {
                    this.CorrelationId = context.Envelope.CorrelationId;
                }

                return next();
            }
        }

        private class RegisterOutgoingCorrelationIdStep : OutgoingEnvelopeStep
        {
            private readonly IList<Guid> correlationIds = new List<Guid>();

            public override string Name => "Register Outgoing CorrelationId Step";

            public IReadOnlyCollection<Guid> CorrelationIds => this.correlationIds.ToList();

            public override Task InvokeAsync(OutgoingEnvelopeContext context, Func<Task> next)
            {
                if (context.Configuration.HasCorrelationId)
                {
                    var correlationId = context.Configuration.PeekCorrelationId();
                    this.correlationIds.Add(correlationId);
                }

                return next();
            }
        }

        private class TestContext : IBoundedContext
        {
            public string Name => "TestContext";

            public void Configure(
                ISubscribeMessageHandlers configuration,
                IFeatureSelector featureSelector,
                IDeliverMessages bus,
                IEventSourcedRepository repository)
            {
                configuration.SubscribeCommandHandler<ValueCommand>(c => HandleCommand(bus));
                configuration.SubscribeEventHandler<ValueEvent>(e => Task.CompletedTask);
            }

            private static async Task HandleCommand(IDeliverMessages bus)
            {
                await bus.PublishAsync(new ValueEvent(666)).ConfigureAwait(false);
                await bus.PublishAsync(new ValueEvent(999)).ConfigureAwait(false);
            }
        }
    }
}