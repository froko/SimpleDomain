//-------------------------------------------------------------------------------
// <copyright file="MsmqProvider.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.MSMQ
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Messaging;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;

    using global::Common.Logging;
    
    /// <summary>
    /// The MSMQ message queue provider
    /// </summary>
    public class MsmqProvider : IMessageQueueProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MsmqProvider));

        private Func<Envelope, Task> callMeBackWhenEnvelopeArrives;
        private MessageQueue localQueue;
        
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private ConcurrentDictionary<Task, Task> handlerTasks;
        private Task localQueueReceptionTask;

        /// <inheritdoc />
        public string TransportMediumName => "MSMQ";

        /// <inheritdoc />
        public void Connect(EndpointAddress localEndpointAddress, Func<Envelope, Task> asyncEnvelopeReceivedCallback)
        {
            this.callMeBackWhenEnvelopeArrives = asyncEnvelopeReceivedCallback;
            this.localQueue = MsmqUtilities.GetMessageQueue(localEndpointAddress, QueueAccessMode.Receive);

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;

            this.handlerTasks = new ConcurrentDictionary<Task, Task>();
            this.localQueueReceptionTask = Task.Run(this.RunMessageReceptionTask, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task SendAsync(Envelope envelope)
        {
            var recipientEndpointAddress = envelope.GetHeader<EndpointAddress>(HeaderKeys.Recipient);

            try
            {
                return SendAsync(envelope, recipientEndpointAddress);
            }
            catch (Exception exception)
            {
                var message = $"Could not send {envelope.Body.GetIntent()} of type {envelope.Body.GetFullName()} to {recipientEndpointAddress}";
                throw new MsmqException(message, exception);
            }
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            this.cancellationTokenSource.Cancel();

            var allTasks = this.handlerTasks.Values.Concat(new[] { this.localQueueReceptionTask });
            Task.WhenAll(allTasks).Wait();

            this.handlerTasks.Clear();
            this.localQueue.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Disconnect();
        }

        private static Task SendAsync(Envelope envelope, EndpointAddress recipientEndpointAddress)
        {
            using (var recipientQueue = MsmqUtilities.GetMessageQueue(recipientEndpointAddress, QueueAccessMode.Send))
            {
                recipientQueue.Send(envelope, MsmqUtilities.GetTransactionType());
                return Task.CompletedTask;
            }
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
                    Logger.Warn("MSMQ receive task failed", exception);
                }
            }
        }

        private async Task ProcessMessagesAsync()
        {
            using (var enumerator = this.localQueue.GetMessageEnumerator2())
            {
                while (!this.cancellationToken.IsCancellationRequested)
                {
                    await this.ProcessMessagesAsync(enumerator).ConfigureAwait(false);
                }
            }
        }

        private Task ProcessMessagesAsync(MessageEnumerator enumerator)
        {
            try
            {
                if (!enumerator.MoveNext(TimeSpan.FromMilliseconds(10)))
                {
                    return Task.CompletedTask;
                }
            }
            catch (Exception exception)
            {
                Logger.Warn("MSMQ receive operation failed", exception);
                return Task.CompletedTask;
            }

            if (this.cancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            var handlerTask = this.HandleMessageAsync();
            this.handlerTasks.TryAdd(handlerTask, handlerTask);

            return handlerTask.ContinueWith(t =>
            {
                Task toBeRemoved;
                this.handlerTasks.TryRemove(t, out toBeRemoved);
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task HandleMessageAsync()
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    var message = this.localQueue.Receive();
                    await this.HandleMessageAsync(message).ConfigureAwait(false);

                    transactionScope.Complete();
                }
                catch (Exception exception)
                {
                    Logger.Error("Could not handle message", exception);
                }
            }
        }

        private async Task HandleMessageAsync(Message message)
        {
            if (message == null)
            {
                Logger.Warn("Received a NULL message. That's strange but should cause no error");
                return;
            }

            if (!(message.Body is Envelope))
            {
                Logger.Warn("Received a message that is not an Envelope. That's strange but should cause no error");
                return;
            }

            var envelope = (Envelope)message.Body;
            await this.callMeBackWhenEnvelopeArrives(envelope).ConfigureAwait(false);
        }
    }
}