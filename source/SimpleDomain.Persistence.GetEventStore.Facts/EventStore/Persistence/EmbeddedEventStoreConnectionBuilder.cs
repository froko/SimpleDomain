//-------------------------------------------------------------------------------
// <copyright file="EmbeddedEventStoreConnectionBuilder.cs" company="frokonet.ch">
//   Copyright (c) 2014-2016
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using global::EventStore.ClientAPI;
    using global::EventStore.ClientAPI.Embedded;
    using global::EventStore.Core;
    using global::EventStore.Core.Data;

    public class EmbeddedEventStoreConnectionBuilder : IConnectionBuilder
    {
        private readonly ClusterVNode node;

        public EmbeddedEventStoreConnectionBuilder()
        {
            this.node = EmbeddedVNodeBuilder.AsSingleNode().RunInMemory().OnDefaultEndpoints().Build();
            this.WaitToBecomeMasterNode();
        }

        public async Task<IEventStoreConnection> BuildAsync()
        {
            var connection = EmbeddedEventStoreConnection.Create(this.node);
            await connection.ConnectAsync();

            return connection;
        }

        private void WaitToBecomeMasterNode()
        {
            var isMasterNode = false;
            var stopwatch = new Stopwatch();

            this.node.NodeStatusChanged += (sender, eventArgs) => { isMasterNode = eventArgs.NewVNodeState == VNodeState.Master; };
            this.node.Start();
            
            stopwatch.Start();

            while (!isMasterNode)
            {
                if (stopwatch.Elapsed.Seconds > 20)
                {
                    throw new InvalidOperationException("Waited too long for EventStore to become master");
                }

                Thread.Sleep(1);
            }

            stopwatch.Stop();
        }
    }
}