//-------------------------------------------------------------------------------
// <copyright file="EventSubscription.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Threading.Tasks;

    using SimpleDomain.Common;

    /// <summary>
    /// The subscription of an event
    /// </summary>
    /// <typeparam name="TEvent">The type of the event</typeparam>
    public class EventSubscription<TEvent> : Subscription where TEvent : IEvent
    {
        private readonly Func<TEvent, Task> handler;
        private readonly Type eventType;

        /// <summary>
        /// Creates a new instance of <see cref="EventSubscription{TEvent}"/>
        /// </summary>
        /// <param name="handler">The action to handle the event asynchronously</param>
        public EventSubscription(Func<TEvent, Task> handler)
        {
            this.handler = handler;
            this.eventType = typeof(TEvent);
        }

        /// <inheritdoc />
        public override bool CanHandle(IMessage message)
        {
            return message.GetType().IsAssignableFrom(this.eventType);
        }

        /// <inheritdoc />
        public override bool CanHandle<TMessage>()
        {
            return typeof(TMessage).IsAssignableFrom(this.eventType);
        }

        /// <inheritdoc />
        public override Task HandleAsync(IMessage message)
        {
            Guard.NotNull(() => message);
            return this.handler.Invoke((TEvent)message);
        }
    }
}