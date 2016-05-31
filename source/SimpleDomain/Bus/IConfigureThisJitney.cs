//-------------------------------------------------------------------------------
// <copyright file="IConfigureThisJitney.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Reflection;

    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.Bus.Pipeline.Outgoing;

    /// <summary>
    /// The Jitney configuration interface
    /// </summary>
    public interface IConfigureThisJitney
    {
        /// <summary>
        /// Defines the local endpoint address
        /// </summary>
        /// <param name="queueName">The name of the local endpoint queue</param>
        /// <returns>The instance of the class implementing this interface since this is a fluent interface</returns>
        IConfigureThisJitney DefineLocalEndpointAddress(string queueName);
        
        /// <summary>
        /// Sets the subscription store
        /// </summary>
        /// <param name="store">An instance of <see cref="ISubscriptionStore"/></param>
        /// <returns>The instance of the class implementing this interface since this is a fluent interface</returns>
        IConfigureThisJitney SetSubscriptionStore(ISubscriptionStore store);

        /// <summary>
        /// Maps all message contracts in a given assembly
        /// </summary>
        /// <param name="contractAssembly">The contract assembly</param>
        /// <returns>An instance of <see cref="IMapContractsToEndpoints"/></returns>
        IMapContractsToEndpoints MapContracts(Assembly contractAssembly);

        /// <summary>
        /// Adds a pipeline step for incomming envelopes
        /// </summary>
        /// <param name="pipelineStep">The pipeline step</param>
        /// <returns>The instance of the class implementing this interface since this is a fluent interface</returns>
        IConfigureThisJitney AddPipelineStep(IncommingEnvelopeStep pipelineStep);

        /// <summary>
        /// Adds a pipeline step for incomming messages
        /// </summary>
        /// <param name="pipelineStep">The pipeline step</param>
        /// <returns>The instance of the class implementing this interface since this is a fluent interface</returns>
        IConfigureThisJitney AddPipelineStep(IncommingMessageStep pipelineStep);

        /// <summary>
        /// Adds a pipeline step for outgoing messages
        /// </summary>
        /// <param name="pipelineStep">The pipeline step</param>
        /// <returns>The instance of the class implementing this interface since this is a fluent interface</returns>
        IConfigureThisJitney AddPipelineStep(OutgoingMessageStep pipelineStep);

        /// <summary>
        /// Adds a pipeline step for outgoing envelopes
        /// </summary>
        /// <returns>The instance of the class implementing this interface since this is a fluent interface</returns>
        IConfigureThisJitney AddPipelineStep(OutgoingEnvelopeStep pipelineStep);

        /// <summary>
        /// Adds a configuration item
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="item">The item</param>
        void AddConfigurationItem(string key, object item);

        /// <summary>
        /// Registers a <see cref="Jitney" /> instance
        /// <remarks>This method is intended for extension methods only</remarks>
        /// </summary>
        /// <param name="createJitney">The action to create a <see cref="Jitney" /> instance using a configuration</param>
        void Register(Func<IHaveJitneyConfiguration, Jitney> createJitney);
    }
}