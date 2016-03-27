//-------------------------------------------------------------------------------
// <copyright file="DataReaderExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System.Data;

    using Newtonsoft.Json;

    using SimpleDomain.Common;

    /// <summary>
    /// Some useful DataReader extensions
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Deserializes and gets an event out of a <see cref="IDataReader"/>
        /// </summary>
        /// <param name="reader">The data reader</param>
        /// <returns>A deserialized event</returns>
        public static IEvent GetEvent(this IDataReader reader)
        {
            var serializedEvent = reader.GetString(0);
            var eventType = reader.GetString(1);

            return JsonConvert.DeserializeObject(serializedEvent, TypeHelper.GetType(eventType)) as IEvent;
        }

        /// <summary>
        /// Deserializes and gets a snapshot out of a <see cref="IDataReader"/>
        /// </summary>
        /// <param name="reader">The data reader</param>
        /// <returns>A deserialized snapshot</returns>
        public static ISnapshot GetSnapshot(this IDataReader reader)
        {
            var serializedSnapshot = reader.GetString(0);
            var snapshotType = reader.GetString(1);

            return JsonConvert.DeserializeObject(serializedSnapshot, TypeHelper.GetType(snapshotType)) as ISnapshot;
        }
    }
}