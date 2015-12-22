//-------------------------------------------------------------------------------
// <copyright file="FinalIncommingEnvelopeStep.cs" company="frokonet.ch">
//   Copyright (c) 2015
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
    using System.Threading.Tasks;

    /// <summary>
    /// The final incomming envelope pipleine step
    /// </summary>
    public class FinalIncommingEnvelopeStep : IncommingEnvelopeStep
    {
        /// <summary>
        /// Creates a new instance of <see cref="FinalIncommingEnvelopeStep"/>
        /// </summary>
        public FinalIncommingEnvelopeStep()
        {
            this.Name = "Final Incomming Envelope Step";
        }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override Task InvokeAsync(IncommingEnvelopeContext context, Func<Task> next)
        {
            context.SetMessage();
            return Task.CompletedTask;
        }
    }
}