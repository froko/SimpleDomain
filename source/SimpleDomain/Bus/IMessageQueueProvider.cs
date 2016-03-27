//-------------------------------------------------------------------------------
// <copyright file="IMessageQueueProvider.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The message queue provider interface
    /// </summary>
    public interface IMessageQueueProvider : IDisposable
    {
        /// <summary>
        /// Gets the name of the transport medium which this provider class implements
        /// </summary>
        string TransportMediumName { get; }

        /// <summary>
        /// Connects to the local queue
        /// </summary>
        /// <param name="localEndpointAddress">The local endpoint address</param>
        /// <param name="asyncEnvelopeReceivedCallback">An action which is called when an Envelope arrives</param>
        void Connect(EndpointAddress localEndpointAddress, Func<Envelope, Task> asyncEnvelopeReceivedCallback);

        /// <summary>
        /// Sends a message evelopped in an Envelope to a remote endpoint
        /// </summary>
        /// <param name="envelope">The enveloped message</param>
        Task SendAsync(Envelope envelope);
    }
}