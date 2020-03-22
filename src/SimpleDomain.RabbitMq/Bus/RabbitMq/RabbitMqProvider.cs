//-------------------------------------------------------------------------------
// <copyright file="RabbitMqProvider.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.RabbitMq
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    using SimpleDomain.Common.Logging;

    /// <summary>
    /// The RabbitMQ message queue provider
    /// </summary>
    public class RabbitMqProvider : IMessageQueueProvider
    {
        /// <summary>
        /// Default RabbitMQ user name
        /// </summary>
        public const string DefaultUsername = "guest";

        /// <summary>
        /// Default RabbitMQ password
        /// </summary>
        public const string DefaultPassword = "guest";

        /// <summary>
        /// Default RabbitMQ virtual host
        /// </summary>
        public const string DefaultVirtualHost = "/";

        /// <summary>
        /// Default RabbitMQ port
        /// </summary>
        public const int DefaultPort = 5672;

        private const string DefaultExchange = "";

        private static readonly ILogger Logger = LoggerFactory.Create<RabbitMqProvider>();

        private readonly string username;
        private readonly string password;
        private readonly string virtualHost;
        private readonly int port;

        private Func<Envelope, Task> callMeBackWhenEnvelopeArrives;
        private IConnection connection;
        private IModel receivingChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqProvider"/> class.
        /// </summary>
        public RabbitMqProvider() : this(DefaultUsername, DefaultPassword)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqProvider"/> class.
        /// </summary>
        /// <param name="username">The RabbitMQ user name</param>
        /// <param name="password">The RabbitMQ password</param>
        /// <param name="virtualHost">The RabbitMQ virtual host</param>
        /// <param name="port">The RabbitMQ port</param>
        public RabbitMqProvider(
            string username,
            string password,
            string virtualHost = DefaultVirtualHost,
            int port = DefaultPort)
        {
            this.username = username;
            this.password = password;
            this.virtualHost = virtualHost;
            this.port = port;
        }

        /// <inheritdoc />
        public string TransportMediumName => "RabbitMQ";

        /// <inheritdoc />
        public void Connect(EndpointAddress localEndpointAddress, Func<Envelope, Task> asyncEnvelopeReceivedCallback)
        {
            this.callMeBackWhenEnvelopeArrives = asyncEnvelopeReceivedCallback;

            this.connection = RabbitMqUtilities.CreateConnection(
                this.username,
                this.password,
                this.virtualHost,
                this.port,
                localEndpointAddress);

            this.receivingChannel = RabbitMqUtilities.CreateChannel(this.connection, localEndpointAddress);

            var consumer = new AsyncEventingBasicConsumer(this.receivingChannel);
            consumer.Received += this.HandleMessageAsync;

            this.receivingChannel.BasicConsume(localEndpointAddress.QueueName, true, consumer);
        }

        /// <inheritdoc />
        public Task SendAsync(Envelope envelope)
        {
            var recipientEndpointAddress = envelope.GetHeader<EndpointAddress>(HeaderKeys.Recipient);

            try
            {
                return Task.Run(() => this.Send(envelope, recipientEndpointAddress));
            }
            catch (Exception exception)
            {
                var message = $"Could not send {envelope.Body.GetIntent()} of type {envelope.Body.GetFullName()} to {recipientEndpointAddress}";
                throw new RabbitMqException(message, exception);
            }
        }

        /// <inheritdoc />
        public Task DisconnectAsync()
        {
            this.connection.Close();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.receivingChannel.Dispose();
            this.connection.Dispose();
        }

        private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs @event)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    await this.HandleMessageAsync(@event.Body).ConfigureAwait(false);
                    transactionScope.Complete();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, "Could not handle message");
                }
            }
        }

        private async Task HandleMessageAsync(byte[] body)
        {
            var message = body.Deserialize();
            if (!(message is Envelope))
            {
                Logger.Warn("Received a message that is not an Envelope. That's strange but should cause no error");
                return;
            }

            var envelope = (Envelope)message;
            await this.callMeBackWhenEnvelopeArrives(envelope).ConfigureAwait(false);
        }

        private void Send(Envelope envelope, EndpointAddress recipientEndpointAddress)
        {
            using (var channel = RabbitMqUtilities.CreateChannel(this.connection, recipientEndpointAddress))
            {
                channel.BasicPublish(
                    DefaultExchange,
                    recipientEndpointAddress.QueueName,
                    channel.CreateBasicProperties(),
                    envelope.AsByteArray());
            }
        }
    }
}