//-------------------------------------------------------------------------------
// <copyright file="Jitney.cs" company="frokonet.ch">
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
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The abstract bus
    /// </summary>
    public abstract class Jitney : IDeliverMessages
    {
        private readonly IHaveJitneyConfiguration configuration;
        
        /// <summary>
        /// Creates a new instance of <see cref="Jitney"/>
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveJitneyConfiguration"/></param>
        protected Jitney(IHaveJitneyConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Starts the message reception process
        /// </summary>
        public abstract void Start();

        /// <inheritdoc />
        public abstract Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;

        /// <inheritdoc />
        public abstract Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;

        /// <summary>
        /// Subscribes an async handler action for a given command
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="handler">The async handler action (must return a <see cref="Task"/>)</param>
        public void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
        {
            this.configuration.SubscribeCommandHandler(handler);
        }

        /// <summary>
        /// Subscribes an async handler action for a given event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="handler">The async handler action (must return a <see cref="Task"/>)</param>
        public void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            this.configuration.SubscribeEventHandler(handler);
        }

        /// <summary>
        /// Executes the command subscription which is registered for this command
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="command">The instance of the command</param>
        protected async Task HandleCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandSubscription = this.configuration.Subscriptions.GetCommandSubscription(command);
            await commandSubscription.HandleAsync(command);
        }

        /// <summary>
        /// Executes all event subscriptions which are registered for this event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="event">The instance of the event</param>
        protected async Task HandleEventAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var eventSubscriptions = this.configuration.Subscriptions.GetEventSubscriptions(@event);
            var handlerTasks = eventSubscriptions.Select(s => s.HandleAsync(@event));

            await Task.WhenAll(handlerTasks);
        }
    }
}