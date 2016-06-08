//-------------------------------------------------------------------------------
// <copyright file="IEventStore.cs" company="frokonet.ch">
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

    /// <summary>
    /// The event store interface
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Returns an event stream for an aggregate root identified by its id
        /// </summary>
        /// <typeparam name="T">The type of the aggregate root</typeparam>
        /// <param name="aggregateId">The aggregate root id</param>
        /// <returns>An event stream</returns>
        IEventStream OpenStream<T>(Guid aggregateId) where T : IEventSourcedAggregateRoot;
    }
}