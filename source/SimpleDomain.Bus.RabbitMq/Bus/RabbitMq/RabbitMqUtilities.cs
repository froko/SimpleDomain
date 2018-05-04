//-------------------------------------------------------------------------------
// <copyright file="RabbitMqUtilities.cs" company="frokonet.ch">
//   Copyright (c) 2014-2018
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

    public static class RabbitMqUtilities
    {
        public static IConnection CreateConnection(
            string username,
            string password,
            string virtualHost,
            int port,
            EndpointAddress endpointAddress)
        {
            var connectionFactory = new ConnectionFactory
            {
                UserName = username,
                Password = password,
                VirtualHost = virtualHost,
                Port = port,
                HostName = endpointAddress.MachineName,
                DispatchConsumersAsync = true
            };

            return connectionFactory.CreateConnection();
        }

        public static IModel CreateChannel(IConnection connection, EndpointAddress endpointAddress)
        {
            var channel = connection.CreateModel();
            channel.QueueDeclare(endpointAddress.QueueName, true, false, false, null);

            return channel;
        }
    }
}