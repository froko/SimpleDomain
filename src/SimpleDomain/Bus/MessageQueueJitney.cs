//-------------------------------------------------------------------------------
// <copyright file="MessageQueueJitney.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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
    using System.Threading.Tasks;

    using SimpleDomain.Common;
    using SimpleDomain.Common.Logging;

    /// <summary>
    /// The Jitney implementation which uses a message queue infrastructure
    /// </summary>
    public class MessageQueueJitney : Jitney
    {
        /// <summary>
        /// Gets the configuration key for the message queue provider
        /// </summary>
        public const string MessageQueueProvider = "MessageQueueProvider";

        private static readonly ILogger Logger = LoggerFactory.Create<Jitney>();

        private readonly IMessageQueueProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueueJitney"/> class.
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveJitneyConfiguration"/></param>
        public MessageQueueJitney(IHaveJitneyConfiguration configuration)
            : base(configuration)
        {
            this.provider = this.Configuration.Get<IMessageQueueProvider>(MessageQueueProvider);
        }

        /// <inheritdoc />
        public override async Task StartAsync()
        {
            Logger.Debug(this.Configuration.GetSummary(this.GetType()));
            this.provider.Connect(this.Configuration.LocalEndpointAddress, this.HandleAsync);
            await this.SendSubscriptionMessagesAsync().ConfigureAwait(false);

            Logger.InfoFormat("MessageQueueJitney has been started with {0} as transport medium", this.provider.TransportMediumName);
        }

        /// <inheritdoc />
        public override async Task StopAsync()
        {
            await this.provider.DisconnectAsync().ConfigureAwait(false);

            Logger.Info("MessageQueueJitney has been stopped");
        }

        /// <inheritdoc />
        public override Task SendAsync<TCommand>(TCommand command)
        {
            Guard.NotNull(() => command);

            var outgoingPipeline = this.Configuration.CreateOutgoingPipeline(this.provider.SendAsync);
            return outgoingPipeline.InvokeAsync(command);
        }

        /// <inheritdoc />
        public override Task PublishAsync<TEvent>(TEvent @event)
        {
            Guard.NotNull(() => @event);

            var outgoingPipeline = this.Configuration.CreateOutgoingPipeline(this.provider.SendAsync);
            return outgoingPipeline.InvokeAsync(@event);
        }

        private async Task SendSubscriptionMessagesAsync()
        {
            foreach (var eventType in this.Configuration.Subscriptions.GetSubscribedEventTypes())
            {
                var outgoingPipeline = this.Configuration.CreateOutgoingPipeline(this.provider.SendAsync);
                await outgoingPipeline
                    .InvokeAsync(new SubscriptionMessage(this.Configuration.LocalEndpointAddress, eventType.FullName))
                    .ConfigureAwait(false);
            }
        }
    }
}