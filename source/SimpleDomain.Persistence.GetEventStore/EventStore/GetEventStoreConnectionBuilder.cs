//-------------------------------------------------------------------------------
// <copyright file="GetEventStoreConnectionBuilder.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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

namespace SimpleDomain.EventStore
{
    using System.Net;
    using System.Threading.Tasks;

    using global::EventStore.ClientAPI;
    using global::EventStore.ClientAPI.SystemData;

    /// <summary>
    /// The GetEventStore connection builder
    /// </summary>
    public class GetEventStoreConnectionBuilder : IConnectionBuilder
    {
        private const string DefaultUserName = "admin";
        private const string DefaultPassword = "changeit";
        private const int DefaultPort = 1113;

        private UserCredentials credentials;
        private IPEndPoint endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEventStoreConnectionBuilder"/> class.
        /// </summary>
        private GetEventStoreConnectionBuilder()
        {
            this.credentials = new UserCredentials(DefaultUserName, DefaultPassword);
            this.endpoint = new IPEndPoint(IPAddress.Loopback, DefaultPort);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GetEventStoreConnectionBuilder"/>
        /// </summary>
        /// <returns>A new instance of <see cref="GetEventStoreConnectionBuilder"/></returns>
        public static GetEventStoreConnectionBuilder Initialize()
        {
            return new GetEventStoreConnectionBuilder();
        }

        /// <summary>
        /// Defines the authentication
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>The event store connection builder itself</returns>
        public GetEventStoreConnectionBuilder AuthenticateWith(string userName, string password)
        {
            this.credentials = new UserCredentials(userName, password);
            return this;
        }

        /// <summary>
        /// Defines the connection endpoint of the event store service
        /// </summary>
        /// <param name="ipAddress">The IP address of the event store service</param>
        /// <param name="port">The port of the event store service</param>
        /// <returns>The event store connection builder itself</returns>
        public GetEventStoreConnectionBuilder ConnectTo(string ipAddress, int port = DefaultPort)
        {
            this.endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            return this;
        }

        /// <inheritdoc />
        public async Task<IEventStoreConnection> BuildAsync()
        {
            var settings = ConnectionSettings.Create()
                .UseConsoleLogger()
                .KeepReconnecting()
                .SetDefaultUserCredentials(this.credentials);

            var connection = EventStoreConnection.Create(settings, this.endpoint);

            await connection.ConnectAsync().ConfigureAwait(false);

            return connection;
        }
    }
}