//-------------------------------------------------------------------------------
// <copyright file="AsyncMessageDelegate.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The async message delegate
    /// </summary>
    public class AsyncMessageDelegate
    {
        private readonly Type messageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncMessageDelegate"/> class.
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="asyncDelegate">The async handler delegate</param>
        public AsyncMessageDelegate(Type messageType, Func<object, object, Task> asyncDelegate)
        {
            this.messageType = messageType;
            this.InvokeAsync = asyncDelegate;
        }

        /// <summary>
        /// Gets the async handler delegate
        /// </summary>
        public Func<object, object, Task> InvokeAsync { get; private set; }

        /// <summary>
        /// Returns the fact that this instance can handle a given message type
        /// </summary>
        /// <param name="type">The message type</param>
        /// <returns><c>True</c> if this instance can handle a message type or <c>false</c> if not</returns>
        public bool CanHandle(Type type)
        {
            return this.messageType == type;
        }

        /// <summary>
        /// Returns the fact that this instance can handle a given message instance
        /// </summary>
        /// <param name="message">The message instance</param>
        /// <returns><c>True</c> if this instance can handle the message instance or <c>false</c> if not</returns>
        public bool CanHandle(object message)
        {
            return this.messageType.IsInstanceOfType(message);
        }
    }
}