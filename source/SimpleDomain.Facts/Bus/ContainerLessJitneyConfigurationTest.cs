//-------------------------------------------------------------------------------
// <copyright file="ContainerLessJitneyConfigurationTest.cs" company="frokonet.ch">
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
    using System;

    using FluentAssertions;

    using Xunit;

    public class ContainerLessJitneyConfigurationTest
    {
        [Fact]
        public void CanCreateInstance()
        {
            var testee = new ContainerLessJitneyConfiguration();

            testee.HandlerSubscriptions.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsException_WhenTryingToGetLocalEndpointAddressButItHasNotYetBeenSet()
        {
            var testee = new ContainerLessJitneyConfiguration();

            Action action = () => { var address = testee.LocalEndpointAddress; };

            action.ShouldThrow<MissingConfigurationException>();
        }

        [Fact]
        public void ReturnsLocalEndpointAddress_WhenItHasBeenSetBefore()
        {
            var testee = new ContainerLessJitneyConfiguration();

            testee.DefineEndpointName("MyEndpoint");

            testee.LocalEndpointAddress.IsLocal.Should().BeTrue();
            testee.LocalEndpointAddress.QueueName.Should().Be("MyEndpoint");
            testee.LocalEndpointAddress.MachineName.Should().Be(Environment.MachineName);
        }

        [Fact]
        public void CanAddAndGetConfigurationItem()
        {
            var testee = new ContainerLessJitneyConfiguration();
            
            testee.AddConfigurationItem("Foo", new ConfigurationItem());

            var configurationItem = testee.Get<ConfigurationItem>("Foo");

            configurationItem.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsException_WhenTryingToRegisterJitney()
        {
            var testee = new ContainerLessJitneyConfiguration();

            Action action = () => testee.Register<SimpleJitney>();

            action.ShouldThrow<LiskovSubstitutionException>()
                .WithMessage("You cannot register a Bus when there is no IoC container");
        }

        public class ConfigurationItem
        {
        }
    }
}