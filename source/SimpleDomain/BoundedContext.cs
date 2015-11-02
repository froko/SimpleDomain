//-------------------------------------------------------------------------------
// <copyright file="BoundedContext.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using SimpleDomain.Bus;

    /// <summary>
    /// The abstract technical definition of a bounded context
    /// </summary>
    public abstract class BoundedContext
    {
        /// <summary>
        /// Gets the name of the bounded context
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Configures the bounded context
        /// <remarks>Derived classes will define the message subscriptions in this method</remarks>
        /// </summary>
        /// <param name="bus">The Jitney bus</param>
        /// <param name="repository">The repository</param>
        public abstract void Configure(Jitney bus, IEventSourcedRepository repository);
    }
}