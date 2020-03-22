//-------------------------------------------------------------------------------
// <copyright file="FinalOutgoingMessageStep.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The final outgoing message pipeline step
    /// </summary>
    public class FinalOutgoingMessageStep : OutgoingMessageStep
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinalOutgoingMessageStep"/> class.
        /// </summary>
        public FinalOutgoingMessageStep()
        {
            this.Name = "Final Outgoing Message Step";
        }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override Task InvokeAsync(OutgoingMessageContext context, Func<Task> next)
        {
            var messageIntent = context.Message.GetIntent();
            switch (messageIntent)
            {
                case MessageIntent.Command:
                    CreateCommandEnvelope(
                        context.Message as ICommand,
                        context.Configuration,
                        context.CreateEnvelope);
                    break;

                case MessageIntent.Event:
                    CreateEventEnvelopes(
                        context.Message as IEvent,
                        context.Configuration,
                        context.CreateEnvelope);
                    break;

                case MessageIntent.SubscriptionMessage:
                    CreateSubscriptionMessageEnvelope(
                        context.Message as SubscriptionMessage,
                        context.Configuration,
                        context.CreateEnvelope);
                    break;

                case MessageIntent.Unknown:
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Cannot create envelope for {messageIntent}");
            }

            return Task.CompletedTask;
        }

        private static void CreateCommandEnvelope(
            ICommand command,
            IHavePipelineConfiguration configuration,
            Action<EndpointAddress> createEnvelope)
        {
            var endpointAddress = configuration.GetConsumingEndpointAddress(command);
            createEnvelope(endpointAddress);
        }

        private static void CreateEventEnvelopes(
            IEvent @event,
            IHavePipelineConfiguration configuration,
            Action<EndpointAddress> createEnvelope)
        {
            foreach (var endpointAddress in configuration.GetSubscribedEndpointAddresses(@event))
            {
                createEnvelope(endpointAddress);
            }
        }

        private static void CreateSubscriptionMessageEnvelope(
            SubscriptionMessage subscriptionMessage,
            IHavePipelineConfiguration configuration,
            Action<EndpointAddress> createEnvelope)
        {
            var endpointAddress = configuration.GetPublishingEndpointAddress(subscriptionMessage.MessageType);
            createEnvelope(endpointAddress);
        }
    }
}