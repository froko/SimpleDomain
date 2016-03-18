//-------------------------------------------------------------------------------
// <copyright file="FileSubscriptionStore.cs" company="frokonet.ch">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using SimpleDomain.Common;

    /// <summary>
    /// A subscription store which persists entries to a file called subscriptions.txt
    /// </summary>
    public class FileSubscriptionStore : ISubscriptionStore
    {
        private const string FileName = "subscriptions.txt";

        private readonly string subscriptionFile;
        private Dictionary<string, List<EndpointAddress>> subscriptions;

        /// <summary>
        /// Creates a new instance of <see cref="FileSubscriptionStore"/>
        /// </summary>
        public FileSubscriptionStore()
        {
            this.subscriptionFile = Path.Combine(HomeDirectory, FileName);

            try
            {
                this.LoadSubscriptionsFromFile();
            }
            catch (Exception)
            {
                this.subscriptions = new Dictionary<string, List<EndpointAddress>>();
            }
        }

        private static string HomeDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }

        /// <inheritdoc />
        public async Task SaveAsync(SubscriptionMessage subscriptionMessage)
        {
            Guard.NotNull(() => subscriptionMessage);

            var messageType = subscriptionMessage.MessageType;
            var handlingEndpoint = subscriptionMessage.HandlingEndpointAddress;

            List<EndpointAddress> endpoints;

            if (this.subscriptions.TryGetValue(messageType, out endpoints))
            {
                if (endpoints.Any(e => e.QueueName == handlingEndpoint.QueueName))
                {
                    return;
                }

                endpoints.Add(handlingEndpoint);
            }
            else
            {
                this.subscriptions.Add(messageType, new List<EndpointAddress> { handlingEndpoint });
            }

            await this.SaveSubscriptionsToFileAsync();
        }

        /// <inheritdoc />
        public IEnumerable<EndpointAddress> GetSubscribedEndpoints(IEvent @event)
        {
            var messageType = @event.GetFullName();

            return this.subscriptions[messageType];
        }

        /// <summary>
        /// Deletes the subscription file and makes the using endpoint forget about subscriptions
        /// </summary>
        public void Clear()
        {
            try
            {
                this.subscriptions.Clear();
                File.Delete(this.subscriptionFile);
            }
            catch (Exception exception)
            {
                ExceptionHelper.Eat(exception);
            }
        }

        private void LoadSubscriptionsFromFile()
        {
            using (var fileReader = new StreamReader(this.subscriptionFile))
            {
                this.Deserialize(fileReader.ReadToEnd());
            }
        }

        private async Task SaveSubscriptionsToFileAsync()
        {
            using (var fileWriter = new StreamWriter(this.subscriptionFile, false))
            {
                await fileWriter.WriteAsync(this.Serialize());
            }
        }

        private string Serialize()
        {
            var writer = new StringWriter();

            foreach (var key in this.subscriptions.Keys)
            {
                writer.Write("{0}: ", key);
                this.subscriptions[key].ForEach(endpoint => writer.Write("{0}; ", endpoint.ToString()));
                writer.WriteLine();
            }

            return writer.ToString();
        }

        private void Deserialize(string fileContent)
        {
            this.subscriptions = new Dictionary<string, List<EndpointAddress>>();

            var lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var messageType = line.Split(':')[0].Trim();
                var queueNames = line.Split(':')[1].Trim();
                var endpoints = queueNames.Split(';').Where(s => !string.IsNullOrEmpty(s)).Select(queue => EndpointAddress.Parse(queue.Trim())).ToList();

                this.subscriptions.Add(messageType, endpoints);
            }
        }
    }
}