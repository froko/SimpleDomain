//-------------------------------------------------------------------------------
// <copyright file="AbstractEventStoreConfiguration.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The EventStore configuration base class
    /// </summary>
    public abstract class AbstractEventStoreConfiguration : IHaveEventStoreConfiguration
    {
        private readonly IDictionary<string, object> configurationItems;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractEventStoreConfiguration"/>
        /// </summary>
        protected AbstractEventStoreConfiguration()
        {
            this.configurationItems = new Dictionary<string, object>();
            this.DispatchEvents = @event => Task.FromResult(0);
        }

        /// <inheritdoc />
        public Func<IEvent, Task> DispatchEvents { get; protected set; }

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
        public virtual void AddConfigurationItem(string key, object item)
        {
            this.configurationItems.Add(key, item);
        }

        /// <summary>
        /// Defines the action how to resolve a bus and asynchronously publish events over this bus
        /// </summary>
        /// <param name="dispatchEvents">The async resolve action</param>
        public void DefineAsyncEventDispatching(Func<IEvent, Task> dispatchEvents)
        {
            this.DispatchEvents = dispatchEvents;
        }

        /// <summary>
        /// Registers a specific implementation of <see cref="IEventStore"/> in the IoC container.
        /// <remarks>This method is intended for extension methods only</remarks>
        /// </summary>
        /// <typeparam name="TEventStore">The type of the EventStore</typeparam>
        public abstract void Register<TEventStore>() where TEventStore : IEventStore;
    }
}