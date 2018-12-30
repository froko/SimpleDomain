//-------------------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Some Assembly extensions
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets a list of async handler types for a given assembly
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>A list of async handler types</returns>
        public static IEnumerable<Type> GetAsyncHandlerTypes(this Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.IsClass && type.ImplementsAsyncHandlerInterface());
        }
    }
}