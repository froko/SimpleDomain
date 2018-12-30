//-------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="frokonet.ch">
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

    /// <summary>
    /// Type extensions for handler mappings
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the fact that a given class type implements at least one async handler interface
        /// </summary>
        /// <param name="classType">The class type</param>
        /// <returns><c>true</c> if the class type implements 1 to many async handler interfaces or <c>false</c> if not</returns>
        public static bool ImplementsAsyncHandlerInterface(this Type classType)
        {
            if (classType.HasPreventAutomaticHandlerRegistrationAttribute())
            {
                return false;
            }

            var asyncHandlerInterfaces = from asyncHandlerInterface in classType.GetInterfaces()
                                         where asyncHandlerInterface.IsGenericType
                                         let baseInterface = asyncHandlerInterface.GetGenericTypeDefinition()
                                         where baseInterface == typeof(IHandleAsync<>)
                                         select asyncHandlerInterface.GetGenericArguments();

            return asyncHandlerInterfaces.Any();
        }

        /// <summary>
        /// Gets all message types which a handler class type can handle
        /// </summary>
        /// <param name="type">The type of the handler class</param>
        /// <returns>A list of message types</returns>
        public static IEnumerable<Type> GetAllMessageTypeThisTypeCanHandle(this Type type)
        {
            return from asyncHandlerInterface in type.GetInterfaces()
                where asyncHandlerInterface.IsGenericType
                let messageType = asyncHandlerInterface.GetGenericArguments()[0]
                where typeof(IHandleAsync<>).MakeGenericType(messageType).IsAssignableFrom(asyncHandlerInterface)
                select messageType;
        }

        private static bool HasPreventAutomaticHandlerRegistrationAttribute(this Type type)
        {
            return type.GetCustomAttributes(false)
                .Any(attribute => attribute is PreventAutomaticHandlerRegistrationAttribute);
        }
    }
}