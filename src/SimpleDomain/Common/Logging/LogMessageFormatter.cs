﻿//-------------------------------------------------------------------------------
// <copyright file="LogMessageFormatter.cs" company="frokonet.ch">
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
    /// The standard log message formatter
    /// </summary>
    public class LogMessageFormatter : IFormatLogMessages
    {
        /// <inheritdoc />
        public string Format(Type classType, LogLevel logLevel, object message)
        {
            return $"{DateTime.UtcNow} [{logLevel}]\t{classType.Name} >> {message}";
        }

        /// <inheritdoc />
        public string Format(Type classType, LogLevel logLevel, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            return $"{DateTime.UtcNow} [{logLevel}]\t{classType.Name} >> {formattedMessage}";
        }

        /// <inheritdoc />
        public string Format(Type classType, LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            return $"{DateTime.UtcNow} [{logLevel}]\t{classType.Name} >> {formattedMessage}\r\n{exception.Message}";
        }
    }
}