//-------------------------------------------------------------------------------
// <copyright file="EnvelopeBuilder.cs" company="frokonet.ch">
//   Copyright (c) 2014-2017
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
    using System;

    using SimpleDomain.TestDoubles;

    public static class EnvelopeBuilder
    {
        public static Envelope Build()
        {
            return Build(Guid.NewGuid(), new MyCommand());
        }

        public static Envelope Build(IMessage message)
        {
            return Build(Guid.NewGuid(), message);
        }

        public static Envelope Build(Guid correlationId)
        {
            return Build(correlationId, new MyCommand());
        }

        private static Envelope Build(Guid correlationId, IMessage message)
        {
            return Envelope.Create(
                new EndpointAddress("sender@localhost"),
                new EndpointAddress("recipient@localhost"),
                correlationId,
                message);
        }
    }
}