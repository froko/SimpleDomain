//-------------------------------------------------------------------------------
// <copyright file="AbstractCompositionRoot.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using SimpleDomain.Bus;
    using SimpleDomain.EventStore;

    /// <summary>
    /// An abstract composition root which helps wiring up the Jitney bus, 
    /// the EventStore and the Repository without any IoC container
    /// </summary>
    public abstract class AbstractCompositionRoot
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractCompositionRoot"/>
        /// </summary>
        protected AbstractCompositionRoot()
        {
            this.Bus = this.CreateBus(new ContainerLessJitneyConfiguration());
            this.EventStore = this.CreateEventStore(new ContainerLessEventStoreConfiguration());
        }

        /// <summary>
        /// Gets the Jitney bus
        /// </summary>
        public Jitney Bus { get; private set; }

        /// <summary>
        /// Gets the EventStore
        /// </summary>
        public IEventStore EventStore { get; private set; }

        /// <summary>
        /// Gets the Repository
        /// </summary>
        public IEventSourcedRepository Repository
        {
            get { return new EventStoreRepository(this.EventStore); }
        }

        /// <summary>
        /// Registers a <see cref="BoundedContext"/> within this composition root
        /// </summary>
        /// <param name="boundedContext">The bounded context</param>
        public void Register(BoundedContext boundedContext)
        {
            boundedContext.Configure(this.Bus, this.Repository);
        }

        /// <summary>
        /// Creates an instance of a Jitney bus
        /// </summary>
        /// <param name="configuration">The Jitney configuration</param>
        /// <returns>An instance of a Jitney bus</returns>
        protected abstract Jitney CreateBus(ContainerLessJitneyConfiguration configuration);

        /// <summary>
        /// Creates an instance of an EventStore
        /// </summary>
        /// <param name="configuration">The EventStore configuration</param>
        /// <returns>An instance of the EventStore</returns>
        protected abstract IEventStore CreateEventStore(ContainerLessEventStoreConfiguration configuration);
    }
}