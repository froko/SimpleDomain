//-------------------------------------------------------------------------------
// <copyright file="IHavePipelineConfiguration.cs" company="frokonet.ch">
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
    using System.Collections.Generic;

    /// <summary>
    /// The pipeline configuration holder interface
    /// </summary>
    public interface IHavePipelineConfiguration
    {
        /// <summary>
        /// Gets the local endpoint address
        /// </summary>
        EndpointAddress LocalEndpointAddress { get; }

        /// <summary>
        /// Gets a value indicating whether the correlation id stak is not empty
        /// </summary>
        bool HasCorrelationId { get; }

        /// <summary>
        /// Gets the address of the consuming endpoint for a given command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>The address of the consuming endpoint</returns>
        EndpointAddress GetConsumingEndpointAddress(ICommand command);

        /// <summary>
        /// Gets a list of endpoint addresses for the subscribed enpoints of a given event
        /// </summary>
        /// <param name="event">The event</param>
        /// <returns>A list of endpint addresses for the subscribed endpoints</returns>
        IEnumerable<EndpointAddress> GetSubscribedEndpointAddresses(IEvent @event);

        /// <summary>
        /// Gets the address of the publishing endpoint for a given event type
        /// </summary>
        /// <param name="fullNameOfEventType">The full name of the event type</param>
        /// <returns>The address of the publishing endpoint</returns>
        EndpointAddress GetPublishingEndpointAddress(string fullNameOfEventType);

        /// <summary>
        /// Pushes the correlation id of an incomming message to the stack
        /// </summary>
        /// <param name="correlationId">The correlation id of the incomming message</param>
        void PushCorrelationId(Guid correlationId);

        /// <summary>
        /// Pops the current correlation id from the stak
        /// </summary>
        void PopCorrelationId();

        /// <summary>
        /// Peeks the current correlation id from the stak
        /// </summary>
        /// <returns>The current correlation id</returns>
        Guid PeekCorrelationId();
    }
}