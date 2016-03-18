//-------------------------------------------------------------------------------
// <copyright file="IConfigureThisJitney.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    
    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.Bus.Pipeline.Outgoing;

    public interface IConfigureThisJitney
    {
        /// <summary>
        /// Adds a configuration item
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="item">The item</param>
        void AddConfigurationItem(string key, object item);

        /// <summary>
        /// Defines the local endpoint address
        /// </summary>
        /// <param name="queueName">The name of the local endpoint queue</param>
        void DefineLocalEndpointAddress(string queueName);

        /// <summary>
        /// Sets the subscription store
        /// </summary>
        /// <param name="store">The subscription store</param>
        void SetSubscriptionStore(ISubscriptionStore store);

        /// <summary>
        /// Subscribes an async handler action for a given command
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="handler">The async handler action (must return a <see cref="Task"/>)</param>
        void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand;

        /// <summary>
        /// Subscribes an async handler action for a given event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="handler">The async handler action (must return a <see cref="Task"/>)</param>
        void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;

        /// <summary>
        /// Subscribes all message handlers in a given list of assemblies
        /// </summary>
        /// <param name="assemblies">The list of assemblies to scan and register for</param>
        void SubscribeMessageHandlers(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// Subscribes all message handlers in the calling assembly
        /// </summary>
        void SubscribeMessageHandlersInThisAssembly();

        /// <summary>
        /// Maps all message contracts in a given assembly
        /// </summary>
        /// <param name="contractAssembly">The contract assembly</param>
        /// <returns>An instance of <see cref="IMapContractsToEndpoints"/></returns>
        IMapContractsToEndpoints MapContracts(Assembly contractAssembly);

        /// <summary>
        /// Registers a specific type of <see cref="Jitney"/> in the IoC container.
        /// <remarks>This method is intended for extension methods only</remarks>
        /// </summary>
        /// <typeparam name="TJitney">The type of the <see cref="Jitney"/> bus</typeparam>
        void Register<TJitney>() where TJitney : Jitney;

        /// <summary>
        /// Adds a pipeline step for incomming envelopes
        /// </summary>
        /// <param name="pipelineStep">The pipeline step</param>
        void AddPipelineStep(IncommingEnvelopeStep pipelineStep);

        /// <summary>
        /// Adds a pipeline step for incomming messages
        /// </summary>
        /// <param name="pipelineStep">The pipeline step</param>
        void AddPipelineStep(IncommingMessageStep pipelineStep);

        /// <summary>
        /// Adds a pipeline step for outgoing messages
        /// </summary>
        /// <param name="pipelineStep">The pipeline step</param>
        void AddPipelineStep(OutgoingMessageStep pipelineStep);

        /// <summary>
        /// Adds a pipeline step for outgoing envelopes
        /// </summary>
        /// <param name="pipelineStep">The pipeline step</param>
        void AddPipelineStep(OutgoingEnvelopeStep pipelineStep);
    }
}