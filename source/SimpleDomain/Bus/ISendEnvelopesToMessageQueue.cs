//-------------------------------------------------------------------------------
// <copyright file="ISendEnvelopesToMessageQueue.cs" company="frokonet.ch">
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
    /// <summary>
    /// Defines an interface to send instances of <see cref="Envelope"/> directly to a message queue.
    /// This interface is used for audit and error queue behavior.
    /// </summary>
    public interface ISendEnvelopesToMessageQueue
    {
        /// <summary>
        /// Sends an <see cref="Envelope"/> directly to a message queue addressed by its <see cref="EndpointAddress"/>
        /// <remarks>There is no transaction involved for this type of delivery</remarks>
        /// </summary>
        /// <param name="envelope">The envelope to send</param>
        /// <param name="endpointAddress">The endpoint address to send to</param>
        void Send(Envelope envelope, EndpointAddress endpointAddress);
    }
}