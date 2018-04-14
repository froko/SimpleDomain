//-------------------------------------------------------------------------------
// <copyright file="CompositionRoot.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SimpleDomain.Bus;
    using SimpleDomain.Bus.Configuration;
    using SimpleDomain.EventStore;
    using SimpleDomain.EventStore.Configuration;

    /// <summary>
    /// The composition root used to configure the bus and event store in a no-IoC container environment
    /// </summary>
    public class CompositionRoot
    {
        private readonly JitneyFactory jitneyFactory;
        private readonly EventStoreFactory eventStoreFactory;
        private readonly List<IBoundedContext> boundedContexts;

        private AbstractJitneyConfiguration jitneyConfiguration;
        private AbstractEventStoreConfiguration eventStoreConfiguration;
        private ExecutionContext executionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionRoot"/> class.
        /// </summary>
        public CompositionRoot()
        {
            this.jitneyFactory = new JitneyFactory();
            this.eventStoreFactory = new EventStoreFactory();
            this.boundedContexts = new List<IBoundedContext>();

            this.jitneyConfiguration = new ContainerLessJitneyConfiguration(this.jitneyFactory);
            this.eventStoreConfiguration = new ContainerLessEventStoreConfiguration(this.eventStoreFactory);
            this.executionContext = null;
        }

        /// <summary>
        /// Configures the Jitney bus
        /// </summary>
        /// <returns>An instance of <see cref="IConfigureThisJitney"/> since this is a builder method</returns>
        public IConfigureThisJitney ConfigureJitney()
        {
            this.ThrowExceptionIfAlreadyStarted();
            return this.jitneyConfiguration;
        }

        /// <summary>
        /// Runs an action to configure the Jitney bus
        /// </summary>
        /// <param name="configureJitney">An action which configures the Jitney bus</param>
        public void Run(Action<IConfigureThisJitney> configureJitney)
        {
            this.ThrowExceptionIfAlreadyStarted();
            configureJitney(this.jitneyConfiguration);
        }

        /// <summary>
        /// Configures the event store
        /// </summary>
        /// <returns>An instance of <see cref="IConfigureThisEventStore"/> since this is a builder method</returns>
        public IConfigureThisEventStore ConfigureEventStore()
        {
            this.ThrowExceptionIfAlreadyStarted();
            return this.eventStoreConfiguration;
        }

        /// <summary>
        /// Runs an action to configure the event store
        /// </summary>
        /// <param name="configureEventStore">An action to configure the event store</param>
        public void Run(Action<IConfigureThisEventStore> configureEventStore)
        {
            this.ThrowExceptionIfAlreadyStarted();
            configureEventStore(this.eventStoreConfiguration);
        }

        /// <summary>
        /// Registers a bounded context
        /// </summary>
        /// <param name="boundedContext">An instance of a class implementing <see cref="IBoundedContext"/></param>
        public void Register(IBoundedContext boundedContext)
        {
            this.ThrowExceptionIfAlreadyStarted();
            this.boundedContexts.Add(boundedContext);
        }

        /// <summary>
        /// Starts the composition root
        /// </summary>
        /// <returns>A disposable execution context containing a reference to the bus and the event store</returns>
        public ExecutionContext Start()
        {
            return this.StartAsync().Result;
        }

        /// <summary>
        /// Starts the composition root
        /// </summary>
        /// <returns>A disposable execution context containing a reference to the bus and the event store</returns>
        public async Task<ExecutionContext> StartAsync()
        {
            this.ThrowExceptionIfAlreadyStarted();

            var bus = this.jitneyFactory.Create(this.jitneyConfiguration);
            var eventStore = this.eventStoreFactory.Create(this.eventStoreConfiguration, bus);

            this.boundedContexts.ForEach(bc => bc.Configure(this.jitneyConfiguration, bus, new EventStoreRepository(eventStore)));

            await bus.StartAsync().ConfigureAwait(false);

            this.executionContext = new ExecutionContext(bus, eventStore);
            this.executionContext.Stopped += (sender, args) => { this.executionContext = null; };
            this.executionContext.Disposed += (sender, args) =>
            {
                this.jitneyConfiguration = new ContainerLessJitneyConfiguration(this.jitneyFactory);
                this.eventStoreConfiguration = new ContainerLessEventStoreConfiguration(this.eventStoreFactory);
                this.executionContext = null;
            };

            return this.executionContext;
        }

        private void ThrowExceptionIfAlreadyStarted()
        {
            if (this.executionContext != null)
            {
                throw new CompositionRootAlreadyStartedException();
            }
        }
    }
}