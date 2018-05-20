//-------------------------------------------------------------------------------
// <copyright file="AbstractFeature.cs" company="frokonet.ch">
//   Copyright (c) 2014-2018
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

namespace SimpleDomain
{
    using SimpleDomain.Bus.Configuration;
    using SimpleDomain.EventStore.Configuration;

    /// <summary>
    /// The abstract class to extend the functionality of SimpleDomain
    /// </summary>
    public abstract class AbstractFeature
    {
        /// <summary>
        /// Gets or sets the name of the feature
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Configures the feature using bus
        /// </summary>
        /// <param name="configuration">The Jitney configuration</param>
        /// <param name="bus">The bus</param>
        public virtual void Configure(AbstractJitneyConfiguration configuration, IDeliverMessages bus)
        {
        }

        /// <summary>
        /// Configures the feature using the event store
        /// </summary>
        /// <param name="configuration">The event store configuration</param>
        /// <param name="repository">The event store repository</param>
        public virtual void Configure(AbstractEventStoreConfiguration configuration, IEventSourcedRepository repository)
        {
        }
    }
}