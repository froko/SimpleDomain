//-------------------------------------------------------------------------------
// <copyright file="Jitney.cs" company="frokonet.ch">
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
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;

    using SimpleDomain.Common;

    /// <summary>
    /// The abstract bus
    /// </summary>
    public abstract class Jitney : IDeliverMessages
    {
        /// <summary>
        /// Creates a new instance of <see cref="Jitney"/>
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveJitneyConfiguration"/></param>
        protected Jitney(IHaveJitneyConfiguration configuration)
        {
            Guard.NotNull(() => configuration);
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the Jitney configuration
        /// </summary>
        protected IHaveJitneyConfiguration Configuration { get; }

        /// <summary>
        /// Starts the message reception process
        /// </summary>
        public abstract Task StartAsync();

        /// <summary>
        /// Stops the message reception process
        /// </summary>
        /// <returns></returns>
        public abstract Task StopAsync();

        /// <inheritdoc />
        public abstract Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;

        /// <inheritdoc />
        public abstract Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;
        
        /// <summary>
        /// Handles an incomming envelope
        /// </summary>
        /// <param name="envelope">The envelope</param>
        protected async Task HandleAsync(Envelope envelope)
        {
            using (var transactionScope = CreateTransactionScope())
            {
                var incommingPipeline = this.Configuration.CreateIncommingPipeline(
                    this.HandleCommandAsync,
                    this.HandleEventAsync,
                    this.HandleSubscriptionMessageAsync);

                await incommingPipeline.InvokeAsync(envelope).ConfigureAwait(false);

                transactionScope.Complete();
            }
        }

        private static TransactionScope CreateTransactionScope()
        {
            return new TransactionScope(
                TransactionScopeOption.Required,
                TransactionScopeAsyncFlowOption.Enabled);
        }

        private async Task HandleCommandAsync(ICommand command)
        {
            var commandSubscription = this.Configuration.Subscriptions.GetCommandSubscription(command);
            await commandSubscription.HandleAsync(command).ConfigureAwait(false);
        }

        private async Task HandleEventAsync(IEvent @event)
        {
            var eventSubscriptions = this.Configuration.Subscriptions.GetEventSubscriptions(@event);
            var handlerTasks = eventSubscriptions.Select(s => s.HandleAsync(@event));

            await Task.WhenAll(handlerTasks).ConfigureAwait(false);
        }

        private Task HandleSubscriptionMessageAsync(SubscriptionMessage subscriptionMessage)
        {
            return this.Configuration.SubscriptionStore.SaveAsync(subscriptionMessage);
        }
    }
}