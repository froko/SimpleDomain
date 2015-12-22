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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// The Jitney configuration base class
    /// </summary>
    public abstract class AbstractJitneyConfiguration : 
        IConfigureThisJitney,
        IHaveJitneyConfiguration
    {
        protected readonly JitneySubscriptions JitneySubscriptions;

        private readonly IDictionary<string, object> configurationItems;
        
        /// <summary>
        /// Creates a new instance of <see cref="AbstractJitneyConfiguration"/>
        /// </summary>
        /// <param name="handlerRegistry">Dependency injection for <see cref="AbstractHandlerRegistry"/></param>
        protected AbstractJitneyConfiguration(AbstractHandlerRegistry handlerRegistry)
        {
            this.configurationItems = new Dictionary<string, object>();
            
            this.JitneySubscriptions = new JitneySubscriptions(handlerRegistry, new HandlerInvocationCache());
            this.ContractMap = new Dictionary<Type, EndpointAddress>();
        }

        /// <inheritdoc />
        public IHaveJitneySubscriptions Subscriptions
        {
            get { return this.JitneySubscriptions; }
        }

        public IDictionary<Type, EndpointAddress> ContractMap { get; private set; }
        
        /// <inheritdoc />
        public T Get<T>(string key)
        {
            return (T)this.configurationItems[key];
        }

        /// <inheritdoc />
        public void AddConfigurationItem(string key, object item)
        {
            this.configurationItems.Add(key, item);
        }

        /// <inheritdoc />
        public void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
        {
            this.JitneySubscriptions.AddCommandHandler(handler);
        }

        /// <inheritdoc />
        public void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            this.JitneySubscriptions.AddEventHandler(handler);
        }

        /// <inheritdoc />
        public void SubscribeMessageHandlers(IEnumerable<Assembly> assemblies)
        {
            Action<Assembly> scanAssembly = assembly => 
                this.JitneySubscriptions.ScanAssemblyForMessageHandlers(assembly, this.RegisterHandlerType);

            assemblies.ToList().ForEach(scanAssembly);
        }

        /// <inheritdoc />
        public void SubscribeMessageHandlersInThisAssembly()
        {
            this.SubscribeMessageHandlers(new[] { Assembly.GetCallingAssembly() });
        }
        
        /// <inheritdoc />
        public abstract void Register<TJitney>() where TJitney : Jitney;

        /// <summary>
        /// Registers a handler type in the IoC container
        /// </summary>
        /// <param name="type">The handler type</param>
        protected abstract void RegisterHandlerType(Type type);
    }
}