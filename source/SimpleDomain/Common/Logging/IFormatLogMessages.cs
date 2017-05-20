//-------------------------------------------------------------------------------
// <copyright file="IFormatLogMessages.cs" company="frokonet.ch">
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
    /// The log message formatter interface
    /// </summary>
    public interface IFormatLogMessages
    {
        /// <summary>
        /// Formats the input to a log message string
        /// </summary>
        /// <param name="classType">The type of the class to log from</param>
        /// <param name="logLevel">The log level</param>
        /// <param name="message">The message which is stringified by .ToString()</param>
        /// <returns>A formatted log message string</returns>
        string Format(Type classType, LogLevel logLevel, object message);

        /// <summary>
        /// Formats the input to a log message string
        /// </summary>
        /// <param name="classType">The type of the class to log from</param>
        /// <param name="logLevel">The log level</param>
        /// <param name="message">The message string with placeholders for format arguments</param>
        /// <param name="args">The format arguments</param>
        /// <returns>A formatted log message string</returns>
        string Format(Type classType, LogLevel logLevel, string message, params object[] args);

        /// <summary>
        /// Formats the input to a log message string
        /// </summary>
        /// <param name="classType">The type of the class to log from</param>
        /// <param name="logLevel">The log level</param>
        /// <param name="exception">The exception</param>
        /// <param name="message">The message string with placeholders for format arguments</param>
        /// <param name="args">The format arguments</param>
        /// <returns>A formatted log message string</returns>
        string Format(Type classType, LogLevel logLevel, Exception exception, string message, params object[] args);
    }
}