//-------------------------------------------------------------------------------
// <copyright file="FinalIncommingMessageStep.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Incomming
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Common.Logging;

    /// <summary>
    /// The final incomming message pipeline step
    /// </summary>
    public class FinalIncommingMessageStep : IncommingMessageStep
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Jitney));

        private readonly IDictionary<MessageIntent, Func<IMessage, Task>> handlers; 
        
        /// <summary>
        /// Creates a new instance of <see cref="FinalIncommingMessageStep"/>
        /// </summary>
        /// <param name="handleCommandAsync">The async command handler action</param>
        /// <param name="handleEventAsync">The async event handler action</param>
        /// <param name="handleSubscriptionMessageAsync">The async subscription message handler action</param>
        public FinalIncommingMessageStep(
            Func<ICommand, Task> handleCommandAsync,
            Func<IEvent, Task> handleEventAsync,
            Func<SubscriptionMessage, Task> handleSubscriptionMessageAsync)
        {
            this.handlers = new Dictionary<MessageIntent, Func<IMessage, Task>>();

            this.handlers.Add(MessageIntent.Command, message => handleCommandAsync(message as ICommand));
            this.handlers.Add(MessageIntent.Event, message => handleEventAsync(message as IEvent));
            this.handlers.Add(MessageIntent.SubscriptionMessage, message => handleSubscriptionMessageAsync(message as SubscriptionMessage));

            this.Name = "Final Incomming Message Step";
        }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override Task InvokeAsync(IncommingMessageContext context, Func<Task> next)
        {
            Logger.InfoFormat(
                "Received {0} of type {1} from {2}",
                context.Message.GetIntent(),
                context.Message.GetFullName(),
                context.Envelope.Headers[HeaderKeys.Sender]);

            return this.handlers[context.MessageIntent](context.Message);
        }
    }
}