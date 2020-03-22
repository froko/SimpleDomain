//-------------------------------------------------------------------------------
// <copyright file="JitneySubscriptions.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using SimpleDomain.Common;

    /// <summary>
    /// The Jitney subscription holder
    /// </summary>
    public class JitneySubscriptions : IHaveJitneySubscriptions
    {
        private readonly AbstractHandlerRegistry handlerRegistry;
        private readonly IHandlerInvocationCache handlerInvocationCache;
        private readonly IList<Subscription> commandSubscriptions;
        private readonly IList<Subscription> eventSubscriptions;
        private readonly IList<Type> eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="JitneySubscriptions"/> class.
        /// </summary>
        /// <param name="handlerRegistry">Dependency injection for <see cref="AbstractHandlerRegistry"/></param>
        /// <param name="handlerInvocationCache">Dependency injection for <see cref="IHandlerInvocationCache"/></param>
        public JitneySubscriptions(
            AbstractHandlerRegistry handlerRegistry,
            IHandlerInvocationCache handlerInvocationCache)
        {
            this.handlerRegistry = handlerRegistry;
            this.handlerInvocationCache = handlerInvocationCache;
            this.commandSubscriptions = new List<Subscription>();
            this.eventSubscriptions = new List<Subscription>();
            this.eventTypes = new List<Type>();
        }

        /// <summary>
        /// Adds an async command handling action
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="handler">The async command handling action</param>
        public void AddCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
        {
            Guard.NotNull(() => handler);

            if (this.commandSubscriptions.Any(s => s.CanHandle<TCommand>()))
            {
                throw new CommandSubscriptionException<TCommand>();
            }

            this.commandSubscriptions.Add(new CommandSubscription<TCommand>(handler));
        }

        /// <summary>
        /// Adds an async event handling action
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="handler">An async event handling action</param>
        public void AddEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Guard.NotNull(() => handler);

            this.eventSubscriptions.Add(new EventSubscription<TEvent>(handler));
            this.eventTypes.Add(typeof(TEvent));
        }

        /// <summary>
        /// Scans a given assembly for message handlers and registers them in the IoC container
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="registerInContainer">The IoC container registration action</param>
        public void ScanAssemblyForMessageHandlers(Assembly assembly, Action<Type> registerInContainer)
        {
            Guard.NotNull(() => assembly);

            foreach (var asyncHandlerType in assembly.GetAsyncHandlerTypes())
            {
                this.RegisterHandlerType(asyncHandlerType);
                registerInContainer(asyncHandlerType);
            }
        }

        /// <inheritdoc />
        public virtual Subscription GetCommandSubscription<TCommand>(TCommand command) where TCommand : ICommand
        {
            var subscription = this.commandSubscriptions.SingleOrDefault(s => s.CanHandle(command));
            if (subscription != null)
            {
                return subscription;
            }

            var handler = this.handlerRegistry.GetCommandHandler(command);
            if (handler != null)
            {
                return new CommandSubscription<TCommand>(message => this.handlerInvocationCache.InvokeAsync(handler, message));
            }

            throw new NoSubscriptionException(command);
        }

        /// <inheritdoc />
        public virtual IEnumerable<Subscription> GetEventSubscriptions<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var subscriptions = this.eventSubscriptions.Where(s => s.CanHandle(@event));
            var handlers = this.handlerRegistry.GetEventHandlers(@event);
            var handlerSubscriptions = handlers.Select(handler =>
                new EventSubscription<TEvent>(message => this.handlerInvocationCache.InvokeAsync(handler, message)));

            return subscriptions.Union(handlerSubscriptions);
        }

        /// <inheritdoc />
        public IEnumerable<Type> GetSubscribedEventTypes()
        {
            return this.eventTypes.Distinct();
        }

        private void RegisterHandlerType(Type asyncHandlerType)
        {
            foreach (var messageType in asyncHandlerType.GetAllMessageTypeThisTypeCanHandle())
            {
                this.handlerRegistry.Register(asyncHandlerType, messageType);
                this.handlerInvocationCache.Add(asyncHandlerType, messageType);

                if (typeof(IEvent).IsAssignableFrom(messageType))
                {
                    this.eventTypes.Add(messageType);
                }
            }
        }
    }
}