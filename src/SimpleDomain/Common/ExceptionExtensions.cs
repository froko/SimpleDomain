//-------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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

namespace SimpleDomain.Common
{
    using System;

    /// <summary>
    /// Some <see cref="Exception"/> extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Gets the inner most exception
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns>The inner most exception</returns>
        public static Exception InnerMostException(this Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception;
        }
    }
}