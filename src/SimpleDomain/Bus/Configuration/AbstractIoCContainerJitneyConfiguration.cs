//-------------------------------------------------------------------------------
// <copyright file="AbstractIoCContainerJitneyConfiguration.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The Jitney configuration base class for IoC containers
    /// </summary>
    public abstract class AbstractIoCContainerJitneyConfiguration : AbstractJitneyConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractIoCContainerJitneyConfiguration"/> class.
        /// </summary>
        /// <param name="handlerRegistry">Dependency injection for <see cref="AbstractHandlerRegistry"/></param>
        protected AbstractIoCContainerJitneyConfiguration(AbstractHandlerRegistry handlerRegistry)
            : base(handlerRegistry)
        {
        }

        /// <summary>
        /// Subscribes all message handlers in a given list of assemblies
        /// </summary>
        /// <param name="assemblies">The list of assemblies to scan and register for</param>
        public void SubscribeMessageHandlers(IEnumerable<Assembly> assemblies)
        {
            this.SubscribeMessageHandlers(assemblies, this.RegisterHandlerType);
        }

        /// <summary>
        /// Subscribes all message handlers in the calling assembly
        /// </summary>
        public void SubscribeMessageHandlersInThisAssembly()
        {
            this.SubscribeMessageHandlersInThisAssembly(this.RegisterHandlerType);
        }

        /// <summary>
        /// Registers a handler type in the IoC container
        /// </summary>
        /// <param name="type">The handler type</param>
        protected abstract void RegisterHandlerType(Type type);
    }
}