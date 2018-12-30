//-------------------------------------------------------------------------------
// <copyright file="RabbitMqSender.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.RabbitMq
{
    using RabbitMQ.Client;

    /// <summary>
    /// The RabbitMQ envelope sender
    /// </summary>
    public class RabbitMqSender : ISendEnvelopesToMessageQueue
    {
        private const string DefaultExchange = "";

        private readonly string username;
        private readonly string password;
        private readonly string virtualHost;
        private readonly int port;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqSender"/> class.
        /// </summary>
        public RabbitMqSender() : this(RabbitMqProvider.DefaultUsername, RabbitMqProvider.DefaultPassword)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqSender"/> class.
        /// </summary>
        /// <param name="username">The RabbitMQ user name</param>
        /// <param name="password">The RabbitMQ password</param>
        /// <param name="virtualHost">The RabbitMQ virtual host</param>
        /// <param name="port">The RabbitMQ port</param>
        public RabbitMqSender(
            string username,
            string password,
            string virtualHost = RabbitMqProvider.DefaultVirtualHost,
            int port = RabbitMqProvider.DefaultPort)
        {
            this.username = username;
            this.password = password;
            this.virtualHost = virtualHost;
            this.port = port;
        }

        /// <inheritdoc />
        public void Send(Envelope envelope, EndpointAddress endpointAddress)
        {
            using (var connection = this.CreateConnection(endpointAddress))
            using (var channel = CreateChannel(connection, endpointAddress))
            {
                channel.BasicPublish(
                    DefaultExchange,
                    endpointAddress.QueueName,
                    channel.CreateBasicProperties(),
                    envelope.AsByteArray());
            }
        }

        private IConnection CreateConnection(EndpointAddress endpointAddress)
        {
            return RabbitMqUtilities.CreateConnection(
                this.username,
                this.password,
                this.virtualHost,
                this.port,
                endpointAddress);
        }

        private static IModel CreateChannel(IConnection connection, EndpointAddress endpointAddress)
        {
            return RabbitMqUtilities.CreateChannel(connection, endpointAddress);
        }
    }
}