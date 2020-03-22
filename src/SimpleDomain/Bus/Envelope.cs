//-------------------------------------------------------------------------------
// <copyright file="Envelope.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using SimpleDomain.Common;

    /// <summary>
    /// The message envelope
    /// </summary>
    public class Envelope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope"/> class.
        /// </summary>
        /// <param name="headers">The headers</param>
        /// <param name="body">The message body</param>
        [JsonConstructor]
        public Envelope(
            Dictionary<string, object> headers,
            IMessage body)
        {
            this.Headers = headers;
            this.Body = body;
        }

        /// <summary>
        /// Gets the headers
        /// </summary>
        public Dictionary<string, object> Headers { get; }

        /// <summary>
        /// Gets the message body
        /// </summary>
        public IMessage Body { get; }

        /// <summary>
        /// Gets the correlation id
        /// </summary>
        [JsonIgnore]
        public Guid CorrelationId => Guid.Parse(this.GetHeader<object>(HeaderKeys.CorrelationId).ToString());

        /// <summary>
        /// Creates a new instance of <see cref="Envelope"/>
        /// </summary>
        /// <param name="sender">The sending endpoint address</param>
        /// <param name="recipient">The receiving endpoint address</param>
        /// <param name="body">The message body</param>
        /// <returns>A new instance of <see cref="Envelope"/></returns>
        public static Envelope Create(EndpointAddress sender, EndpointAddress recipient, IMessage body)
        {
            Guard.NotNull(() => sender);
            Guard.NotNull(() => recipient);
            Guard.NotNull(() => body);

            var messageId = Guid.NewGuid();
            return Create(sender, recipient, messageId, messageId, body);
        }

        /// <summary>
        /// Creates a new instance of <see cref="Envelope"/>
        /// </summary>
        /// <param name="sender">The sending endpoint address</param>
        /// <param name="recipient">The receiving endpoint address</param>
        /// <param name="correlationId">The correlation id</param>
        /// <param name="body">The message body</param>
        /// <returns>A new instance of <see cref="Envelope"/></returns>
        public static Envelope Create(
            EndpointAddress sender,
            EndpointAddress recipient,
            Guid correlationId,
            IMessage body)
        {
            Guard.NotNull(() => sender);
            Guard.NotNull(() => recipient);
            Guard.NotNull(() => body);

            var messageId = Guid.NewGuid();
            return Create(sender, recipient, messageId, correlationId, body);
        }

        /// <summary>
        /// Adds a single header to the header collection
        /// </summary>
        /// <param name="key">The header key</param>
        /// <param name="value">The header value</param>
        /// <returns>The envelope itself since this is a builder method</returns>
        public Envelope AddHeader(string key, object value)
        {
            if (this.Headers.ContainsKey(key))
            {
                return this;
            }

            this.Headers.Add(key, value);
            return this;
        }

        /// <summary>
        /// Replaces a single header in the header collection
        /// </summary>
        /// <param name="key">The header key</param>
        /// <param name="value">The header value</param>
        /// <returns>The envelope itself since this is a builder method</returns>
        public Envelope ReplaceHeader(string key, object value)
        {
            if (this.Headers.ContainsKey(key))
            {
                this.Headers.Remove(key);
            }

            this.Headers.Add(key, value);
            return this;
        }

        /// <summary>
        /// Gets the typed header item by its key
        /// </summary>
        /// <typeparam name="T">The type of the header item</typeparam>
        /// <param name="key">The header key</param>
        /// <returns>The typed header item</returns>
        public T GetHeader<T>(string key)
        {
            Guard.NotNullOrEmpty(() => key);

            if (!this.Headers.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            if (!(this.Headers[key] is T))
            {
                throw new InvalidCastException();
            }

            return (T)this.Headers[key];
        }

        private static Envelope Create(
            EndpointAddress sender,
            EndpointAddress recipient,
            Guid messageId,
            Guid correlationId,
            IMessage body)
        {
            var messageIntent = MessageIntent.Unknown;

            if (body is ICommand)
            {
                messageIntent = MessageIntent.Command;
            }

            if (body is IEvent)
            {
                messageIntent = MessageIntent.Event;
            }

            if (body is SubscriptionMessage)
            {
                messageIntent = MessageIntent.SubscriptionMessage;
            }

            var headers = new Dictionary<string, object>
            {
                { HeaderKeys.Sender, sender },
                { HeaderKeys.Recipient, recipient },
                { HeaderKeys.TimeSent, DateTime.UtcNow },
                { HeaderKeys.MessageType, body.GetFullName() },
                { HeaderKeys.MessageIntent, messageIntent },
                { HeaderKeys.MessageId, messageId },
                { HeaderKeys.CorrelationId, correlationId }
            };

            return new Envelope(headers, body);
        }
    }
}