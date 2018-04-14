//-------------------------------------------------------------------------------
// <copyright file="JitneyConfiguration.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;

    using Ninject;

    using SimpleDomain.Bus.Configuration;

    /// <summary>
    /// The Jitney configuration
    /// </summary>
    public class JitneyConfiguration : AbstractIoCContainerJitneyConfiguration
    {
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="JitneyConfiguration"/> class.
        /// </summary>
        /// <param name="kernel">Dependency injection for <see cref="IKernel"/></param>
        public JitneyConfiguration(IKernel kernel) : base(new HandlerRegistry(kernel))
        {
            this.kernel = kernel;
        }

        /// <inheritdoc />
        public override void Register(Func<IHaveJitneyConfiguration, Jitney> createJitney)
        {
            this.kernel.Bind<IDeliverMessages, Jitney>().ToConstant(createJitney(this));
        }

        /// <inheritdoc />
        protected override void RegisterHandlerType(Type type)
        {
            this.kernel.Bind(type).ToSelf().InTransientScope();
        }
    }
}