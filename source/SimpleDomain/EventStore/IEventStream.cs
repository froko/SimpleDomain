//-------------------------------------------------------------------------------
// <copyright file="IEventStream.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The event stream interface
    /// </summary>
    public interface IEventStream : IDisposable
    {
        /// <summary>
        /// Persits a list of events
        /// </summary>
        /// <param name="events">A list of versionable events</param>
        /// <param name="expectedVersion">The actual version of the aggregate root</param>
        /// <param name="headers">A list of arbitrary headers</param>
        Task SaveAsync(IEnumerable<VersionableEvent> events, int expectedVersion, IDictionary<string, object> headers);

        /// <summary>
        /// Persists a sapshot
        /// </summary>
        /// <param name="snapshot">The snapshot</param>
        Task SaveSnapshotAsync(ISnapshot snapshot);

        /// <summary>
        /// Replays all events of an aggregate root
        /// </summary>
        /// <returns>An observable of events ordered by their version</returns>
        Task<EventHistory> ReplayAsync();

        /// <summary>
        /// Replays all events of an aggregate root since a given snapshot
        /// </summary>
        /// <param name="snapshot">The snapshot</param>
        /// <returns>An observable of events ordered by their version</returns>
        Task<EventHistory> ReplayAsyncFromSnapshot(ISnapshot snapshot); 

        /// <summary>
        /// Returns the fact that there exists at least one snapshot
        /// </summary>
        /// <returns></returns>
        Task<bool> HasSnapshotAsync();

        /// <summary>
        /// Gets the latest snapshot
        /// </summary>
        /// <returns>The latest snapshot</returns>
        Task<ISnapshot> GetLatestSnapshotAsync();
    }
}