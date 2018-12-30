//-------------------------------------------------------------------------------
// <copyright file="CommandSubscription.cs" company="frokonet.ch">
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
    using System;
    using System.Threading.Tasks;

    using SimpleDomain.Common;

    /// <summary>
    /// The subscription of a command
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    public class CommandSubscription<TCommand> : Subscription where TCommand : ICommand
    {
        private readonly Func<TCommand, Task> handler;
        private readonly Type commandType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSubscription{TCommand}"/> class.
        /// </summary>
        /// <param name="handler">The action to handle the command asynchronously</param>
        public CommandSubscription(Func<TCommand, Task> handler)
        {
            this.handler = handler;
            this.commandType = typeof(TCommand);
        }

        /// <inheritdoc />
        public override bool CanHandle(IMessage message)
        {
            Guard.NotNull(() => message);
            return message.GetType().IsAssignableFrom(this.commandType);
        }

        /// <inheritdoc />
        public override bool CanHandle<TMessage>()
        {
            return typeof(TMessage).IsAssignableFrom(this.commandType);
        }

        /// <inheritdoc />
        public override Task HandleAsync(IMessage message)
        {
            Guard.NotNull(() => message);
            return this.handler.Invoke((TCommand)message);
        }
    }
}