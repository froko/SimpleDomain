//-------------------------------------------------------------------------------
// <copyright file="EventStoreConfigurationTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Configuration
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using Ninject;

    using SimpleDomain.Bus;
    using SimpleDomain.EventStore.Persistence;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class EventStoreConfigurationTest : IDisposable
    {
        private readonly IKernel kernel;
        private readonly EventStoreConfiguration testee;

        public EventStoreConfigurationTest()
        {
            this.kernel = new StandardKernel();
            this.testee = new EventStoreConfiguration(this.kernel);
        }

        [Fact]
        public void CanRegisterEventStore()
        {
            this.testee.Register<InMemoryEventStore>();

            this.kernel.Get<IEventStore>().Should().BeAssignableTo<InMemoryEventStore>();
        }

        [Fact]
        public async Task CanDefineAsyncEventDispatching()
        {
            var jitney = A.Fake<Jitney>();
            this.kernel.Bind<Jitney>().ToConstant(jitney).InSingletonScope();

            this.testee.DefineAsyncEventDispatching<Jitney>((bus, @event) => bus.PublishAsync(@event));

            await this.testee.DispatchEvents(new ValueEvent(11));

            A.CallTo(() => jitney.PublishAsync(A<IEvent>.Ignored)).MustHaveHappened();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }
}