//-------------------------------------------------------------------------------
// <copyright file="MsmqJitneyConfigurationExtensions.cs" company="frokonet.ch">
//   Copyright (c) 2014-2018
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
    using SimpleDomain.Bus.Msmq;

    /// <summary>
    /// Configuation extensions for the Jitney configuration base class
    /// </summary>
    public static class MsmqJitneyConfigurationExtensions
    {
        /// <summary>
        /// Registers the <see cref="MessageQueueJitney"/> bus with the MSMQ provider
        /// </summary>
        /// <param name="configuration">The Jintey bus configuration</param>
        public static void UseMsmqJitney(this IConfigureThisJitney configuration)
        {
            configuration.AddConfigurationItem(MessageQueueJitney.MessageQueueProvider, new MsmqProvider());
            configuration.Register(config => new MessageQueueJitney(config));
        }
    }
}