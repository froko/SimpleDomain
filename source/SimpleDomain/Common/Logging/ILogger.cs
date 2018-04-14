//-------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="frokonet.ch">
//   Copyright (c) 2014-2017
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

namespace SimpleDomain.Common.Logging
{
    using System;

    /// <summary>
    /// The common logger interface for SimpleDomain
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message at Debug level
        /// </summary>
        /// <param name="message">The message which is stringified by .ToString()</param>
        void Debug(object message);

        /// <summary>
        /// Logs a message at Debg level
        /// </summary>
        /// <param name="message">The message string with placeholders for format arguments</param>
        /// <param name="args">The format arguments</param>
        void DebugFormat(string message, params object[] args);

        /// <summary>
        /// Logs a message at Info level
        /// </summary>
        /// <param name="message">The message which is stringified by .ToString()</param>
        void Info(object message);

        /// <summary>
        /// Logs a message at Info level
        /// </summary>
        /// <param name="message">The message string with placeholders for format arguments</param>
        /// <param name="args">The format arguments</param>
        void InfoFormat(string message, params object[] args);

        /// <summary>
        /// Logs a message at Warning level
        /// </summary>
        /// <param name="message">The message which is stringified by .ToString()</param>
        void Warn(object message);

        /// <summary>
        /// Logs an exception ana a message at Warning level
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The message which is stringified by .ToString()</param>
        void Warn(Exception exception, object message);

        /// <summary>
        /// Logs a message at Warning level
        /// </summary>
        /// <param name="message">The message string with placeholders for format arguments</param>
        /// <param name="args">The format arguments</param>
        void WarnFormat(string message, params object[] args);

        /// <summary>
        /// Logs an exception and a message at Warning level
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The message string with placeholders for format arguments</param>
        /// <param name="args">The format arguments</param>
        void WarnFormat(Exception exception, string message, params object[] args);

        /// <summary>
        /// Logs an exception and a message at Error level
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The message which is stringified by .ToString()</param>
        void Error(Exception exception, object message);

        /// <summary>
        /// Logs an exception and a message at Error level
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The message string with placeholders for format arguments</param>
        /// <param name="args">The format arguments</param>
        void ErrorFormat(Exception exception, string message, params object[] args);
    }
}