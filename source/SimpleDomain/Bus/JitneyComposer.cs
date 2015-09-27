//-------------------------------------------------------------------------------
// <copyright file="JitneyComposer.cs" company="frokonet.ch">
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
    /// Base class used to compose the <see cref="Jitney"/> bus
    /// </summary>
    public abstract class JitneyComposer
    {
        /// <summary>
        /// Gets the bus to send commands or publish events
        /// </summary>
        protected IDeliverMessages Bus { get; private set; }

        /// <summary>
        /// Sets the bus to send commands or publish events
        /// </summary>
        /// <param name="bus">The bus</param>
        public virtual void Initialize(IDeliverMessages bus)
        {
            this.Bus = bus;
        }
        
        /// <summary>
        /// Subscribes command and/or event handlers to the bus
        /// </summary>
        /// <param name="bus">The bus</param>
        public virtual void Subscribe(ISubscribeHandlers bus)
        {
        }
    }
}