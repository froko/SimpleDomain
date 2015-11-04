//-------------------------------------------------------------------------------
// <copyright file="RavenQueryableExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Raven.Client;
    using Raven.Client.Linq;

    /// <summary>
    /// Some useful RavenQueryable extensions
    /// </summary>
    public static class RavenQueryableExtensions
    {
        /// <summary>
        /// Gets all events out of a RavenQueryable of event descriptors
        /// </summary>
        /// <param name="ravenQueryable">A RavenQueryable of event descriptors</param>
        /// <returns>A list of events</returns>
        public static async Task<IList<IEvent>> GetAllEventsAsync(this IRavenQueryable<EventDescriptor> ravenQueryable)
        {
            const int ElementTakeCount = 1024;

            var events = new List<IEvent>();
            var counter = 0;
            var skipResults = 0;

            IList<IEvent> nextGroupOfEvents;

            do
            {
                RavenQueryStatistics statistics;

                nextGroupOfEvents = await ravenQueryable
                    .Statistics(out statistics)
                    .Skip((counter * ElementTakeCount) + skipResults)
                    .Take(ElementTakeCount)
                    .Select(e => e.Event)
                    .ToListAsync();

                counter++;
                skipResults += statistics.SkippedResults;

                events = events.Concat(nextGroupOfEvents).ToList();
            }
            while (nextGroupOfEvents.Count == ElementTakeCount);

            return events;
        }

        /// <summary>
        /// Gets all snapshots out of a RavenQueryable of snapshot descriptors
        /// </summary>
        /// <param name="ravenQueryable">A RavenQueryable of snapshot descriptors</param>
        /// <returns>A list of snapshots</returns>
        public static async Task<IList<ISnapshot>> GetAllSnapshotsAsync(this IRavenQueryable<SnapshotDescriptor> ravenQueryable)
        {
            const int ElementTakeCount = 1024;

            var snapshots = new List<ISnapshot>();
            var counter = 0;
            var skipResults = 0;

            IList<ISnapshot> nextGroupOfEvents;

            do
            {
                RavenQueryStatistics statistics;

                nextGroupOfEvents = await ravenQueryable
                    .Statistics(out statistics)
                    .Skip((counter * ElementTakeCount) + skipResults)
                    .Take(ElementTakeCount)
                    .Select(e => e.Snapshot)
                    .ToListAsync();

                counter++;
                skipResults += statistics.SkippedResults;

                snapshots = snapshots.Concat(nextGroupOfEvents).ToList();
            }
            while (nextGroupOfEvents.Count == ElementTakeCount);

            return snapshots;
        }
    }
}