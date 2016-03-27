//-------------------------------------------------------------------------------
// <copyright file="ContractsToEndpointMapper.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using SimpleDomain.Common;

    /// <summary>
    /// The contracts to endpoint mapper
    /// </summary>
    public class ContractsToEndpointMapper : IMapContractsToEndpoints
    {
        private readonly EndpointAddress localEndpointAddress;
        private readonly IDictionary<Type, EndpointAddress> contractMap;
        private readonly Assembly contractAssembly;

        /// <summary>
        /// Creates a new instance of <see cref="ContractsToEndpointMapper"/>
        /// </summary>
        /// <param name="localEndpointAddress">The local endpoint</param>
        /// <param name="contractMap">The contract map dictionary</param>
        /// <param name="contractAssembly">The contract assembly</param>
        public ContractsToEndpointMapper(
            EndpointAddress localEndpointAddress,
            IDictionary<Type, EndpointAddress> contractMap,
            Assembly contractAssembly)
        {
            this.localEndpointAddress = localEndpointAddress;
            this.contractMap = contractMap;
            this.contractAssembly = contractAssembly;
        }

        /// <inheritdoc />
        public void To(string queueName)
        {
            this.To(new EndpointAddress(queueName));
        }

        /// <inheritdoc />
        public void To(string queueName, string machineName)
        {
            this.To(new EndpointAddress(queueName, machineName));
        }

        /// <inheritdoc />
        public void To(EndpointAddress remoteEndpointAddress)
        {
            Guard.NotNull(() => remoteEndpointAddress);
            this.GetMessageContracts().ForEach(messageType => this.AddToContractMap(messageType, remoteEndpointAddress));
        }

        /// <inheritdoc />
        public void ToMe()
        {
            this.GetMessageContracts().ForEach(messageType => this.AddToContractMap(messageType, this.localEndpointAddress));
        }

        private List<Type> GetMessageContracts()
        {
            return this.contractAssembly.GetTypes().Where(t => typeof(IMessage).IsAssignableFrom(t)).ToList();
        }

        private void AddToContractMap(Type messageType, EndpointAddress endpoint)
        {
            if (this.contractMap.Any(m => m.Key == messageType))
            {
                return;
            }

            this.contractMap.Add(messageType, endpoint);
        }
    }
}