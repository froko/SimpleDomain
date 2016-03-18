//-------------------------------------------------------------------------------
// <copyright file="JsonMessageFormatter.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.MSMQ
{
    using System.IO;
    using System.Messaging;
    using System.Text;

    using Newtonsoft.Json;

    using SimpleDomain.Common;

    /// <summary>
    /// The Json message formatter
    /// </summary>
    public class JsonMessageFormatter : IMessageFormatter
    {
        private static readonly JsonSerializerSettings DefaultSerializerSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

        private readonly Encoding encoding;
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Creates a new instance of <see cref="JsonMessageFormatter"/>
        /// </summary>
        public JsonMessageFormatter()
        {
            this.encoding = Encoding.UTF8;
            this.serializerSettings = DefaultSerializerSettings;
        }

        /// <inheritdoc />
        public bool CanRead(Message message)
        {
            Guard.NotNull(() => message);

            var stream = message.BodyStream;

            return stream != null
                   && stream.CanRead
                   && stream.Length > 0;
        }

        /// <inheritdoc />
        public object Read(Message message)
        {
            Guard.NotNull(() => message);

            if (this.CanRead(message) == false)
            {
                return null;
            }

            using (var reader = new StreamReader(message.BodyStream, this.encoding))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject(json, this.serializerSettings);
            }
        }

        /// <inheritdoc />
        public void Write(Message message, object obj)
        {
            Guard.NotNull(() => message);
            Guard.NotNull(() => obj);

            var json = JsonConvert.SerializeObject(obj, Formatting.None, this.serializerSettings);

            message.BodyStream = new MemoryStream(this.encoding.GetBytes(json));

            message.BodyType = 0;
        }

        /// <summary>
        /// Clones this instance
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new JsonMessageFormatter();
        }
    }
}