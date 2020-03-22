//-------------------------------------------------------------------------------
// <copyright file="FinalOutgoingEnvelopeStep.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
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
    using System;
    using System.Threading.Tasks;

    using SimpleDomain.Common.Logging;

    /// <summary>
    /// The final outgoing envelope pipeline step
    /// </summary>
    public class FinalOutgoingEnvelopeStep : OutgoingEnvelopeStep
    {
        private static readonly ILogger Logger = LoggerFactory.Create<FinalOutgoingEnvelopeStep>();

        private readonly Func<Envelope, Task> handleEnvelopeAsync;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalOutgoingEnvelopeStep"/> class.
        /// </summary>
        /// <param name="handleEnvelopeAsync">The last async action to be performed for an outgoing envelope</param>
        public FinalOutgoingEnvelopeStep(Func<Envelope, Task> handleEnvelopeAsync)
        {
            this.handleEnvelopeAsync = handleEnvelopeAsync;
            this.Name = "Final Outgoing Envelope Step";
        }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override Task InvokeAsync(OutgoingEnvelopeContext context, Func<Task> next)
        {
            Logger.InfoFormat(
                "Sending {0} of type {1} to {2}",
                context.Envelope.Body.GetIntent(),
                context.Envelope.Body.GetFullName(),
                context.Envelope.Headers[HeaderKeys.Recipient]);

            return this.handleEnvelopeAsync(context.Envelope);
        }
    }
}