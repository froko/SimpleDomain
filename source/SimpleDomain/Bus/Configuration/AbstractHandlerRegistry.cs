//-------------------------------------------------------------------------------
// <copyright file="AbstractHandlerRegistry.cs" company="frokonet.ch">
//   Copyright (c) 2014-2016
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
    /// The abstract handler registry
    /// </summary>
    public abstract class AbstractHandlerRegistry
    {
        private readonly IDictionary<Type, List<Type>> handlerList;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractHandlerRegistry"/>
        /// </summary>
        protected AbstractHandlerRegistry()
        {
            this.handlerList = new Dictionary<Type, List<Type>>();
        }

        /// <summary>
        /// Registers an async handler/message pair by their types
        /// </summary>
        /// <param name="asyncHandlerType">The type of the async handler</param>
        /// <param name="messageType">The type of the message</param>
        public virtual void Register(Type asyncHandlerType, Type messageType)
        {
            this.AddToHandlerList(asyncHandlerType, messageType);
        }

        /// <summary>
        /// Gets an untyped command handler reference for a given command instance
        /// </summary>
        /// <param name="command">The command instance</param>
        /// <returns>An untyped command handler reference of <c>null</c> if none was found</returns>
        public virtual object GetCommandHandler(ICommand command)
        {
            var commandHandlerType = this.GetHandlerTypes(command.GetType()).FirstOrDefault();
            return commandHandlerType != null 
                ? this.Resolve(commandHandlerType) 
                : null;
        }

        /// <summary>
        /// Gets a list of untyped event handler references for a given event instance
        /// </summary>
        /// <param name="event">The event instance</param>
        /// <returns>A list of untyped event handlers. This list can be empty but should never be <c>null</c>.</returns>
        public virtual IEnumerable<object> GetEventHandlers(IEvent @event)
        {
            var eventHandlerTypes = this.GetHandlerTypes(@event.GetType()).ToList();
            
            if (eventHandlerTypes.Any())
            {
                return from eventHandlerType in eventHandlerTypes
                       let eventHandler = this.Resolve(eventHandlerType)
                       where eventHandler != null
                       select this.Resolve(eventHandlerType);
            }

            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Resolves an async handler instance based on its type
        /// </summary>
        /// <param name="handlerType">The type of the async handler</param>
        /// <returns>An async handler instance or <c>null</c> if none was found</returns>
        protected abstract object Resolve(Type handlerType);

        private void AddToHandlerList(Type asyncHandlerType, Type messageType)
        {
            List<Type> messageTypeList;
            if (!this.handlerList.TryGetValue(asyncHandlerType, out messageTypeList))
            {
                this.handlerList[asyncHandlerType] = messageTypeList = new List<Type>();
            }

            if (!messageTypeList.Contains(messageType))
            {
                messageTypeList.Add(messageType);
            }
        }

        private IEnumerable<Type> GetHandlerTypes(Type messageType)
        {
            return from keyValue in this.handlerList
                where keyValue.Value.Any(msgTypeHandled => msgTypeHandled.IsAssignableFrom(messageType))
                select keyValue.Key;
        }
    }
}