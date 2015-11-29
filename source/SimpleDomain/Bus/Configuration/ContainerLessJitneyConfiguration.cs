//-------------------------------------------------------------------------------
// <copyright file="ContainerLessJitneyConfiguration.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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
        /// <summary>
        /// Creates a new instance of <see cref="ContainerLessJitneyConfiguration"/>
        /// </summary>
        public ContainerLessJitneyConfiguration() : base(new ContainerLessHandlerRegistry())
        {
        }

        /// <inheritdoc />
        public override void Register<TJitney>()
        {
            throw new NotSupportedException("You cannot register a Bus when there is no IoC container");
        }

        /// <inheritdoc />
        protected override void RegisterHandlerType(Type type)
        {
            throw new NotSupportedException("You cannot register a handler when there is no IoC container");
        }
    }
}