//-------------------------------------------------------------------------------
// <copyright file="IResolveTypes.cs" company="frokonet.ch">
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
    using System.Collections.Generic;

    /// <summary>
    /// The type resolver interface
    /// </summary>
    public interface IResolveTypes
    {
        /// <summary>
        /// Resolves the registered instance of a given type
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <returns>An instance of the given type</returns>
        T Resolve<T>();

        /// <summary>
        /// Resolves all registered instances of a given type
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <returns>A list of instances of the given type</returns>
        IEnumerable<T> ResolveAll<T>();
    }
}