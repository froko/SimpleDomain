//-------------------------------------------------------------------------------
// <copyright file="JitneyConfigurationTest.cs" company="frokonet.ch">
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
    using System.Configuration;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ninject;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class JitneyConfigurationTest : IDisposable
    {
        private readonly IKernel kernel;
        private readonly JitneyConfiguration testee;

        public JitneyConfigurationTest()
        {
            this.kernel = new StandardKernel();
            this.testee = new JitneyConfiguration(this.kernel);
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void ShouldRegisterHandlerTypesInTheContainer()
        {
            this.testee.Subscribe<ValueCommand, ValueCommandHandler>();

            this.kernel.Get<IHandleAsync<ValueCommand>>().Should().BeOfType(typeof(ValueCommandHandler));
        }

        [Fact]
        public void CanSubscribeCommandHandlerAction()
        {
            this.testee.SubscribeCommandHandler<ValueCommand>(c => Task.FromResult(0));

            this.testee.HandlerSubscriptions.GetCommandSubscription(new ValueCommand(42)).Should().NotBeNull();
        }

        [Fact]
        public void CanSubscribeEventHandlerAction()
        {
            this.testee.SubscribeEventHandler<ValueEvent>(e => Task.FromResult(0));

            this.testee.HandlerSubscriptions.GetEventSubscriptions(new ValueEvent(42)).Should().HaveCount(1);
        }

        [Fact]
        public void CanDefineEndpointName()
        {
            const string EndpointName = "MyEndpoint";

            this.testee.DefineEndpointName(EndpointName);
            
            this.testee.LocalEndpointAddress.QueueName.Should().Be(EndpointName);
        }

        [Fact]
        public void ThrowsException_WhenGettingLocalEndpointAddressAndItHasNotBeenDefinedBefore()
        {
            Action action = () => { var endpointAddress = this.testee.LocalEndpointAddress; };

            action.ShouldThrow<ConfigurationErrorsException>()
                .WithMessage(ExceptionMessages.LocalEndpointAddressNotDefined);
        }
    }
}