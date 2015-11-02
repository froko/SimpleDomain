//-------------------------------------------------------------------------------
// <copyright file="LiskovSubstitutionException.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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
    /// The exception that is thrown when a method in a derived class makes no sense
    /// (See Liskov Substitution Principle [LSP])
    /// </summary>
    [Serializable]
    public class LiskovSubstitutionException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="LiskovSubstitutionException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        public LiskovSubstitutionException(string message) : base(message)
        {
        }
    }
}