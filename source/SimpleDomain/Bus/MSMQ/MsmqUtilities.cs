//-------------------------------------------------------------------------------
// <copyright file="MsmqUtilities.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.MSMQ
{
    using System.Messaging;
    using System.Transactions;

    using SimpleDomain.Common;

    /// <summary>
    /// Some helpers for MSMQ
    /// </summary>
    public static class MsmqUtilities
    {
        /// <summary>
        /// Gets the formatted queue name
        /// </summary>
        /// <param name="endpointAddress">The endpoint address</param>
        /// <returns>A formatted queue name</returns>
        public static string GetFormattedQueueName(EndpointAddress endpointAddress)
        {
            Guard.NotNull(() => endpointAddress);

            return endpointAddress.IsLocal
                ? $@".\Private$\{endpointAddress.QueueName}"
                : $@"FormatName:Direct=OS:{endpointAddress.MachineName}\Private$\{endpointAddress.QueueName}";
        }

        /// <summary>
        /// Creates or opens a message queue
        /// </summary>
        /// <param name="queueName">The formatted queue name</param>
        /// <param name="queueAccessMode">The queue access mode</param>
        /// <returns>An instance of type <see cref="MessageQueue"/></returns>
        public static MessageQueue GetMessageQueue(string queueName, QueueAccessMode queueAccessMode)
        {
            Guard.NotNullOrEmpty(() => queueName);
            
            var messageQueue = MessageQueue.Exists(queueName)
                ? new MessageQueue(queueName, queueAccessMode)
                : MessageQueue.Create(queueName, true);

            messageQueue.Formatter = new JsonMessageFormatter();

            return messageQueue;
        }

        /// <summary>
        /// Creates or opens an message queue
        /// </summary>
        /// <param name="endpointAddress">The endpoint address</param>
        /// <param name="queueAccessMode">The queue access mode</param>
        /// <returns>An instance of type <see cref="MessageQueue"/></returns>
        public static MessageQueue GetMessageQueue(EndpointAddress endpointAddress, QueueAccessMode queueAccessMode)
        {
            Guard.NotNull(() => endpointAddress);
            
            var queueName = GetFormattedQueueName(endpointAddress);

            return GetMessageQueue(queueName, queueAccessMode);
        }

        /// <summary>
        /// Returns the current message queue transaction type
        /// </summary>
        /// <returns>The current message queue transaction type</returns>
        public static MessageQueueTransactionType GetTransactionType()
        {
            return Transaction.Current == null
                ? MessageQueueTransactionType.Single
                : MessageQueueTransactionType.Automatic;
        }
    }
}