//-------------------------------------------------------------------------------
// <copyright file="FluentTestingExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System.Linq;

    using FluentAssertions;
    using FluentAssertions.Primitives;

    using SimpleDomain.Common;

    /// <summary>
    /// Commonly used testing extensions
    /// </summary>
    public static class FluentTestingExtensions
    {
        /// <summary>
        /// Asserts that a given string has been logged
        /// </summary>
        /// <param name="stringAssertions">The string that should have been logged</param>
        public static LogLevelAware HaveBeenLogged(this StringAssertions stringAssertions)
        {
            var logMessage = InMemoryTraceListener.LogMessages.LastOrDefault(s => s.Contains(stringAssertions.Subject));

            logMessage.Should().NotBeNullOrEmpty($"{stringAssertions.Subject} should have been logged");

            return new LogLevelAware(logMessage);
        }

        public static void WithDebugLevel(this LogLevelAware logLevelAware)
        {
            logLevelAware.LogMessage.Should().Contain("[DEBUG]", "log level should be [DEBUG]");
        }

        public static void WithInfoLevel(this LogLevelAware logLevelAware)
        {
            logLevelAware.LogMessage.Should().Contain("[INFO]", "log level should be [INFO]");
        }

        public static void WithWarningLevel(this LogLevelAware logLevelAware)
        {
            logLevelAware.LogMessage.Should().Contain("[WARN]", "log level should be [WARN]");
        }

        public static void WithErrorLevel(this LogLevelAware logLevelAware)
        {
            logLevelAware.LogMessage.Should().Contain("[ERROR]", "log level should be [ERROR]");
        }
    }

    public class LogLevelAware
    {
        public LogLevelAware(string logMessage)
        {
            this.LogMessage = logMessage;
        }

        public string LogMessage { get; private set; }
    }
}