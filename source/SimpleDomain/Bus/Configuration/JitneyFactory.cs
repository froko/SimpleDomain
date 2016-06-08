//-------------------------------------------------------------------------------
// <copyright file="JitneyFactory.cs" company="frokonet.ch">
//   Copyright (c) 2014-2016
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

    using SimpleDomain.Bus;

    /// <summary>
    /// The Jitney factory
    /// </summary>
    public class JitneyFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="JitneyFactory"/>
        /// </summary>
        public JitneyFactory()
        {
            this.Create = config => new SimpleJitney(config);
        }

        /// <summary>
        /// Gets the function to create a Jitney bus using a configuration
        /// </summary>
        public Func<IHaveJitneyConfiguration, Jitney> Create { get; private set; }

        /// <summary>
        /// Registers the function to create a Jitney bus using a configuration
        /// </summary>
        /// <param name="create"></param>
        public void Register(Func<IHaveJitneyConfiguration, Jitney> create)
        {
            this.Create = create;
        }
    }
}