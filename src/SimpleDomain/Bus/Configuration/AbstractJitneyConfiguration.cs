//-------------------------------------------------------------------------------
// <copyright file="AbstractJitneyConfiguration.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using SimpleDomain.Bus.Pipeline;
    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.Bus.Pipeline.Outgoing;
    using SimpleDomain.Common;

    /// <summary>
    /// The Jitney configuration base class
    /// </summary>
    public abstract class AbstractJitneyConfiguration :
        IConfigureThisJitney,
        ISubscribeMessageHandlers,
        IHaveJitneyConfiguration,
        IHavePipelineConfiguration
    {
        private readonly IDictionary<string, object> configurationItems;
        private readonly IDictionary<Type, EndpointAddress> contractMap;
        private readonly JitneySubscriptions jitneySubscriptions;

        private readonly IList<IncommingEnvelopeStep> incommingEnvelopeSteps;
        private readonly IList<IncommingMessageStep> incommingMessageSteps;
        private readonly IList<OutgoingMessageStep> outgoingMessageSteps;
        private readonly IList<OutgoingEnvelopeStep> outgoingEnvelopeSteps;
        private readonly Stack<Guid> correlationIds;

        private ISubscriptionStore subscriptionStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractJitneyConfiguration"/> class.
        /// </summary>
        /// <param name="handlerRegistry">Dependency injection for <see cref="AbstractHandlerRegistry"/></param>
        protected AbstractJitneyConfiguration(AbstractHandlerRegistry handlerRegistry)
        {
            this.configurationItems = new Dictionary<string, object>();
            this.contractMap = new Dictionary<Type, EndpointAddress>();
            this.jitneySubscriptions = new JitneySubscriptions(handlerRegistry, new HandlerInvocationCache());

            this.incommingEnvelopeSteps = new List<IncommingEnvelopeStep>();
            this.incommingMessageSteps = new List<IncommingMessageStep>();
            this.outgoingMessageSteps = new List<OutgoingMessageStep>();
            this.outgoingEnvelopeSteps = new List<OutgoingEnvelopeStep>();
            this.correlationIds = new Stack<Guid>();

            this.subscriptionStore = new InMemorySubscriptionStore();
        }

        /// <summary>
        /// Gets the local endpoint address
        /// </summary>
        public EndpointAddress LocalEndpointAddress { get; private set; }

        /// <inheritdoc />
        public IHaveJitneySubscriptions Subscriptions => this.jitneySubscriptions;

        /// <inheritdoc />
        public ISaveSubscriptionMessages SubscriptionStore => this.subscriptionStore;

        /// <inheritdoc />
        public bool HasCorrelationId => this.correlationIds.Any();

        /// <inheritdoc />
        public IConfigureThisJitney DefineLocalEndpointAddress(string queueName)
        {
            Guard.NotNullOrEmpty(() => queueName);

            this.LocalEndpointAddress = new EndpointAddress(queueName);
            return this;
        }

        /// <inheritdoc />
        public IConfigureThisJitney DefineLocalEndpointAddress(string queueName, string machineName)
        {
            Guard.NotNullOrEmpty(() => queueName);

            this.LocalEndpointAddress = new EndpointAddress(queueName, machineName);
            return this;
        }

        /// <inheritdoc />
        public IConfigureThisJitney SetSubscriptionStore(ISubscriptionStore store)
        {
            Guard.NotNull(() => store);

            this.subscriptionStore = store;
            return this;
        }

        /// <inheritdoc />
        public IMapContractsToEndpoints MapContracts(Assembly contractAssembly)
        {
            Guard.NotNull(() => contractAssembly);

            if (this.LocalEndpointAddress == null)
            {
                throw new JitneyConfigurationException(ExceptionMessages.LocalEndpointAddressNotDefined);
            }

            return new ContractsToEndpointMapper(this, this.LocalEndpointAddress, this.contractMap, contractAssembly);
        }

        /// <inheritdoc />
        public IConfigureThisJitney AddPipelineStep(IncommingEnvelopeStep pipelineStep)
        {
            Guard.NotNull(() => pipelineStep);

            if (!this.incommingEnvelopeSteps.Contains(pipelineStep))
            {
                this.incommingEnvelopeSteps.Add(pipelineStep);
            }

            return this;
        }

        /// <inheritdoc />
        public IConfigureThisJitney AddPipelineStep(IncommingMessageStep pipelineStep)
        {
            Guard.NotNull(() => pipelineStep);

            if (!this.incommingMessageSteps.Contains(pipelineStep))
            {
                this.incommingMessageSteps.Add(pipelineStep);
            }

            return this;
        }

        /// <inheritdoc />
        public IConfigureThisJitney AddPipelineStep(OutgoingMessageStep pipelineStep)
        {
            Guard.NotNull(() => pipelineStep);

            if (!this.outgoingMessageSteps.Contains(pipelineStep))
            {
                this.outgoingMessageSteps.Add(pipelineStep);
            }

            return this;
        }

        /// <inheritdoc />
        public IConfigureThisJitney AddPipelineStep(OutgoingEnvelopeStep pipelineStep)
        {
            Guard.NotNull(() => pipelineStep);

            if (!this.outgoingEnvelopeSteps.Contains(pipelineStep))
            {
                this.outgoingEnvelopeSteps.Add(pipelineStep);
            }

            return this;
        }

        /// <inheritdoc />
        public void AddConfigurationItem(string key, object item)
        {
            if (this.configurationItems.ContainsKey(key))
            {
                return;
            }

            this.configurationItems.Add(key, item);
        }

        /// <inheritdoc />
        public abstract void Register(Func<IHaveJitneyConfiguration, Jitney> createJitney);

        /// <inheritdoc />
        public void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
        {
            Guard.NotNull(() => handler);

            this.jitneySubscriptions.AddCommandHandler(handler);
        }

        /// <inheritdoc />
        public void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Guard.NotNull(() => handler);

            this.jitneySubscriptions.AddEventHandler(handler);
        }

        /// <inheritdoc />
        public OutgoingPipeline CreateOutgoingPipeline(Func<Envelope, Task> handleEnvelopeAsync)
        {
            return new OutgoingPipeline(
                this,
                this.outgoingMessageSteps.WithFinalOutgoingMessageStep(),
                this.outgoingEnvelopeSteps.WithFinalOutgoingEnvelopeStep(handleEnvelopeAsync));
        }

        /// <inheritdoc />
        public IncommingPipeline CreateIncommingPipeline(
            Func<ICommand, Task> handleCommandAsync,
            Func<IEvent, Task> handleEventAsync,
            Func<SubscriptionMessage, Task> handleSubscriptionMessageAsync)
        {
            return new IncommingPipeline(
                this,
                this.incommingEnvelopeSteps.WithFinalIncommingEnvelopeStep(),
                this.incommingMessageSteps.WithFinalIncommingMessageStep(handleCommandAsync, handleEventAsync, handleSubscriptionMessageAsync));
        }

        /// <inheritdoc />
        public string GetSummary(Type jitneyType)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"This is {jitneyType.Name} with the following configuration values:");
            stringBuilder.AppendLine($"Local endpoint name: {this.LocalEndpointAddress}");

            if (typeof(MessageQueueJitney).IsAssignableFrom(jitneyType))
            {
                stringBuilder.AppendLine($"Subscription store: {this.SubscriptionStore.GetType().Name}");
            }

            AppendPipelineSteps(stringBuilder, "Outgoing message steps", this.outgoingMessageSteps.WithFinalOutgoingMessageStep());
            AppendPipelineSteps(stringBuilder, "Outgoing envelope steps", this.outgoingEnvelopeSteps.WithFinalOutgoingEnvelopeStep(null));
            AppendPipelineSteps(stringBuilder, "Incomming envelope steps", this.incommingEnvelopeSteps.WithFinalIncommingEnvelopeStep());
            AppendPipelineSteps(stringBuilder, "Incomming message steps", this.incommingMessageSteps.WithFinalIncommingMessageStep(null, null, null));

            if (this.configurationItems.Any())
            {
                stringBuilder.AppendLine("\r\nConfiguration items:");
                foreach (var configurationItem in this.configurationItems)
                {
                    stringBuilder.AppendLine($"- {configurationItem.Key}: {configurationItem.Value}");
                }
            }

            return stringBuilder.ToString();
        }

        /// <inheritdoc />
        public EndpointAddress GetConsumingEndpointAddress(ICommand command)
        {
            return this.contractMap[command.GetType()];
        }

        /// <inheritdoc />
        public IEnumerable<EndpointAddress> GetSubscribedEndpointAddresses(IEvent @event)
        {
            return this.subscriptionStore.GetSubscribedEndpoints(@event);
        }

        /// <inheritdoc />
        public EndpointAddress GetPublishingEndpointAddress(string fullNameOfEventType)
        {
            return this.contractMap[TypeHelper.GetType(fullNameOfEventType)];
        }

        /// <inheritdoc />
        public T Get<T>(string key)
        {
            Guard.NotNullOrEmpty(() => key);

            if (!this.configurationItems.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            if (!(this.configurationItems[key] is T))
            {
                throw new InvalidCastException();
            }

            return (T)this.configurationItems[key];
        }

        /// <inheritdoc />
        public void PushCorrelationId(Guid correlationId)
        {
            this.correlationIds.Push(correlationId);
        }

        /// <inheritdoc />
        public void PopCorrelationId()
        {
            this.correlationIds.Pop();
        }

        /// <inheritdoc />
        public Guid PeekCorrelationId()
        {
            return this.correlationIds.Peek();
        }

        /// <summary>
        /// Subscribes all message handlers in a given list of assemblies
        /// </summary>
        /// <param name="assemblies">The list of assemblies to scan and register for</param>
        /// <param name="registerInContainer">The action to register types in the IoC container</param>
        protected void SubscribeMessageHandlers(IEnumerable<Assembly> assemblies, Action<Type> registerInContainer)
        {
            Action<Assembly> scanAssembly = assembly =>
                this.jitneySubscriptions.ScanAssemblyForMessageHandlers(assembly, registerInContainer);

            assemblies.ToList().ForEach(scanAssembly);
        }

        /// <summary>
        /// Subscribes all message handlers in the calling assembly
        /// </summary>
        /// <param name="registerInContainer">The action to register types in the IoC container</param>
        protected void SubscribeMessageHandlersInThisAssembly(Action<Type> registerInContainer)
        {
            this.SubscribeMessageHandlers(new[] { Assembly.GetCallingAssembly() }, registerInContainer);
        }

        private static void AppendPipelineSteps<T>(
            StringBuilder stringBuilder,
            string header,
            IEnumerable<PipelineStep<T>> pipelineSteps) where T : PipelineContext
        {
            var pipelineStepCounter = 1;

            stringBuilder.AppendLine($"\r\n{header}:");
            foreach (var pipelineStep in pipelineSteps)
            {
                stringBuilder.AppendLine($"{pipelineStepCounter++}: {pipelineStep.Name}");
            }
        }
    }
}