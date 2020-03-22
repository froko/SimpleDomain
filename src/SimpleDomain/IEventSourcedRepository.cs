//-------------------------------------------------------------------------------
// <copyright file="IEventSourcedRepository.cs" company="frokonet.ch">
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The event sourced repository interface for aggregate roots
    /// </summary>
    public interface IEventSourcedRepository
    {
        /// <summary>
        /// Gets an aggregate root identified by its id
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
        /// <param name="aggregateId">The aggregate root id</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        Task<TAggregateRoot> GetByIdAsync<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IEventSourcedAggregateRoot;

        /// <summary>
        /// Persits a new or modified aggregate root
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
        /// <param name="aggregateRoot">The aggregate root</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : IEventSourcedAggregateRoot;

        /// <summary>
        /// Persists a new or modified aggregate root
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
        /// <param name="aggregateRoot">The aggregate root</param>
        /// <param name="headers">A list of arbitrary headers which serve as meta information</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot, IDictionary<string, object> headers) where TAggregateRoot : IEventSourcedAggregateRoot;
    }
}