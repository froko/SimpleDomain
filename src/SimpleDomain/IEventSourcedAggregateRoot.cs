//-------------------------------------------------------------------------------
// <copyright file="IEventSourcedAggregateRoot.cs" company="frokonet.ch">
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
    using System;

    /// <summary>
    /// Interface for all event sourced aggregate roots
    /// </summary>
    public interface IEventSourcedAggregateRoot : IAggregateRoot, INeedVersion
    {
        /// <summary>
        /// Gets the id of this Aggregate Root
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Creates a snapshot of this Aggregate Root
        /// </summary>
        /// <returns>A snapshot</returns>
        ISnapshot CreateSnapshot();

        /// <summary>
        /// Builds up the Aggregate Root from a snapshot
        /// </summary>
        /// <param name="snapshot">The snapshot</param>
        void LoadFromSnapshot(ISnapshot snapshot);

        /// <summary>
        /// Builds up the Aggregate Root from a list of events
        /// </summary>
        /// <param name="eventHistory">The history as list of events</param>
        void LoadFromEventHistory(EventHistory eventHistory);
    }
}