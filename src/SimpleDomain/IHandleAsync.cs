//-------------------------------------------------------------------------------
// <copyright file="IHandleAsync.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System.Threading.Tasks;

    /// <summary>
    /// Typed async handler interface for messages such as commands or events
    /// </summary>
    /// <typeparam name="TMessage">The type of the message</typeparam>
    public interface IHandleAsync<in TMessage> where TMessage : IMessage
    {
        /// <summary>
        /// Handles the typed message
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        Task HandleAsync(TMessage message);
    }
}