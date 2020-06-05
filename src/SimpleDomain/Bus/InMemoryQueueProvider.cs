//-------------------------------------------------------------------------------
// <copyright file="InMemoryQueueProvider.cs" company="frokonet.ch">
//   Copyright (c) 2014-2017
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
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;

    using SimpleDomain.Common.Logging;

    /// <summary>
    /// The In-Memory Queue provider
    /// </summary>
    public sealed class InMemoryQueueProvider : IMessageQueueProvider
    {
        private static readonly ILogger Logger = LoggerFactory.Create<InMemoryQueueProvider>();

        private Func<Envelope, Task> callMeBackWhenEnvelopeArrives;
        private ConcurrentQueue<Envelope> queue;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private ConcurrentDictionary<Task, Task> handlerTasks;
        private Task localQueueReceptionTask;

        /// <inheritdoc />
        public string TransportMediumName => "In-Memory Queue";

        /// <inheritdoc />
        public void Connect(EndpointAddress localEndpointAddress, Func<Envelope, Task> asyncEnvelopeReceivedCallback)
        {
            this.callMeBackWhenEnvelopeArrives = asyncEnvelopeReceivedCallback;
            this.queue = new ConcurrentQueue<Envelope>();

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;

            this.handlerTasks = new ConcurrentDictionary<Task, Task>();
            this.localQueueReceptionTask = Task.Run(this.RunMessageReceptionTask, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task SendAsync(Envelope envelope)
        {
            this.queue.Enqueue(envelope);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            this.cancellationTokenSource.Cancel();

            var allTasks = this.handlerTasks.Values.Concat(new[] { this.localQueueReceptionTask });
            await Task.WhenAll(allTasks).ConfigureAwait(false);

            this.handlerTasks.Clear();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.cancellationTokenSource.Dispose();
            this.DisconnectAsync().Wait(TimeSpan.FromSeconds(30));
        }

        private static TransactionScope CreateTransactionScope()
        {
            return new TransactionScope(
                TransactionScopeOption.RequiresNew,
                TransactionScopeAsyncFlowOption.Enabled);
        }

        private async Task RunMessageReceptionTask()
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await this.ProcessMessagesAsync().ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Do nothing here. This belongs to the shutdown process.
                }
                catch (Exception exception)
                {
                    Logger.Warn(exception, "In-Memory Queue receive task failed");
                }
            }
        }

        private Task ProcessMessagesAsync()
        {
            Envelope envelope;
            if (!this.queue.TryDequeue(out envelope))
            {
                return Task.CompletedTask;
            }

            var handlerTask = this.HandleMessageAsync(envelope);
            this.handlerTasks.TryAdd(handlerTask, handlerTask);

            return handlerTask.ContinueWith(t =>
            {
                this.handlerTasks.TryRemove(t, out _);
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task HandleMessageAsync(Envelope envelope)
        {
            using (var transactionScope = CreateTransactionScope())
            {
                try
                {
                    await this.callMeBackWhenEnvelopeArrives(envelope).ConfigureAwait(false);
                    transactionScope.Complete();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, "Could not handle message");
                }
            }
        }
    }
}