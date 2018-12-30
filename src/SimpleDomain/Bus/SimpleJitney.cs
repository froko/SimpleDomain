//-------------------------------------------------------------------------------
// <copyright file="SimpleJitney.cs" company="frokonet.ch">
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
    /// The most simple <see cref="Jitney"/> you may can think of
    /// </summary>
    public class SimpleJitney : Jitney
    {
        private static readonly ILogger Logger = LoggerFactory.Create<Jitney>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleJitney"/> class.
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveJitneyConfiguration"/></param>
        public SimpleJitney(IHaveJitneyConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public override async Task StartAsync()
        {
            Logger.Debug(this.Configuration.GetSummary(this.GetType()));
            await this.SendSubscriptionMessagesAsync().ConfigureAwait(false);

            Logger.Info("SimpleJitney has been started");
        }

        /// <inheritdoc />
        public override Task StopAsync()
        {
            Logger.Info("SimpleJitney has been stopped");

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task SendAsync<TCommand>(TCommand command)
        {
            Guard.NotNull(() => command);

            var outgoingPipeline = this.Configuration.CreateOutgoingPipeline(this.HandleAsync);
            return outgoingPipeline.InvokeAsync(command);
        }

        /// <inheritdoc />
        public override Task PublishAsync<TEvent>(TEvent @event)
        {
            Guard.NotNull(() => @event);

            var outgoingPipeline = this.Configuration.CreateOutgoingPipeline(this.HandleAsync);
            return outgoingPipeline.InvokeAsync(@event);
        }

        private async Task SendSubscriptionMessagesAsync()
        {
            foreach (var eventType in this.Configuration.Subscriptions.GetSubscribedEventTypes())
            {
                var outgoingPipeline = this.Configuration.CreateOutgoingPipeline(this.HandleAsync);
                await outgoingPipeline
                    .InvokeAsync(new SubscriptionMessage(this.Configuration.LocalEndpointAddress, eventType.FullName))
                    .ConfigureAwait(false);
            }
        }
    }
}