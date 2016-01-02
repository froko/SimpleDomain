//-------------------------------------------------------------------------------
// <copyright file="AbstractEventStoreConfigurationTest.cs" company="frokonet.ch">
//   Copyright (c) 2016
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.EventStore.Persistence;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class AbstractEventStoreConfigurationTest
    {
        private readonly IContainer container;
        private readonly EventStoreConfiguration testee;

        public AbstractEventStoreConfigurationTest()
        {
            this.container = A.Fake<IContainer>();
            this.testee = new EventStoreConfiguration(this.container);
        }

        [Fact]
        public void CanAddAndGetConfigurationItem()
        {
            this.testee.AddConfigurationItem("Foo", new ConfigurationItem());

            var configurationItem = this.testee.Get<ConfigurationItem>("Foo");

            configurationItem.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsException_WhenTryingToAddConfigurationItemWithNullAsKey()
        {
            Action action = () => this.testee.AddConfigurationItem(null, new ConfigurationItem());

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToGetConfigurationItemWithNullAsKey()
        {
            Action action = () => this.testee.Get<string>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenConfigurationItemTypeIsNotOfGivenType()
        {
            this.testee.AddConfigurationItem("Foo", new ConfigurationItem());

            Action action = () => this.testee.Get<string>("Foo");

            action.ShouldThrow<InvalidCastException>();
        }

        [Fact]
        public void ThrowsException_WhenConfigurationItemKeyIsNotFound()
        {
            Action action = () => this.testee.Get<string>("NotExistingKey");

            action.ShouldThrow<KeyNotFoundException>();
        }

        [Fact]
        public void CanDefineAsyncEventDispatching()
        {
            var eventDispatcher = new Func<IEvent, Task>((e) => Task.CompletedTask);
            this.testee.DefineAsyncEventDispatching(eventDispatcher);

            this.testee.DispatchEvents.Should().Be(eventDispatcher);
        }

        [Fact]
        public void CanRegisterEventStore()
        {
            this.testee.Register<InMemoryEventStore>();

            A.CallTo(() => this.container.Register<InMemoryEventStore>()).MustHaveHappened();
        }

        private class EventStoreConfiguration : AbstractEventStoreConfiguration
        {
            private readonly IContainer container;

            public EventStoreConfiguration(IContainer container)
            {
                this.container = container;
            }

            public override void Register<TEventStore>()
            {
                this.container.Register<TEventStore>();
            }
        }

        private class ConfigurationItem
        {
        }
    }
}