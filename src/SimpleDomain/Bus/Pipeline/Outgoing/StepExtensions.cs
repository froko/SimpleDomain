//-------------------------------------------------------------------------------
// <copyright file="StepExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Some pipeline step extensions
    /// </summary>
    public static class StepExtensions
    {
        /// <summary>
        /// Appends the <see cref="FinalOutgoingMessageStep"/> to the list of all registered outgoing message pipeline steps
        /// </summary>
        /// <param name="steps">The list of all registered outgoing message pipeline steps</param>
        /// <returns>The updated list of all registered outgoing message pipeline steps</returns>
        public static IEnumerable<OutgoingMessageStep> WithFinalOutgoingMessageStep(
            this IEnumerable<OutgoingMessageStep> steps)
        {
            return steps.Concat(new[] { new FinalOutgoingMessageStep() });
        }

        /// <summary>
        /// Appends the <see cref="FinalOutgoingEnvelopeStep"/> to the list of all registered outgoing envelope pipeline steps
        /// </summary>
        /// <param name="steps">The list of all registered outgoing envelope pipeline steps</param>
        /// <param name="handleEnvelopeAsync">The last async action to be performed for an outgoing envelope</param>
        /// <returns>The updated list of all registered outgoing envelope pipeline steps</returns>
        public static IEnumerable<OutgoingEnvelopeStep> WithFinalOutgoingEnvelopeStep(
            this IEnumerable<OutgoingEnvelopeStep> steps,
            Func<Envelope, Task> handleEnvelopeAsync)
        {
            return steps.Concat(new[] { new FinalOutgoingEnvelopeStep(handleEnvelopeAsync) });
        }
    }
}