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
    using System.Messaging;
    using System.Threading.Tasks;
    using System.Transactions;

    using global::Common.Logging;

    using Nito.AsyncEx;

    /// <summary>
    /// The MSMQ message queue provider
    /// </summary>
    public class MsmqProvider : IMessageQueueProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MsmqProvider));

        private Func<Envelope, Task> callMeBackWhenEnvelopeArrives;
        private MessageQueue localQueue;

        /// <inheritdoc />
        public string TransportMediumName => "MSMQ";

        /// <inheritdoc />
        public void Connect(EndpointAddress localEndpointAddress, Func<Envelope, Task> asyncEnvelopeReceivedCallback)
        {
            this.callMeBackWhenEnvelopeArrives = asyncEnvelopeReceivedCallback;

            this.localQueue = MsmqUtilities.GetMessageQueue(localEndpointAddress, QueueAccessMode.Receive);
            this.localQueue.PeekCompleted += this.PeekCompleted;
            this.localQueue.BeginPeek();
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
        public void Dispose()
        {
            this.localQueue.Dispose();
        }

        private void PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    AsyncContext.Run(this.HandleMessageAsync);
                    transactionScope.Complete();
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Could not handle message", exception);
            }
            finally
            {
                this.localQueue.Refresh();
                this.localQueue.BeginPeek();
            }
        }

        private async Task HandleMessageAsync()
        {
            var message = this.localQueue.Receive();

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
            await this.callMeBackWhenEnvelopeArrives(envelope);
        }

        private static Task SendAsync(Envelope envelope, EndpointAddress recipientEndpointAddress)
        {
            using (var recipientQueue = MsmqUtilities.GetMessageQueue(recipientEndpointAddress, QueueAccessMode.Send))
            {
                recipientQueue.Send(envelope, MsmqUtilities.GetTransactionType());
                return Task.CompletedTask;
            }
        }
    }
}