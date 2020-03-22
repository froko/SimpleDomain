//-------------------------------------------------------------------------------
// <copyright file="AbstractEventStoreConfiguration.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SimpleDomain.Common;

    /// <summary>
    /// The event store configuration base class
    /// </summary>
    public abstract class AbstractEventStoreConfiguration : IConfigureThisEventStore, IHaveEventStoreConfiguration
    {
        private readonly IDictionary<string, object> configurationItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventStoreConfiguration"/> class.
        /// </summary>
        protected AbstractEventStoreConfiguration()
        {
            this.configurationItems = new Dictionary<string, object>();
        }

        /// <inheritdoc />
        public Func<IEvent, Task> DispatchEvents { get; protected set; }

        /// <inheritdoc />
        public virtual void AddConfigurationItem(string key, object item)
        {
            if (this.configurationItems.ContainsKey(key))
            {
                return;
            }

            this.configurationItems.Add(key, item);
        }

        /// <summary>
        /// Defines how an event is dispatched
        /// </summary>
        /// <param name="dispatchEvents">The method to dispatch events</param>
        public void DefineAsyncEventDispatching(Func<IEvent, Task> dispatchEvents)
        {
            this.DispatchEvents = dispatchEvents;
        }

        /// <inheritdoc />
        public abstract void Register(Func<IHaveEventStoreConfiguration, IEventStore> createEventStore);

        /// <inheritdoc />
        public virtual T Get<T>(string key)
        {
            Guard.NotNullOrEmpty(() => key);

            if (!this.configurationItems.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            if (!(this.configurationItems[key] is T))
            {
                throw new InvalidCastException();
            }

            return (T)this.configurationItems[key];
        }
    }
}