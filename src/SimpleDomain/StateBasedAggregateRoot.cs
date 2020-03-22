//-------------------------------------------------------------------------------
// <copyright file="StateBasedAggregateRoot.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    /// <summary>
    /// Base class for all state based aggregate roots
    /// </summary>
    /// <typeparam name="TState">The type of the state holding entity</typeparam>
    public abstract class StateBasedAggregateRoot<TState> : AggregateRoot where TState : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateBasedAggregateRoot{TState}"/> class.
        /// </summary>
        protected StateBasedAggregateRoot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateBasedAggregateRoot{TState}"/> class.
        /// </summary>
        /// <param name="state">Dependency injection of the state holding entity</param>
        protected StateBasedAggregateRoot(TState state)
        {
            this.State = state;
        }

        /// <summary>
        /// Gets or sets the state holding entity
        /// </summary>
        protected TState State { get; set; }
    }
}