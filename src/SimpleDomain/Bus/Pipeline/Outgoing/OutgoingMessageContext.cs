//-------------------------------------------------------------------------------
// <copyright file="OutgoingMessageContext.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using System.Collections.Generic;

    /// <summary>
    /// The outgoing message pipeline context
    /// </summary>
    public class OutgoingMessageContext : PipelineContext
    {
        private readonly IList<Envelope> envelopes;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingMessageContext"/> class.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        /// <param name="configuration">Dependency injection for <see cref="IHavePipelineConfiguration"/></param>
        public OutgoingMessageContext(IMessage message, IHavePipelineConfiguration configuration)
            : base(configuration)
        {
            this.envelopes = new List<Envelope>();
            this.Message = message;
        }

        /// <summary>
        /// Gets the outgoing message
        /// </summary>
        public virtual IMessage Message { get; }

        /// <summary>
        /// Gets a list of envelopes for the registered receiving endpoints
        /// </summary>
        public IEnumerable<Envelope> Envelopes => this.envelopes;

        /// <summary>
        /// Creates a new envelope and adds it to the list of envelopes for the registered receiving endpoints
        /// </summary>
        /// <param name="recipient">The receiving endpoint address</param>
        public virtual void CreateEnvelope(EndpointAddress recipient)
        {
            var sender = this.Configuration.LocalEndpointAddress;

            if (this.Configuration.HasCorrelationId)
            {
                var correlationId = this.Configuration.PeekCorrelationId();
                this.envelopes.Add(Envelope.Create(sender, recipient, correlationId, this.Message));
            }
            else
            {
                this.envelopes.Add(Envelope.Create(sender, recipient, this.Message));
            }
        }
    }
}