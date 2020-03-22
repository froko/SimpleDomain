//-------------------------------------------------------------------------------
// <copyright file="IHaveJitneySubscriptions.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Jitney message subscription holder interface
    /// </summary>
    public interface IHaveJitneySubscriptions
    {
        /// <summary>
        /// Gets a command subscription for a given command
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="command">The command</param>
        /// <returns>A command subscription or <c>null</c> if none was found</returns>
        Subscription GetCommandSubscription<TCommand>(TCommand command) where TCommand : ICommand;

        /// <summary>
        /// Gets a list of event subscriptions for a given event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="event">The event</param>
        /// <returns>A list of event subscriptions. This list can also be empty if no subscriptions were found</returns>
        IEnumerable<Subscription> GetEventSubscriptions<TEvent>(TEvent @event) where TEvent : IEvent;

        /// <summary>
        /// Gets all event types which have been subscribed
        /// </summary>
        /// <returns>A list of types</returns>
        IEnumerable<Type> GetSubscribedEventTypes();
    }
}