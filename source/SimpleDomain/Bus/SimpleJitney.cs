//-------------------------------------------------------------------------------
// <copyright file="SimpleJitney.cs" company="frokonet.ch">
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
    using System.Threading.Tasks;

    using global::Common.Logging;

    using SimpleDomain.Common;

    /// <summary>
    /// The most simple <see cref="Jitney"/> you may can think of
    /// </summary>
    public class SimpleJitney : Jitney
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Jitney));

        /// <summary>
        /// Creates a new instance of <see cref="SimpleJitney"/>
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveJitneyConfiguration"/></param>
        public SimpleJitney(IHaveJitneyConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public override async Task StartAsync()
        {
            foreach (var eventType in this.Configuration.Subscriptions.GetSubscribedEventTypes())
            {
                var outgoingPipeline = this.Configuration.CreateOutgoingPipeline(this.HandleAsync);
                await outgoingPipeline
                    .InvokeAsync(new SubscriptionMessage(this.Configuration.LocalEndpointAddress, eventType.FullName))
                    .ConfigureAwait(false);
            }

            Logger.Info("SimpleJitney has been started");
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
    }
}