//-------------------------------------------------------------------------------
// <copyright file="HeaderKeys.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    /// <summary>
    /// A directory for predefined header keys
    /// </summary>
    public static class HeaderKeys
    {
        /// <summary>
        /// Gets the header key for the sending endpoint
        /// </summary>
        public const string Sender = "Sender";

        /// <summary>
        /// Gets the header key for the receiving endpoint
        /// </summary>
        public const string Recipient = "Recipient";

        /// <summary>
        /// Gets the header key for the sending timestamp
        /// </summary>
        public const string TimeSent = "TimeSent";

        /// <summary>
        /// Gets the header key for the message type
        /// </summary>
        public const string MessageType = "MessageType";

        /// <summary>
        /// Gets the header key for the message name
        /// </summary>
        public const string MessageName = "MessageName";

        /// <summary>
        /// Gets the header key for the message id
        /// </summary>
        public const string MessageId = "MessageId";

        /// <summary>
        /// Gets the header key for the correlation id
        /// </summary>
        public const string CorrelationId = "CorrelationId";

        /// <summary>
        /// Gets the header key for the processing timestamp
        /// </summary>
        public const string TimeProcessed = "TimeProcessed";

        /// <summary>
        /// Gets the header key for the exception name
        /// </summary>
        public const string ExceptionName = "ExceptionName";

        /// <summary>
        /// Gets the header key for the exception message
        /// </summary>
        public const string ExceptionMessage = "ExceptionMessage";

        /// <summary>
        /// Gets the header key for the exception string
        /// </summary>
        public const string ExceptionString = "ExceptionString";

        /// <summary>
        /// Gets the header key for the retry count
        /// </summary>
        public const string RetryCount = "RetryCount";
    }
}