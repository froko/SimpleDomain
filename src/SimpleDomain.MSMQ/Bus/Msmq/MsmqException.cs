﻿//-------------------------------------------------------------------------------
// <copyright file="MsmqException.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Msmq
{
    using System;

    /// <summary>
    /// The Exception that is thrown when something goes wrong with the MSMQ provider
    /// </summary>
    [Serializable]
    public class MsmqException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqException"/> class.
        /// </summary>
        /// <param name="message">The innerException message</param>
        /// <param name="innerException">The inner innerException</param>
        public MsmqException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}