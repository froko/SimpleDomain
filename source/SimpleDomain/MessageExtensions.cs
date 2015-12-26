//-------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using SimpleDomain.Bus;

    /// <summary>
    /// Common extension methods for messages
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Gets the full CLR type name of the message
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>The full CLR type name</returns>
        public static string GetFullName(this IMessage message)
        {
            return message.GetType().FullName;
        }

        /// <summary>
        /// Gets the intent of the message
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>The intent of the message</returns>
        public static MessageIntent GetIntent(this IMessage message)
        {
            if (message is ICommand)
            {
                return MessageIntent.Command;
            }

            if (message is IEvent)
            {
                return MessageIntent.Event;
            }

            if (message is SubscriptionMessage)
            {
                return MessageIntent.SubscriptionMessage;
            }

            return MessageIntent.Unknown;
        }

        /// <summary>
        /// Sets the version in a fluent manner
        /// </summary>
        /// <typeparam name="T">The type of a message on which a version can be set</typeparam>
        /// <param name="message">The message</param>
        /// <param name="version">The version</param>
        /// <returns>The version itself since this is a fluently used method</returns>
        public static T WithVersion<T>(this T message, int version) where T : INeedVersion
        {
            var versionProperty = message.GetType().GetProperty("Version");

            versionProperty?.SetValue(message, version);

            return message;
        }
    }
}