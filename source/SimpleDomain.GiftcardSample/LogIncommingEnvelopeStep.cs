//-------------------------------------------------------------------------------
// <copyright file="LogIncommingEnvelopeStep.cs" company="frokonet.ch">
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

namespace GiftcardSample
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using SimpleDomain.Bus;
    using SimpleDomain.Bus.Pipeline.Incomming;

    public class LogIncommingEnvelopeStep : IncommingEnvelopeStep
    {
        public LogIncommingEnvelopeStep()
        {
            this.Name = "Log Incomming Envelope Step";
        }

        public override string Name { get; }

        public override Task InvokeAsync(IncommingEnvelopeContext context, Func<Task> next)
        {
            Debug.WriteLine("Received message from {0}", context.Envelope.Headers[HeaderKeys.Sender]);
            return next();
        }
    }
}