//-------------------------------------------------------------------------------
// <copyright file="IncommingPipeline.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Incomming
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The incomming pipeline
    /// </summary>
    public class IncommingPipeline
    {
        private readonly IHavePipelineConfiguration configuration;
        private readonly Queue<IncommingEnvelopeStep> incommingEnvelopeSteps;
        private readonly Queue<IncommingMessageStep> incommingMessageSteps;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncommingPipeline"/> class.
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHavePipelineConfiguration"/></param>
        /// <param name="incommingEnvelopeSteps">All registered pipeline steps for incomming envelopes</param>
        /// <param name="incommingMessageSteps">All registered pipeline steps for incomming messages</param>
        public IncommingPipeline(
            IHavePipelineConfiguration configuration,
            IEnumerable<IncommingEnvelopeStep> incommingEnvelopeSteps,
            IEnumerable<IncommingMessageStep> incommingMessageSteps)
        {
            this.configuration = configuration;
            this.incommingEnvelopeSteps = new Queue<IncommingEnvelopeStep>(incommingEnvelopeSteps);
            this.incommingMessageSteps = new Queue<IncommingMessageStep>(incommingMessageSteps);
        }

        /// <summary>
        /// Invokes the pipeline
        /// </summary>
        /// <param name="envelope">The envelope</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        public async Task InvokeAsync(Envelope envelope)
        {
            var envelopeContext = new IncommingEnvelopeContext(envelope, this.configuration);
            await this.InvokeIncommingEnvelopeStepsAsync(envelopeContext).ConfigureAwait(false);

            if (envelopeContext.Message == null)
            {
                return;
            }

            var messageContext = new IncommingMessageContext(
                envelopeContext.Envelope,
                this.configuration);
            await this.InvokeIncommingMessageStepsAsync(messageContext).ConfigureAwait(false);
        }

        private Task InvokeIncommingEnvelopeStepsAsync(IncommingEnvelopeContext context)
        {
            if (!this.incommingEnvelopeSteps.Any())
            {
                return Task.CompletedTask;
            }

            var nextStep = this.incommingEnvelopeSteps.Dequeue();
            return nextStep.InvokeAsync(context, () => this.InvokeIncommingEnvelopeStepsAsync(context));
        }

        private Task InvokeIncommingMessageStepsAsync(IncommingMessageContext context)
        {
            if (!this.incommingMessageSteps.Any())
            {
                return Task.CompletedTask;
            }

            var nextStep = this.incommingMessageSteps.Dequeue();
            return nextStep.InvokeAsync(context, () => this.InvokeIncommingMessageStepsAsync(context));
        }
    }
}