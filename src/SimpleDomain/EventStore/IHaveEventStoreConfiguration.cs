//-------------------------------------------------------------------------------
// <copyright file="IHaveEventStoreConfiguration.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The event store configuration interface
    /// </summary>
    public interface IHaveEventStoreConfiguration
    {
        /// <summary>
        /// Gets the async action how to dispatch events
        /// </summary>
        Func<IEvent, Task> DispatchEvents { get; }

        /// <summary>
        /// Gets a typed configuration item by its key
        /// </summary>
        /// <typeparam name="T">The type of the configuration item</typeparam>
        /// <param name="key">The key</param>
        /// <returns>A typed configuration item</returns>
        T Get<T>(string key);
    }
}