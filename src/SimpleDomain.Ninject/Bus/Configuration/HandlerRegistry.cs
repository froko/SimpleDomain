﻿//-------------------------------------------------------------------------------
// <copyright file="HandlerRegistry.cs" company="frokonet.ch">
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

    using global::Ninject;

    /// <summary>
    /// The Ninject handler registry
    /// </summary>
    public class HandlerRegistry : AbstractHandlerRegistry
    {
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerRegistry"/> class.
        /// </summary>
        /// <param name="kernel">Dependency injection for <see cref="IKernel"/></param>
        public HandlerRegistry(IKernel kernel)
        {
            this.kernel = kernel;
        }

        /// <inheritdoc />
        protected override object Resolve(Type handlerType)
        {
            return this.kernel.Get(handlerType);
        }
    }
}