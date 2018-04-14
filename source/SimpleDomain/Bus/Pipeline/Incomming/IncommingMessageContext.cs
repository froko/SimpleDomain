//-------------------------------------------------------------------------------
// <copyright file="IncommingMessageContext.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Incomming
{
    /// <summary>
    /// The incomming message pipeline context
    /// </summary>
    public class IncommingMessageContext : PipelineContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncommingMessageContext"/> class.
        /// </summary>
        /// <param name="envelope">The originating envelope</param>
        /// <param name="configuration">Dependency injection for <see cref="IHavePipelineConfiguration"/></param>
        public IncommingMessageContext(
            Envelope envelope,
            IHavePipelineConfiguration configuration) : base(configuration)
        {
            this.Envelope = envelope;
            this.Message = envelope.Body;
        }

        /// <summary>
        /// Gets the originating envelope
        /// </summary>
        public Envelope Envelope { get; }

        /// <summary>
        /// Gets the incomming message
        /// </summary>
        public IMessage Message { get; }

        /// <summary>
        /// Gets the intent of the incomming message
        /// </summary>
        public MessageIntent MessageIntent => this.Message.GetIntent();
    }
}