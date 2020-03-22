//-------------------------------------------------------------------------------
// <copyright file="AuditQueueStep.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline
{
    using System;
    using System.Threading.Tasks;

    using SimpleDomain.Bus.Pipeline.Incomming;
    using SimpleDomain.Bus.RabbitMq;
    using SimpleDomain.Common;

    /// <summary>
    /// The incomming message step which records all incomming message to an audit queue
    /// </summary>
    public class AuditQueueStep : IncommingMessageStep
    {
        private readonly EndpointAddress auditQueue;
        private readonly ISendEnvelopesToMessageQueue messageQueueSender;

        private bool disableAuditForSubscriptionMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditQueueStep"/> class which uses RabbitMQ.
        /// </summary>
        /// <param name="auditQueue">The address of the audit queue</param>
        public AuditQueueStep(string auditQueue)
            : this(auditQueue, new RabbitMqSender())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditQueueStep"/> class which uses RabbitMQ.
        /// </summary>
        /// <param name="auditQueue">The address of the audit queue</param>
        /// <param name="username">The RabbitMQ user name</param>
        /// <param name="password">The RabbitMQ password</param>
        /// <param name="virtualHost">The RabbitMQ virtual host</param>
        /// <param name="port">The RabbitMQ port</param>
        public AuditQueueStep(
            string auditQueue,
            string username,
            string password,
            string virtualHost = RabbitMqProvider.DefaultVirtualHost,
            int port = RabbitMqProvider.DefaultPort) : this(auditQueue, new RabbitMqSender(username, password, virtualHost, port))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditQueueStep"/> class which uses MSMQ.
        /// </summary>
        /// <param name="auditQueue">The address of the audit queue</param>
        /// <param name="messageQueueSender">Dependency injection for <see cref="ISendEnvelopesToMessageQueue"/></param>
        public AuditQueueStep(string auditQueue, ISendEnvelopesToMessageQueue messageQueueSender)
        {
            Guard.NotNullOrEmpty(() => auditQueue);
            Guard.NotNull(() => messageQueueSender);

            this.auditQueue = new EndpointAddress(auditQueue);
            this.messageQueueSender = messageQueueSender;
            this.Name = $"Audit Queue Step ({this.auditQueue})";

            this.disableAuditForSubscriptionMessages = true;
        }

        /// <inheritdoc />
        public override string Name { get; }

        /// <summary>
        /// Audit subscription messages as well
        /// </summary>
        /// <returns>The audit queue step itself since this is a builder method</returns>
        public AuditQueueStep WithSubscriptionMessages()
        {
            this.disableAuditForSubscriptionMessages = false;
            return this;
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(IncommingMessageContext context, Func<Task> next)
        {
            await next().ConfigureAwait(false);
            this.SendToAuditQueue(context);
        }

        private void SendToAuditQueue(IncommingMessageContext context)
        {
            if (this.disableAuditForSubscriptionMessages && context.MessageIntent == MessageIntent.SubscriptionMessage)
            {
                return;
            }

            var envelope = context.Envelope.AddHeader(HeaderKeys.TimeProcessed, DateTime.UtcNow);
            this.messageQueueSender.Send(envelope, this.auditQueue);
        }
    }
}