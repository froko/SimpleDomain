//-------------------------------------------------------------------------------
// <copyright file="NullLogger.cs" company="frokonet.ch">
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
    /// A logger implementation which does nothing
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <inheritdoc />
        public void Debug(object message)
        {
        }

        /// <inheritdoc />
        public void DebugFormat(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void Info(object message)
        {
        }

        /// <inheritdoc />
        public void InfoFormat(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void Warn(object message)
        {
        }

        /// <inheritdoc />
        public void Warn(Exception exception, object message)
        {
        }

        /// <inheritdoc />
        public void WarnFormat(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void WarnFormat(Exception exception, string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void Error(Exception exception, object message)
        {
        }

        /// <inheritdoc />
        public void ErrorFormat(Exception exception, string message, params object[] args)
        {
        }
    }
}