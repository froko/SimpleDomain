//-------------------------------------------------------------------------------
// <copyright file="IConfigureThisEventStore.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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
        /// Registers an <see cref="IEventStore"/> instance
        /// <remarks>This method is intended for extension methods only</remarks>
        /// </summary>
        /// <param name="createEventStore">The action to create a <see cref="IEventStore"/> instance using a configuration</param>
        void Register(Func<IHaveEventStoreConfiguration, IEventStore> createEventStore);
    }
}