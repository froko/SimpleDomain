//-------------------------------------------------------------------------------
// <copyright file="IHandlerInvocationCache.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The handler invocation cache interface
    /// </summary>
    public interface IHandlerInvocationCache
    {
        /// <summary>
        /// Adds an async handler/message pair by their types
        /// </summary>
        /// <param name="asyncHandlerType">The type of the async handler</param>
        /// <param name="messageType">The type of the message</param>
        void Add(Type asyncHandlerType, Type messageType);

        /// <summary>
        /// Asynchronously invokes a handler instance with a message it can handle
        /// </summary>
        /// <param name="handler">The handler instance</param>
        /// <param name="message">The message instance</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        Task InvokeAsync(object handler, object message);
    }
}