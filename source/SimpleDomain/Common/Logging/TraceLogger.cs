//-------------------------------------------------------------------------------
// <copyright file="TraceLogger.cs" company="frokonet.ch">
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
    using System.Diagnostics;
    
    /// <summary>
    /// The Trace logger
    /// </summary>
    public class TraceLogger : ILogger
    {
        private readonly Type classType;
        private readonly IFormatLogMessages formatter;

        /// <summary>
        /// Creates a new instance of <see cref="TraceLogger"/>
        /// </summary>
        /// <param name="classType">The type of the class from which the log entry comes from</param>
        /// <param name="formatter">Dependency injection for <see cref="IFormatLogMessages"/></param>
        public TraceLogger(Type classType, IFormatLogMessages formatter)
        {
            this.classType = classType;
            this.formatter = formatter;
        }

        /// <inheritdoc />
        public void Debug(object message)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Debug, message));
        }

        /// <inheritdoc />
        public void DebugFormat(string message, params object[] args)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Debug, message, args));
        }

        /// <inheritdoc />
        public void Info(object message)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Info, message));
        }

        /// <inheritdoc />
        public void InfoFormat(string message, params object[] args)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Info, message, args));
        }

        /// <inheritdoc />
        public void Warn(object message)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Warning, message));
        }

        /// <inheritdoc />
        public void Warn(Exception exception, object message)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Warning, exception, message.ToString()));
        }

        /// <inheritdoc />
        public void WarnFormat(string message, params object[] args)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Warning, message, args));
        }

        /// <inheritdoc />
        public void WarnFormat(Exception exception, string message, params object[] args)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Warning, exception, message, args));
        }

        /// <inheritdoc />
        public void Error(Exception exception, object message)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Error, exception, message.ToString()));
        }

        /// <inheritdoc />
        public void ErrorFormat(Exception exception, string message, params object[] args)
        {
            Trace.WriteLine(this.formatter.Format(this.classType, LogLevel.Error, exception, message, args));
        }
    }
}