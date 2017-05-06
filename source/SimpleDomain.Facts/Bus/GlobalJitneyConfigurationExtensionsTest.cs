//-------------------------------------------------------------------------------
// <copyright file="GlobalJitneyConfigurationExtensionsTest.cs" company="frokonet.ch">
//   Copyright (c) 2014-2017
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

    using FakeItEasy;

    using SimpleDomain.Bus.Configuration;
    using SimpleDomain.Bus.MSMQ;

    using Xunit;

    public class GlobalJitneyConfigurationExtensionsTest
    {
        [Fact]
        public void CanRegisterSimpleJitney()
        {
            var configuration = A.Fake<IConfigureThisJitney>();
            configuration.UseSimpleJitney();

            A.CallTo(() => configuration.Register(A<Func<IHaveJitneyConfiguration, Jitney>>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void CanRegisterInMemoryQueueJitney()
        {
            var configuration = A.Fake<IConfigureThisJitney>();
            configuration.UseInMemoryQueueJitney();

            A.CallTo(() => configuration.AddConfigurationItem(
                MessageQueueJitney.MessageQueueProvider,
                A<InMemoryQueueProvider>.Ignored)).MustHaveHappened();

            A.CallTo(() => configuration.Register(A<Func<IHaveJitneyConfiguration, Jitney>>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void CanRegisterMsmqJitney()
        {
            var configuration = A.Fake<IConfigureThisJitney>();
            configuration.UseMsmqJitney();

            A.CallTo(() => configuration.AddConfigurationItem(
                MessageQueueJitney.MessageQueueProvider,
                A<MsmqProvider>.Ignored)).MustHaveHappened();

            A.CallTo(() => configuration.Register(A<Func<IHaveJitneyConfiguration, Jitney>>.Ignored))
                .MustHaveHappened();
        }
    }
}