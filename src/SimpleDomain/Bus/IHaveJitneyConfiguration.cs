//-------------------------------------------------------------------------------
// <copyright file="IHaveJitneyConfiguration.cs" company="frokonet.ch">
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
    using System;
    using System.Threading.Tasks;

    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.Bus.Pipeline.Outgoing;

    /// <summary>
    /// The Jitney configuration holder interface
    /// </summary>
    public interface IHaveJitneyConfiguration : IHaveDynamicConfiguration
    {
        /// <summary>
        /// Gets the local endpoint address
        /// </summary>
        EndpointAddress LocalEndpointAddress { get; }

        /// <summary>
        /// Gets the Jitney subscriptions
        /// </summary>
        IHaveJitneySubscriptions Subscriptions { get; }

        /// <summary>
        /// Gets the subscription store persister interface
        /// </summary>
        ISaveSubscriptionMessages SubscriptionStore { get; }

        /// <summary>
        /// Creates the outgoing pipeline with all registered pipeline steps
        /// </summary>
        /// <param name="handleEnvelopeAsync">The last async action to be performed for an outgoing envelope</param>
        /// <returns>A new instance of <see cref="OutgoingPipeline"/></returns>
        OutgoingPipeline CreateOutgoingPipeline(Func<Envelope, Task> handleEnvelopeAsync);

        /// <summary>
        /// Creates the incomming pipeline with all registered pipeline steps
        /// </summary>
        /// <param name="handleCommandAsync">The async handling action if the incomming message is a command</param>
        /// <param name="handleEventAsync">The async handling action if the incomming message is an event</param>
        /// <param name="handleSubscriptionMessageAsync">The async handling action if the incomming message is a <see cref="SubscriptionMessage"/></param>
        /// <returns>A new instance of <see cref="IncommingPipeline"/></returns>
        IncommingPipeline CreateIncommingPipeline(
            Func<ICommand, Task> handleCommandAsync,
            Func<IEvent, Task> handleEventAsync,
            Func<SubscriptionMessage, Task> handleSubscriptionMessageAsync);

        /// <summary>
        /// Gets a user friendly formatted content summary of this configuration
        /// </summary>
        /// <param name="jitneyType">The type of <see cref="Jitney"/> using this configuration</param>
        /// <returns>A summary</returns>
        string GetSummary(Type jitneyType);
    }
}