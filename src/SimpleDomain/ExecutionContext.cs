//-------------------------------------------------------------------------------
// <copyright file="ExecutionContext.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System;
    using System.Threading.Tasks;

    using SimpleDomain.Bus;
    using SimpleDomain.Common;
    using SimpleDomain.EventStore;

    /// <summary>
    /// The execution context which is created when starting the <see cref="CompositionRoot"/>
    /// </summary>
    public class ExecutionContext : Disposable
    {
        private readonly Jitney bus;
        private bool isStopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
        /// </summary>
        /// <param name="bus">Dependency injetion for <see cref="Jitney"/></param>
        /// <param name="eventStore">Dependency injection for <see cref="IEventStore"/></param>
        public ExecutionContext(Jitney bus, IEventStore eventStore)
        {
            this.bus = bus;
            this.EventStore = eventStore;
            this.isStopped = false;
        }

        /// <summary>
        /// Gets fired when the execution context is stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Gets fired when the execution context is disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets the <see cref="IDeliverMessages"/> part of the Jitney bus
        /// </summary>
        public IDeliverMessages Bus => this.bus;

        /// <summary>
        /// Gets the event store
        /// </summary>
        public IEventStore EventStore { get; }

        /// <summary>
        /// Stops the execution context
        /// </summary>
        public void Stop()
        {
            this.StopAsync().Wait();
        }

        /// <summary>
        /// Stops the execution context
        /// </summary>
        /// <returns>A task since this is an ansync method</returns>
        public async Task StopAsync()
        {
            if (this.isStopped)
            {
                return;
            }

            await this.bus.StopAsync().ConfigureAwait(false);
            this.isStopped = true;
            this.OnExecutionContextStopped();
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            if (!this.isStopped)
            {
                this.Stop();
            }

            if (!this.IsDisposed)
            {
                this.OnExecutionContextDisposed();
            }
        }

        private void OnExecutionContextStopped()
        {
            this.Stopped?.Invoke(this, EventArgs.Empty);
        }

        private void OnExecutionContextDisposed()
        {
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}