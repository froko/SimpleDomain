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
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    /// <summary>
    /// The Jitney configuration base class
    /// </summary>
    public abstract class AbstractJitneyConfiguration : IConfigureThisJitney, IHaveJitneyConfiguration
    {
        protected readonly JitneySubscriptions JitneySubscriptions;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractJitneyConfiguration"/>
        /// </summary>
        /// <param name="typeResolver">Dependency injection for <see cref="IResolveTypes"/></param>
        protected AbstractJitneyConfiguration(IResolveTypes typeResolver)
        {
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
                    throw new ConfigurationErrorsException(ExceptionMessages.LocalEndpointAddressNotDefined);
                }

                return this.EndpointAddress;
            }
        }

        /// <summary>
        /// Gets or sets the Endpoint address
        /// </summary>
        protected EndpointAddress EndpointAddress { get; set; }

        /// <inheritdoc />
        public abstract void Subscribe<TMessage, THandler>() where TMessage : IMessage where THandler : IHandleAsync<TMessage>;

        /// <inheritdoc />
        public void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
        {
            this.JitneySubscriptions.SubscribeCommandHandler(handler);
        }

        /// <inheritdoc />
        public void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            this.JitneySubscriptions.SubscribeEventHandler(handler);
        }

        /// <inheritdoc />
        public void DefineEndpointName(string endpointName)
        {
            this.EndpointAddress = new EndpointAddress(endpointName);
        }

        /// <inheritdoc />
        public abstract void Use<TJitney>() where TJitney : Jitney;
    }
}