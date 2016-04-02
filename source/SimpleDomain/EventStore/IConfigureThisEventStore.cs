//-------------------------------------------------------------------------------
// <copyright file="IConfigureThisEventStore.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The event store configuration interface
    /// </summary>
    public interface IConfigureThisEventStore
    {
        /// <summary>
        /// Adds a configuration item
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="item">The item</param>
        void AddConfigurationItem(string key, object item);

        /// <summary>
        /// Defines the action how to asynchronously publish events
        /// </summary>
        /// <param name="dispatchEvents">The async resolve action</param>
        void DefineAsyncEventDispatching(Func<IEvent, Task> dispatchEvents);

        /// <summary>
        /// Registers a specific implementation of <see cref="IEventStore"/> in the IoC container.
        /// <remarks>This method is intended for extension methods only</remarks>
        /// </summary>
        /// <typeparam name="TEventStore">The type of the EventStore</typeparam>
        void Register<TEventStore>() where TEventStore : IEventStore;
    }
}