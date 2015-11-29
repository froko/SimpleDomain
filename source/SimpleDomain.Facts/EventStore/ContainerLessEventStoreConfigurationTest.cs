//-------------------------------------------------------------------------------
// <copyright file="ContainerLessEventStoreConfigurationTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System;

    using FluentAssertions;

    using SimpleDomain.EventStore.Configuration;
    using SimpleDomain.EventStore.Persistence;

    using Xunit;

    public class ContainerLessEventStoreConfigurationTest
    {
        [Fact]
        public void CanAddAndGetConfigurationItem()
        {
            var testee = new ContainerLessEventStoreConfiguration();

            testee.AddConfigurationItem("Foo", new ConfigurationItem());

            var configurationItem = testee.Get<ConfigurationItem>("Foo");

            configurationItem.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsException_WhenTryingToRegisterJitney()
        {
            var testee = new ContainerLessEventStoreConfiguration();

            Action action = () => testee.Register<InMemoryEventStore>();

            action.ShouldThrow<LiskovSubstitutionException>()
                .WithMessage("You cannot register an EventStore when there is no IoC container");
        }

        public class ConfigurationItem
        {
        }
    }
}