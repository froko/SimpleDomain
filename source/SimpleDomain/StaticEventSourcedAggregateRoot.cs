//-------------------------------------------------------------------------------
// <copyright file="StaticEventSourcedAggregateRoot.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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
    using System.Collections.Generic;

    /// <summary>
    /// Base class for all statically event sourced aggregate roots
    /// </summary>
    public abstract class StaticEventSourcedAggregateRoot : EventSourcedAggregateRoot
    {
        private readonly Dictionary<Type, Action<IEvent>> routes = new Dictionary<Type, Action<IEvent>>();

        /// <summary>
        /// Registers the state changing transition action
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="transition">The method that is called when an event is applied</param>
        protected void RegisterTransition<TEvent>(Action<TEvent> transition) where TEvent : class, IEvent
        {
            this.routes.Add(typeof(TEvent), @event => transition(@event as TEvent));
        }

        /// <inheritdoc />
        protected override void DoTransition(IEvent @event)
        {
            var eventType = @event.GetType();
            if (this.routes.ContainsKey(eventType))
            {
                this.routes[eventType](@event);
            }
        }
    }
}