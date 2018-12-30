//-------------------------------------------------------------------------------
// <copyright file="RabbitMqJitneyConfigurationExtensions.cs" company="frokonet.ch">
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
    using SimpleDomain.Bus.RabbitMq;

    /// <summary>
    /// Configuation extensions for the Jitney configuration base class
    /// </summary>
    public static class RabbitMqJitneyConfigurationExtensions
    {
        /// <summary>
        /// Registers the <see cref="MessageQueueJitney"/> bus with the RabbitMQ provider
        /// </summary>
        /// <param name="configuration">The Jintey bus configuration</param>
        public static void UseRabbitMqJitney(this IConfigureThisJitney configuration)
        {
            configuration.AddConfigurationItem(MessageQueueJitney.MessageQueueProvider, new RabbitMqProvider());
            configuration.Register(config => new MessageQueueJitney(config));
        }

        /// <summary>
        /// Registers the <see cref="MessageQueueJitney"/> bus with the RabbitMQ provider
        /// </summary>
        /// <param name="configuration">The Jintey bus configuration</param>
        /// <param name="username">The RabbitMQ user name</param>
        /// <param name="password">The RabbitMQ password</param>
        /// <param name="virtualHost">The RabbitMQ virtual host</param>
        /// <param name="port">The RabbitMQ port</param>
        public static void UseRabbitMqJitney(
            this IConfigureThisJitney configuration,
            string username,
            string password,
            string virtualHost = RabbitMqProvider.DefaultVirtualHost,
            int port = RabbitMqProvider.DefaultPort)
        {
            configuration.AddConfigurationItem(
                MessageQueueJitney.MessageQueueProvider,
                new RabbitMqProvider(username, password, virtualHost, port));

            configuration.Register(config => new MessageQueueJitney(config));
        }
    }
}