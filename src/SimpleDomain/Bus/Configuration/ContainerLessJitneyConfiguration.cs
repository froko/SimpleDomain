//-------------------------------------------------------------------------------
// <copyright file="ContainerLessJitneyConfiguration.cs" company="frokonet.ch">
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

    /// <summary>
    /// A Jitney configuration class for the use without any IoC container
    /// </summary>
    public class ContainerLessJitneyConfiguration : AbstractJitneyConfiguration
    {
        private readonly JitneyFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerLessJitneyConfiguration"/> class.
        /// </summary>
        /// <param name="factory">Dependency injection for <see cref="JitneyFactory"/></param>
        public ContainerLessJitneyConfiguration(JitneyFactory factory)
            : base(new ContainerLessHandlerRegistry())
        {
            this.factory = factory;
        }

        /// <inheritdoc />
        public override void Register(Func<IHaveJitneyConfiguration, Jitney> createJitney)
        {
            this.factory.Register(createJitney);
        }
    }
}