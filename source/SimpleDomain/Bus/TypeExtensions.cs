//-------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Type extensions for handler mappings
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets all typed command handler interfaces of a given class type
        /// </summary>
        /// <param name="classType">The class type</param>
        /// <returns>A list of <see cref="Type"/></returns>
        public static IEnumerable<Type> GetCommandHandlerInterfaces(this Type classType)
        {
            return classType.GetInterfaces().Where(i => IsHandlerInterface(i) && GenericArgumentIsCommand(i));
        }

        /// <summary>
        /// Gets all typed event hander interfaces of a given class type
        /// </summary>
        /// <param name="classType">The class type</param>
        /// <returns>A list of <see cref="Type"/></returns>
        public static IEnumerable<Type> GetEventHandlerInterfaces(this Type classType)
        {
            return classType.GetInterfaces().Where(i => i.IsHandlerInterface() && i.GenericArgumentIsEvent());
        }

        private static bool IsHandlerInterface(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IHandleAsync<>);
        }

        private static bool GenericArgumentIsCommand(this Type type)
        {
            return typeof(ICommand).IsAssignableFrom(type.GetGenericArguments()[0]);
        }

        private static bool GenericArgumentIsEvent(this Type type)
        {
            return typeof(IEvent).IsAssignableFrom(type.GetGenericArguments()[0]);
        }
    }
}