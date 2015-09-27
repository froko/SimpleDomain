//-------------------------------------------------------------------------------
// <copyright file="JitneySubscriptions.cs" company="frokonet.ch">
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
    using System.Reflection;
    using System.Threading.Tasks;

    using SimpleDomain.Common;

    /// <summary>
    /// The command and event subscription store
    /// </summary>
    public class JitneySubscriptions : ISubscribeHandlers
    {
        private readonly IRegisterTypes typeRegistrar;
        private readonly IResolveTypes typeResolver;

        private readonly IList<Subscription> commandSubscriptions;
        private readonly IList<Subscription> eventSubscriptions;

        /// <summary>
        /// Creates a new instance of <see cref="JitneySubscriptions"/>
        /// </summary>
        /// <param name="typeRegistrar">Dependency injection for <see cref="IRegisterTypes"/></param>
        /// <param name="typeResolver">Dependency injection for <see cref="IResolveTypes"/></param>
        public JitneySubscriptions(IRegisterTypes typeRegistrar, IResolveTypes typeResolver)
        {
            this.typeRegistrar = typeRegistrar;
            this.typeResolver = typeResolver;

            this.commandSubscriptions = new List<Subscription>();
            this.eventSubscriptions = new List<Subscription>();
        }

        /// <inheritdoc />
        public void SubscribeAllHandlersInThisAssembly()
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            this.RegisterCommandHandlers(callingAssembly);
            this.RegisterEventHandlers(callingAssembly);
        }

        /// <inheritdoc />
        public void Subscribe<TMessage, THandler>() where TMessage : IMessage where THandler : IHandleAsync<TMessage>
        {
            this.typeRegistrar.Register<IHandleAsync<TMessage>, THandler>();
        }

        /// <inheritdoc />
        public void SubscribeCommandHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : ICommand
        {
            Guard.NotNull(() => handler);

            if (this.commandSubscriptions.Any(s => s.CanHandle<TCommand>()))
            {
                throw new CommandSubscriptionException<TCommand>();
            }

            this.commandSubscriptions.Add(new CommandSubscription<TCommand>(handler));
        }

        /// <inheritdoc />
        public void SubscribeEventHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Guard.NotNull(() => handler);

            this.eventSubscriptions.Add(new EventSubscription<TEvent>(handler));
        }

        /// <summary>
        /// Gets the subscription for a given command
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="command">The instance of the command</param>
        /// <returns>A command subscription</returns>
        public virtual Subscription GetCommandSubscription<TCommand>(TCommand command) where TCommand : ICommand
        {
            var subscription = this.commandSubscriptions.SingleOrDefault(s => s.CanHandle<TCommand>());
            if (subscription != null)
            {
                return subscription;
            }

            var handler = this.typeResolver.Resolve<IHandleAsync<TCommand>>();
            if (handler != null)
            {
                return new CommandSubscription<TCommand>(handler.HandleAsync);
            }

            throw new NoSubscriptionException(command);
        }

        /// <summary>
        /// Gets a list of subscriptions for a given event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="@event">The instance of the event</param>
        /// <returns>A list of event subscriptions</returns>
        public virtual IEnumerable<Subscription> GetEventSubscriptions<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var subscriptions = this.eventSubscriptions.Where(s => s.CanHandle<TEvent>());
            var handlers = this.typeResolver.ResolveAll<IHandleAsync<TEvent>>();

            return subscriptions.Union(handlers.Select(h => new EventSubscription<TEvent>(h.HandleAsync)));
        }

        private void RegisterCommandHandlers(Assembly callingAssembly)
        {
            foreach (var classType in callingAssembly.GetTypes().Where(t => t.IsClass))
            {
                foreach (var interfaceType in classType.GetCommandHandlerInterfaces())
                {
                    this.typeRegistrar.Register(interfaceType, classType);
                }
            }
        }

        private void RegisterEventHandlers(Assembly callingAssembly)
        {
            foreach (var classType in callingAssembly.GetTypes().Where(t => t.IsClass))
            {
                foreach (var interfaceType in classType.GetEventHandlerInterfaces())
                {
                    this.typeRegistrar.Register(interfaceType, classType);
                }
            }
        }
    }
}