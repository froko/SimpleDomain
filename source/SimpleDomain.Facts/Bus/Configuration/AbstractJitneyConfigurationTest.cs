//-------------------------------------------------------------------------------
// <copyright file="AbstractJitneyConfigurationTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.Bus.Pipeline.Outgoing;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class AbstractJitneyConfigurationTest
    {
        private readonly IContainer container;
        private readonly AbstractJitneyConfiguration testee;

        public AbstractJitneyConfigurationTest()
        {
            this.container = A.Fake<IContainer>();
            this.testee = new JitneyConfiguration(A.Fake<AbstractHandlerRegistry>(), this.container);
        }

        [Fact]
        public void CanAddAndGetConfigurationItem()
        {
            this.testee.AddConfigurationItem("Foo", new ConfigurationItem());

            var configurationItem = this.testee.Get<ConfigurationItem>("Foo");

            configurationItem.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsException_WhenTryingToAddConfigurationItemWithNullAsKey()
        {
            Action action = () => this.testee.AddConfigurationItem(null, new ConfigurationItem());

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToGetConfigurationItemWithNullAsKey()
        {
            Action action = () => this.testee.Get<string>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenConfigurationItemTypeIsNotOfGivenType()
        {
            this.testee.AddConfigurationItem("Foo", new ConfigurationItem());

            Action action = () => this.testee.Get<string>("Foo");

            action.ShouldThrow<InvalidCastException>();
        }

        [Fact]
        public void ThrowsException_WhenConfigurationItemKeyIsNotFound()
        {
            Action action = () => this.testee.Get<string>("NotExistingKey");

            action.ShouldThrow<KeyNotFoundException>();
        }

        [Fact]
        public void CanDefineLocalEndpointAddress()
        {
            this.testee.DefineLocalEndpointAddress("myEndpoint");

            this.testee.LocalEndpointAddress.IsLocal.Should().BeTrue();
            this.testee.LocalEndpointAddress.QueueName.Should().Be("myEndpoint");
        }

        [Fact]
        public void CanSubscribeCommandHandler()
        {
            var handler = new ValueCommandHandler();
            this.testee.SubscribeCommandHandler<ValueCommand>(handler.HandleAsync);

            var subscription = this.testee.Subscriptions.GetCommandSubscription(new ValueCommand(11));

            subscription.Should().BeAssignableTo<CommandSubscription<ValueCommand>>();
        }

        [Fact]
        public void CanSubscribeEventHandler()
        {
            var handler = new ValueEventHandler();
            this.testee.SubscribeEventHandler<ValueEvent>(handler.HandleAsync);

            var subscriptions = this.testee.Subscriptions.GetEventSubscriptions(new ValueEvent(11));

            subscriptions.Single().Should().BeAssignableTo<EventSubscription<ValueEvent>>();
        }

        [Fact]
        public void CanSubscribeMessageHandlers()
        {
            var handlerAssemblies = new[] { typeof(ValueCommandHandler).Assembly };
            this.testee.SubscribeMessageHandlers(handlerAssemblies);
            
            A.CallTo(() => this.container.Register(typeof(ValueCommandHandler))).MustHaveHappened();
            A.CallTo(() => this.container.Register(typeof(ValueEventHandler))).MustHaveHappened();
        }

        [Fact]
        public void CanSubscribeMessageHandlersInThisAssembly()
        {
            this.testee.SubscribeMessageHandlersInThisAssembly();

            A.CallTo(() => this.container.Register(typeof(ValueCommandHandler))).MustHaveHappened();
            A.CallTo(() => this.container.Register(typeof(ValueEventHandler))).MustHaveHappened();
        }

        [Fact]
        public void CanRegisterJitney()
        {
            this.testee.Register<SimpleJitney>();

            A.CallTo(() => this.container.Register<SimpleJitney>()).MustHaveHappened();
        }

        [Fact]
        public async Task CanAddIncommingEnvelopePipelineStep()
        {
            var pipelineStep = A.Fake<IncommingEnvelopeStep>();
            this.testee.AddPipelineStep(pipelineStep);

            var pipeline = this.testee.CreateIncommingPipeline(c => Task.CompletedTask, e => Task.CompletedTask, m => Task.CompletedTask);
            await pipeline.InvokeAsync(A.Fake<Envelope>());

            A.CallTo(() => pipelineStep.InvokeAsync(A<IncommingEnvelopeContext>.Ignored, A<Func<Task>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task CanAddIncommingMessagePipelineStep()
        {
            var pipelineStep = A.Fake<IncommingMessageStep>();
            this.testee.AddPipelineStep(pipelineStep);

            var envelope = new Envelope(new Dictionary<string, object> { { HeaderKeys.Sender, "sender@localhost" } }, new MyCommand());

            var pipeline = this.testee.CreateIncommingPipeline(c => Task.CompletedTask, e => Task.CompletedTask, m => Task.CompletedTask);
            await pipeline.InvokeAsync(envelope);

            A.CallTo(() => pipelineStep.InvokeAsync(A<IncommingMessageContext>.Ignored, A<Func<Task>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task CanAddOutgoingMessagePipelineStep()
        {
            this.testee.DefineLocalEndpointAddress("sender");

            var pipelineStep = A.Fake<OutgoingMessageStep>();
            this.testee.AddPipelineStep(pipelineStep);

            var pipeline = this.testee.CreateOutgoingPipeline(e => Task.CompletedTask);
            await pipeline.InvokeAsync(A.Fake<IMessage>());

            A.CallTo(() => pipelineStep.InvokeAsync(A<OutgoingMessageContext>.Ignored, A<Func<Task>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task CanAddOutgoingEnvelopePipelineStep()
        {
            this.testee.DefineLocalEndpointAddress("sender");
            this.testee.MapContracts(typeof(ValueCommand).Assembly).ToMe();

            var pipelineStep = A.Fake<OutgoingEnvelopeStep>();
            this.testee.AddPipelineStep(pipelineStep);

            var pipeline = this.testee.CreateOutgoingPipeline(e => Task.CompletedTask);
            await pipeline.InvokeAsync(new ValueCommand(11));

            A.CallTo(() => pipelineStep.InvokeAsync(A<OutgoingEnvelopeContext>.Ignored, A<Func<Task>>.Ignored)).MustHaveHappened();
        }

        private class JitneyConfiguration : AbstractJitneyConfiguration
        {
            private readonly IContainer container;

            public JitneyConfiguration(AbstractHandlerRegistry handlerRegistry, IContainer container)
                : base(handlerRegistry)
            {
                this.container = container;
            }

            public override void Register<TJitney>()
            {
                this.container.Register<TJitney>();
            }

            protected override void RegisterHandlerType(Type type)
            {
                this.container.Register(type);
            }
        }

        private class ConfigurationItem
        {
        }
    }
}