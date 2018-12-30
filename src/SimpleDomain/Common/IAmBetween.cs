//-------------------------------------------------------------------------------
// <copyright file="IAmBetween.cs" company="frokonet.ch">
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
    /// Defines a numeric range guard interface
    /// </summary>
    /// <typeparam name="T">The type of the input parameter</typeparam>
    public interface IAmBetween<in T>
    {
        /// <summary>
        /// Checks if the input parameter defined by the constructor
        /// is between a lower and upper bound and throws an <see cref="ArgumentException"/> otherwise
        /// </summary>
        /// <param name="lowerBound">The lower bound (including)</param>
        /// <param name="upperBound">the upper bound (including)</param>
        void IsBetween(T lowerBound, T upperBound);
    }
}