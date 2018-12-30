//-------------------------------------------------------------------------------
// <copyright file="JitneyConfigurationTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;

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

        [Fact]
        public void CanRegisterJitney()
        {
            this.testee.Register(config => new SimpleJitney(config));

            this.kernel.Get<IDeliverMessages>().Should().BeAssignableTo<SimpleJitney>();
            this.kernel.Get<Jitney>().Should().BeAssignableTo<SimpleJitney>();
        }

        [Fact]
        public void CanRegisterHandlerType()
        {
            var assemblies = new[] { typeof(ValueCommandHandler).Assembly };
            var command = new ValueCommand(11);

            this.testee.SubscribeMessageHandlers(assemblies);

            this.testee.Subscriptions.GetCommandSubscription(command).Should().NotBeNull();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }
}