//-------------------------------------------------------------------------------
// <copyright file="ISubscribeHandlers.cs" company="frokonet.ch">
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
    using System.Threading.Tasks;

    /// <summary>
    /// The handler subscription interface
    /// </summary>
    public interface ISubscribeHandlers
    {
        /// <summary>
        /// Subscribes all handlers in this assembly
        /// </summary>
        void SubscribeAllHandlersInThisAssembly();

        /// <summary>
        /// Subscribes a handler for a given message
        /// </summary>
        /// <typeparam name="TMessage">The type of the message</typeparam>
        /// <typeparam name="THandler">The type of the handler</typeparam>
        void Subscribe<TMessage, THandler>() where TMessage : IMessage where THandler : IHandleAsync<TMessage>;

        /// <summary>
        /// Subscribes an async handler action for a given command
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="handler">The action to handle the command asynchronously</param>
        void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand;

        /// <summary>
        /// Subscribes an async handler action for a given event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="handler">The action to handle the event asynchronously</param>
        void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
    }
}