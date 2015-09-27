//-------------------------------------------------------------------------------
// <copyright file="SimpleJitney.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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
    /// A simple in-memory bus without queueing
    /// </summary>
    public class SimpleJitney : LocalJitney
    {
        /// <summary>
        /// Creates a new instance of <see cref="SimpleJitney"/>
        /// </summary>
        /// <param name="messageSubscriptions">Dependency injection for <see cref="JitneySubscriptions"/></param>
        public SimpleJitney(JitneySubscriptions messageSubscriptions) : base(messageSubscriptions)
        {
        }

        /// <inheritdoc />
        public override async Task SendAsync<TCommand>(TCommand command)
        {
            await this.HandleCommandAsync(command);
        }

        /// <inheritdoc />
        public override async Task PublishAsync<TEvent>(TEvent @event)
        {
            await this.HandleEventAsync(@event);
        }
    }
}