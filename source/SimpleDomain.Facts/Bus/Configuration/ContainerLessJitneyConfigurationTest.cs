//-------------------------------------------------------------------------------
// <copyright file="ContainerLessJitneyConfigurationTest.cs" company="frokonet.ch">
//   Copyright (c) 2015
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

namespace SimpleDomain.Bus.Configuration
{
    using System;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class ContainerLessJitneyConfigurationTest
    {
        [Fact]
        public void ThrowsException_WhenTryingToRegisterJitney()
        {
            var testee = new ContainerLessJitneyConfiguration();

            Action action = () => testee.Register<SimpleJitney>();

            action.ShouldThrow<NotSupportedException>()
                .WithMessage("You cannot register a Bus when there is no IoC container");
        }

        [Fact]
        public void ThrowsException_WhenTryingToSubscribeMessageHandlers()
        {
            var testee = new ContainerLessJitneyConfiguration();
            var handlerAssemblies = new[] { typeof(ValueCommandHandler).Assembly };
            
            Action action = () => testee.SubscribeMessageHandlers(handlerAssemblies);

            action.ShouldThrow<NotSupportedException>()
                .WithMessage("You cannot register a handler when there is no IoC container");
        }

        [Fact]
        public void ThrowsException_WhenTryingToSubscribeMessageHandlersInThisAssembly()
        {
            var testee = new ContainerLessJitneyConfiguration();

            Action action = () => testee.SubscribeMessageHandlersInThisAssembly();

            action.ShouldThrow<NotSupportedException>()
                .WithMessage("You cannot register a handler when there is no IoC container");
        }
    }
}