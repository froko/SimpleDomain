//-------------------------------------------------------------------------------
// <copyright file="LocalJitney.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    /// <summary>
    /// The abstract local bus
    /// </summary>
    public abstract class LocalJitney : Jitney
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocalJitney"/>
        /// </summary>
        /// <param name="messageSubscriptions">Dependency injection for <see cref="JitneySubscriptions"/></param>
        protected LocalJitney(JitneySubscriptions messageSubscriptions) : base(messageSubscriptions)
        {
        }

        /// <inheritdoc />
        public override void Load(params JitneyComposer[] jitneyComposers)
        {
            foreach (var jitneyComposer in jitneyComposers)
            {
                jitneyComposer.Initialize(this);
                jitneyComposer.Subscribe(this.MessageSubscriptions);
            }
        }
    }
}