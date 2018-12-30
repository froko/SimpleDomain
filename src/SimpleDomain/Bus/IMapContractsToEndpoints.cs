//-------------------------------------------------------------------------------
// <copyright file="IMapContractsToEndpoints.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    /// <summary>
    /// The contract to enpoint mapper interface
    /// </summary>
    public interface IMapContractsToEndpoints
    {
        /// <summary>
        /// Maps contracts to a given endpoint on the local machine
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>The Jitney bus configuration itself since this is a builder method</returns>
        IConfigureThisJitney To(string queueName);

        /// <summary>
        /// Maps contracts to a given endpoint
        /// </summary>
        /// <param name="queueName">The name of the endpoint</param>
        /// <param name="machineName">The server name where the endpoint resides</param>
        /// <returns>The Jitney bus configuration itself since this is a builder method</returns>
        IConfigureThisJitney To(string queueName, string machineName);

        /// <summary>
        /// Maps contracts to a given endpoint
        /// </summary>
        /// <param name="remoteEndpointAddress">The endpoint</param>
        /// <returns>The Jitney bus configuration itself since this is a builder method</returns>
        IConfigureThisJitney To(EndpointAddress remoteEndpointAddress);

        /// <summary>
        /// Maps contracts to the calling endpoint itself
        /// </summary>
        /// <returns>The Jitney bus configuration itself since this is a builder method</returns>
        IConfigureThisJitney ToMe();
    }
}