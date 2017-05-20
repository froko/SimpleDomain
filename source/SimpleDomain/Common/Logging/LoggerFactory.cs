//-------------------------------------------------------------------------------
// <copyright file="LoggerFactory.cs" company="frokonet.ch">
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
    /// The static logger factor
    /// </summary>
    public static class LoggerFactory
    {
        private static IFormatLogMessages formatter = new LogMessageFormatter();
        private static Func<Type, IFormatLogMessages, ILogger> createLogger = (type, formatter)
            => new TraceLogger(type, formatter);

        /// <summary>
        /// Creates a new instance of a registered class that implements <see cref="ILogger"/>
        /// </summary>
        /// <typeparam name="T">The type of the class to log from</typeparam>
        /// <returns>A new istance of <see cref="ILogger"/></returns>
        public static ILogger Create<T>()
        {
            return createLogger(typeof(T), formatter);
        }

        /// <summary>
        /// Registers a log message formatter
        /// </summary>
        /// <param name="messageFormatter">An instance which implements <see cref="IFormatLogMessages"/></param>
        public static void Register(IFormatLogMessages messageFormatter)
        {
            formatter = messageFormatter;
        }

        /// <summary>
        /// Registers a function to create a new instance of a class that implements <see cref="ILogger"/>
        /// </summary>
        /// <param name="createNewLogger">The function to create a new instance of a class that implements <see cref="ILogger"/></param>
        public static void Register(Func<Type, IFormatLogMessages, ILogger> createNewLogger)
        {
            createLogger = createNewLogger;
        }
    }
}