//-------------------------------------------------------------------------------
// <copyright file="InMemorySubscriptionStore.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The in-memory subscription store
    /// </summary>
    public class InMemorySubscriptionStore : ISubscriptionStore
    {
        private readonly Dictionary<string, List<EndpointAddress>> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySubscriptionStore"/> class.
        /// </summary>
        public InMemorySubscriptionStore()
        {
            this.subscriptions = new Dictionary<string, List<EndpointAddress>>();
        }

        /// <inheritdoc />
        public Task SaveAsync(SubscriptionMessage subscriptionMessage)
        {
            var messageType = subscriptionMessage.MessageType;
            var handlingEndpoint = subscriptionMessage.HandlingEndpointAddress;

            List<EndpointAddress> endpoints;

            if (this.subscriptions.TryGetValue(messageType, out endpoints))
            {
                if (endpoints.Any(e => e.QueueName == handlingEndpoint.QueueName))
                {
                    return Task.CompletedTask;
                }

                endpoints.Add(handlingEndpoint);
            }
            else
            {
                this.subscriptions.Add(messageType, new List<EndpointAddress> { handlingEndpoint });
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public IEnumerable<EndpointAddress> GetSubscribedEndpoints(IEvent @event)
        {
            var messageType = @event.GetFullName();

            return this.subscriptions[messageType];
        }
    }
}