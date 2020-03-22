//-------------------------------------------------------------------------------
// <copyright file="StepExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline.Incomming
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
        /// Appends the <see cref="FinalIncommingEnvelopeStep"/> to the list of all registered incomming envelope pipeline steps
        /// </summary>
        /// <param name="steps">The list of all registered incomming envelope pipeline steps</param>
        /// <returns>The updated list of all registered incomming envelope pipeline steps</returns>
        public static IEnumerable<IncommingEnvelopeStep> WithFinalIncommingEnvelopeStep(
            this IEnumerable<IncommingEnvelopeStep> steps)
        {
            return steps.Concat(new[] { new FinalIncommingEnvelopeStep() });
        }

        /// <summary>
        /// Appends the <see cref="FinalIncommingMessageStep"/> to the list of all registered incomming message pipeline steps
        /// </summary>
        /// <param name="steps">The list of all registered incomming message pipeline steps</param>
        /// <param name="handleCommandAsync">The async command handler action</param>
        /// <param name="handleEventAsync">The async event handler action</param>
        /// <param name="handleSubscriptionMessageAsync">The async subscription message handler action</param>
        /// <returns>The updated list of all registered incomming message pipeline steps</returns>
        public static IEnumerable<IncommingMessageStep> WithFinalIncommingMessageStep(
            this IEnumerable<IncommingMessageStep> steps,
            Func<ICommand, Task> handleCommandAsync,
            Func<IEvent, Task> handleEventAsync,
            Func<SubscriptionMessage, Task> handleSubscriptionMessageAsync)
        {
            var finalStep = new FinalIncommingMessageStep(
                handleCommandAsync,
                handleEventAsync,
                handleSubscriptionMessageAsync);

            return steps.Concat(new[] { finalStep });
        }
    }
}