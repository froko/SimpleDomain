//-------------------------------------------------------------------------------
// <copyright file="GlobalJitneyConfigurationExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    /// <summary>
    /// Configuation extensions for the Jitney configuration base class
    /// </summary>
    public static class GlobalJitneyConfigurationExtensions
    {
        /// <summary>
        /// Registers the <see cref="SimpleJitney"/> bus
        /// </summary>
        /// <param name="configuration">The Jintey bus configuration</param>
        public static void UseSimpleJitney(this IConfigureThisJitney configuration)
        {
            configuration.Register(config => new SimpleJitney(config));
        }

        /// <summary>
        /// Registers the <see cref="MessageQueueJitney"/> bus with the In-Memory Queue provider
        /// </summary>
        /// <param name="configuration">The Jintey bus configuration</param>
        public static void UseInMemoryQueueJitney(this IConfigureThisJitney configuration)
        {
            configuration.AddConfigurationItem(MessageQueueJitney.MessageQueueProvider, new InMemoryQueueProvider());
            configuration.Register(config => new MessageQueueJitney(config));
        }
    }
}