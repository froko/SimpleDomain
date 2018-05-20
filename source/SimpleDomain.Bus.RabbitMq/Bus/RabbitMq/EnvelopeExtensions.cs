//-------------------------------------------------------------------------------
// <copyright file="EnvelopeExtensions.cs" company="frokonet.ch">
//   Copyright (c) 2014-2018
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

namespace SimpleDomain.Bus.RabbitMq
{
    using System.Text;

    using Newtonsoft.Json;

    /// <summary>
    /// Extension methods for envelopes
    /// </summary>
    public static class EnvelopeExtensions
    {
        private static readonly JsonSerializerSettings DefaultSerializerSettings =
            new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

        /// <summary>
        /// Serializes an envelope as Json string and encodes it into a byte array
        /// </summary>
        /// <param name="envelope">The envelope</param>
        /// <returns>A byte array containing the serialized Json string of the envelope</returns>
        public static byte[] AsByteArray(this Envelope envelope)
        {
            return Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(envelope, Formatting.None, DefaultSerializerSettings));
        }
    }
}