//-------------------------------------------------------------------------------
// <copyright file="GetEventStoreExtensions.cs" company="frokonet.ch">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using global::EventStore.ClientAPI;

    using Newtonsoft.Json;

    /// <summary>
    /// Common extensions for GetEventStore
    /// </summary>
    public static class GetEventStoreExtensions
    {
        /// <summary>
        /// Serializes an <see cref="IEvent"/> to a byte array
        /// </summary>
        /// <param name="event">The event</param>
        /// <returns>The serialized event as byte array</returns>
        public static byte[] AsByteArray(this IEvent @event)
        {
            return Serialize(@event);
        }

        /// <summary>
        /// Serializes an <see cref="ISnapshot"/> to a byte array
        /// </summary>
        /// <param name="snapshot">The snapshot</param>
        /// <returns>The serialized snapshot as byte array</returns>
        public static byte[] AsByteArray(this ISnapshot snapshot)
        {
            var serializedObject = JsonConvert.SerializeObject(snapshot);
            var data = Encoding.UTF8.GetBytes(serializedObject);

            return data;
        }

        /// <summary>
        /// Serializes event headers to a byte array
        /// </summary>
        /// <param name="eventHeaders">The event headers</param>
        /// <returns>The serialized event headers as byte array</returns>
        public static byte[] AsByteArray(this IDictionary<string, object> eventHeaders)
        {
            return Serialize(eventHeaders);
        }

        /// <summary>
        /// Converts an array of <see cref="ResolvedEvent"/> to an <see cref="EventHistory"/>
        /// </summary>
        /// <param name="resolvedEvents">An array of <see cref="ResolvedEvent"/></param>
        /// <returns>A new instance of <see cref="EventHistory"/> containing the original events</returns>
        public static EventHistory AsEventHistory(this ResolvedEvent[] resolvedEvents)
        {
            return new EventHistory(resolvedEvents.Deserialize());
        }

        /// <summary>
        /// Converts a <see cref="ResolvedEvent"/> to a snapshot
        /// </summary>
        /// <param name="resolvedEvent">The resolved event</param>
        /// <returns>An instance of a snapshot</returns>
        public static ISnapshot AsSnapshot(this ResolvedEvent resolvedEvent)
        {
            return resolvedEvent.DeserializeAsSnapshot();
        }

        /// <summary>
        /// Serializes the inner event of a verionable event with its headers to an <see cref="EventData"/>
        /// </summary>
        /// <param name="versionableEvent">The versionable event</param>
        /// <param name="headers">The headers</param>
        /// <returns>A new instance of <see cref="EventData"/> containing the inner event with its headers</returns>
        public static EventData SerializeInnerEvent(this VersionableEvent versionableEvent, IDictionary<string, object> headers)
        {
            return EventDataBuilder.Initialize(versionableEvent.InnerEvent).AddHeaders(headers).Build();
        }

        /// <summary>
        /// Serializes a snapshot to an <see cref="EventData"/>
        /// </summary>
        /// <param name="snapshot">The snapshot</param>
        /// <returns>A new instance of <see cref="EventData"/> containing the snapshot</returns>
        public static EventData Serialize(this ISnapshot snapshot)
        {
            return SnapshotDataBuilder.Initialize(snapshot).Build();
        }
        
        /// <summary>
        /// Deserializes an array of <see cref="ResolvedEvent"/> to a list of events
        /// </summary>
        /// <param name="resolvedEvents">The array of <see cref="ResolvedEvent"/></param>
        /// <returns>A list of the original events</returns>
        public static List<IEvent> Deserialize(this ResolvedEvent[] resolvedEvents)
        {
            return resolvedEvents.Select(DeserializeAsEvent).ToList();
        }

        private static byte[] Serialize(object obj)
        {
            var serializedObject = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(serializedObject);

            return data;
        }

        private static IEvent DeserializeAsEvent(this ResolvedEvent resolvedEvent)
        {
            var originalEvent = resolvedEvent.OriginalEvent;

            var metaData = originalEvent.Metadata.Deserialize<Dictionary<string, object>>();
            var clrType = metaData[Conventions.EventClrTypeHeader].ToString();
            var @event = originalEvent.Data.Deserialize<IEvent>(clrType);

            return @event;
        }

        private static ISnapshot DeserializeAsSnapshot(this ResolvedEvent resolvedEvent)
        {
            var originalEvent = resolvedEvent.OriginalEvent;

            var metaData = originalEvent.Metadata.Deserialize<Dictionary<string, object>>();
            var clrType = metaData[Conventions.EventClrTypeHeader].ToString();
            var snapshot = originalEvent.Data.Deserialize<ISnapshot>(clrType);

            return snapshot;
        }

        private static T Deserialize<T>(this byte[] data)
        {
            return data.Deserialize<T>(typeof(T).AssemblyQualifiedName);
        }

        private static T Deserialize<T>(this byte[] data, string typeName)
        {
            var jsonString = Encoding.UTF8.GetString(data);
            return (T)JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName));
        }
    }
}