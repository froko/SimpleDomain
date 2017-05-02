//-------------------------------------------------------------------------------
// <copyright file="ErrorQueueStep.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Incomming
{
    using System;
    using System.Threading.Tasks;

    using SimpleDomain.Bus.MSMQ;

    /// <summary>
    /// The incomming message step which records all failed messages to an error queue
    /// </summary>
    public class ErrorQueueStep : IncommingMessageStep
    {
        private readonly EndpointAddress errorQueue;
        private readonly ISendEnvelopesToMessageQueue messageQueueSender;

        /// <summary>
        /// Creates a new instance of <see cref="ErrorQueueStep"/> which uses MSMQ
        /// </summary>
        /// <param name="errorQueue">The address of the error queue</param>
        public ErrorQueueStep(string errorQueue)
            : this(errorQueue, new MsmqSender())
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ErrorQueueStep"/>
        /// </summary>
        /// <param name="errorQueue">The address of the error queue</param>
        /// <param name="messageQueueSender">Dependency injection for <see cref="ISendEnvelopesToMessageQueue"/></param>
        public ErrorQueueStep(string errorQueue, ISendEnvelopesToMessageQueue messageQueueSender)
        {
            this.errorQueue = new EndpointAddress(errorQueue);
            this.messageQueueSender = messageQueueSender;
            this.Name = $"Error Queue Step ({this.errorQueue})";
        }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override async Task InvokeAsync(IncommingMessageContext context, Func<Task> next)
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.SendToErrorQueue(context, exception);
                throw;
            }
        }

        private void SendToErrorQueue(IncommingMessageContext context, Exception exception)
        {
            var envelope = context.Envelope
                .ReplaceHeader(HeaderKeys.TimeProcessed, DateTime.UtcNow)    
                .ReplaceHeader(HeaderKeys.ExceptionName, exception.GetType().Name)
                .ReplaceHeader(HeaderKeys.ExceptionMessage, exception.Message)
                .ReplaceHeader(HeaderKeys.ExceptionString, exception.ToString())
                .AddHeader(HeaderKeys.RetryCount, 0);

            this.messageQueueSender.Send(envelope, this.errorQueue);
        }
    }
}