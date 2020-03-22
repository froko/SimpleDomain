//-------------------------------------------------------------------------------
// <copyright file="MissingConfigurationException.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
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
    using System;

    /// <summary>
    /// The exception that is thrown when a configuration item is missing or not yet set
    /// </summary>
    [Serializable]
    public class MissingConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        public MissingConfigurationException(string message) : base(message)
        {
        }
    }
}