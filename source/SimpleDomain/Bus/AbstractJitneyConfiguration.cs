//-------------------------------------------------------------------------------
// <copyright file="AbstractJitneyConfiguration.cs" company="frokonet.ch">
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
    using System.Collections.Generic;

    /// <summary>
    /// The Jitney configuration base class
    /// </summary>
    public abstract class AbstractJitneyConfiguration : IHaveJitneyConfiguration
    {
        protected readonly JitneySubscriptions JitneySubscriptions;

        private readonly IDictionary<string, object> configurationItems;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractJitneyConfiguration"/>
        /// </summary>
        /// <param name="typeResolver">Dependency injection for <see cref="IResolveTypes"/></param>
        protected AbstractJitneyConfiguration(IResolveTypes typeResolver)
        {
            this.configurationItems = new Dictionary<string, object>();
            this.JitneySubscriptions = new JitneySubscriptions(typeResolver);
            this.EndpointAddress = null;
        }

        /// <inheritdoc />
        public IHaveJitneySubscriptions HandlerSubscriptions
        {
            get { return this.JitneySubscriptions; }
        }

        /// <inheritdoc />
        public EndpointAddress LocalEndpointAddress
        {
            get
            {
                if (this.EndpointAddress == null)
                {
                    throw new MissingConfigurationException(ExceptionMessages.LocalEndpointAddressNotDefined);
                }

                return this.EndpointAddress;
            }
        }

        /// <summary>
        /// Gets or sets the Endpoint address
        /// </summary>
        protected EndpointAddress EndpointAddress { get; set; }

        /// <inheritdoc />
        public T Get<T>(string key)
        {
            return (T)this.configurationItems[key];
        }

        /// <summary>
        /// Adds a configuration item
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="item">The item</param>
        public void AddConfigurationItem(string key, object item)
        {
            this.configurationItems.Add(key, item);
        }

        /// <summary>
        /// Defines the endpoint name for this bus
        /// </summary>
        /// <param name="endpointName">The endpoint name</param>
        public void DefineEndpointName(string endpointName)
        {
            this.EndpointAddress = new EndpointAddress(endpointName);
        }

        /// <summary>
        /// Registers a specific type of <see cref="Jitney"/> in the IoC container.
        /// <remarks>This method is intended for extension methods only</remarks>
        /// </summary>
        /// <typeparam name="TJitney">The type of the <see cref="Jitney"/> bus</typeparam>
        public abstract void Register<TJitney>() where TJitney : Jitney;
    }
}