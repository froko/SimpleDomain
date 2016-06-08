//-------------------------------------------------------------------------------
// <copyright file="OutgoingPipeline.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The outgoing pipeline
    /// </summary>
    public class OutgoingPipeline
    {
        private readonly IHavePipelineConfiguration configuration;
        private readonly Queue<OutgoingMessageStep> outgoingMessageSteps;
        private readonly Queue<OutgoingEnvelopeStep> outgoingEnvelopeSteps;

        /// <summary>
        /// Creates a new instance of <see cref="OutgoingPipeline"/>
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHavePipelineConfiguration"/></param>
        /// <param name="outgoingMessageSteps">All registered pipeline steps for outgoing messages</param>
        /// <param name="outgoingEnvelopeSteps">All registered pipeline steps for outgoing envelopes</param>
        public OutgoingPipeline(
            IHavePipelineConfiguration configuration,
            IEnumerable<OutgoingMessageStep> outgoingMessageSteps,
            IEnumerable<OutgoingEnvelopeStep> outgoingEnvelopeSteps)
        {
            this.configuration = configuration;
            this.outgoingMessageSteps = new Queue<OutgoingMessageStep>(outgoingMessageSteps);
            this.outgoingEnvelopeSteps = new Queue<OutgoingEnvelopeStep>(outgoingEnvelopeSteps);
        }

        /// <summary>
        /// Invokes the pipeline
        /// </summary>
        /// <param name="message">The message</param>
        public virtual async Task InvokeAsync(IMessage message)
        {
            if (this.configuration.LocalEndpointAddress == null)
            {
                throw new JitneyConfigurationException(ExceptionMessages.LocalEndpointAddressNotDefined);
            }

            var messageContext = new OutgoingMessageContext(message, this.configuration);
            await this.InvokeOutgoingMessageStepsAsync(messageContext).ConfigureAwait(false);

            if (!messageContext.Envelopes.Any())
            {
                return;
            }
            
            var outgoingEnvelopeTasks = messageContext.Envelopes
                .Select(envelope => new OutgoingEnvelopeContext(envelope, this.configuration))
                .Select(this.InvokeOutgoingEnvelopeStepsAsync);
            
            await Task.WhenAll(outgoingEnvelopeTasks).ConfigureAwait(false);
        }

        private Task InvokeOutgoingMessageStepsAsync(OutgoingMessageContext context)
        {
            if (!this.outgoingMessageSteps.Any())
            {
                return Task.CompletedTask;
            }

            var nextStep = this.outgoingMessageSteps.Dequeue();
            return nextStep.InvokeAsync(context, () => this.InvokeOutgoingMessageStepsAsync(context));
        }

        private Task InvokeOutgoingEnvelopeStepsAsync(OutgoingEnvelopeContext context)
        {
            if (!this.outgoingEnvelopeSteps.Any())
            {
                return Task.CompletedTask;
            }

            var nextStep = this.outgoingEnvelopeSteps.Dequeue();
            return nextStep.InvokeAsync(context, () => this.InvokeOutgoingEnvelopeStepsAsync(context));
        }
    }
}