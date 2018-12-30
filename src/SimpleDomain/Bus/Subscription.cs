//-------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="frokonet.ch">
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
    using System.Threading.Tasks;

    /// <summary>
    /// The abstract base class for subscriptions
    /// </summary>
    public abstract class Subscription
    {
        /// <summary>
        /// Returns the fact that a given message cann be handled by this subscription
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns><c>True</c> if the message can be handeled or <c>false</c> if not</returns>
        public abstract bool CanHandle(IMessage message);

        /// <summary>
        /// Returns the fact that a given message can be handeled by this subscription
        /// </summary>
        /// <typeparam name="TMessage">The type of the message</typeparam>
        /// <returns><c>True</c> if the message can be handeled or <c>false</c> if not</returns>
        public abstract bool CanHandle<TMessage>() where TMessage : IMessage;

        /// <summary>
        /// Does something useful with a given message
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        public abstract Task HandleAsync(IMessage message);
    }
}