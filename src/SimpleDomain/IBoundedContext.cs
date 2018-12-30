//-------------------------------------------------------------------------------
// <copyright file="IBoundedContext.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using SimpleDomain.Bus;

    /// <summary>
    /// The abstract technical definition of a bounded context
    /// </summary>
    public interface IBoundedContext
    {
        /// <summary>
        /// Gets the name of the bounded context
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Configures the bounded context
        /// <remarks>Derived classes will define the message subscriptions in this method</remarks>
        /// </summary>
        /// <param name="configuration">The message handler subscription configuration</param>
        /// <param name="featureSelector">The feature selector</param>
        /// <param name="bus">The message bus</param>
        /// <param name="repository">The repository</param>
        void Configure(
            ISubscribeMessageHandlers configuration,
            IFeatureSelector featureSelector,
            IDeliverMessages bus,
            IEventSourcedRepository repository);
    }
}