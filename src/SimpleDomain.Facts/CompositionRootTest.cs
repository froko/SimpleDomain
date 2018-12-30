//-------------------------------------------------------------------------------
// <copyright file="CompositionRootTest.cs" company="frokonet.ch">
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

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Bus;
    using SimpleDomain.EventStore;
    using SimpleDomain.EventStore.Persistence;

    using Xunit;

    public class CompositionRootTest
    {
        [Fact]
        public async Task SimpleJitneyIsUsed_WhenNoJitneyIsExplicitlyConfigured()
        {
            var testee = new CompositionRoot();

            using (var executionContext = await testee.StartAsync().ConfigureAwait(false))
            {
                executionContext.Bus.Should().BeAssignableTo<SimpleJitney>();
            }
        }

        [Fact]
        public async Task InMemoryEventStoreIsUsed_WhenNoEventStoreIsExplicitlyConfigured()
        {
            var testee = new CompositionRoot();

            using (var executionContext = await testee.StartAsync().ConfigureAwait(false))
            {
                executionContext.EventStore.Should().BeAssignableTo<InMemoryEventStore>();
            }
        }

        [Fact]
        public async Task CanConfigureJitney()
        {
            var testee = new CompositionRoot();

            testee.ConfigureJitney()
                .DefineLocalEndpointAddress("unittest")
                .UseInMemoryQueueJitney();

            using (var executionContext = await testee.StartAsync().ConfigureAwait(false))
            {
                executionContext.Bus.Should().BeAssignableTo<MessageQueueJitney>();
            }
        }

        [Fact]
        public async Task CanConfigureJitneyWithRunMethod()
        {
            var testee = new CompositionRoot();

            testee.Run(ConfigureJitney);

            using (var executionContext = await testee.StartAsync().ConfigureAwait(false))
            {
                executionContext.Bus.Should().BeAssignableTo<MessageQueueJitney>();
            }
        }

        [Fact]
        public async Task CanConfigureEventStore()
        {
            var testee = new CompositionRoot();

            testee.ConfigureEventStore().UseSqlEventStore();

            using (var executionContext = await testee.StartAsync().ConfigureAwait(false))
            {
                executionContext.EventStore.Should().BeAssignableTo<SqlEventStore>();
            }
        }

        [Fact]
        public async Task CanConfigureEventStoreWithRunMethod()
        {
            var testee = new CompositionRoot();

            testee.Run(ConfigureEventStore);

            using (var executionContext = await testee.StartAsync().ConfigureAwait(false))
            {
                executionContext.EventStore.Should().BeAssignableTo<SqlEventStore>();
            }
        }

        [Fact]
        public async Task CanRegisterBoundedContext()
        {
            var boundedContext = A.Fake<IBoundedContext>();
            var testee = new CompositionRoot();

            testee.Register(boundedContext);

            using (await testee.StartAsync().ConfigureAwait(false))
            {
                A.CallTo(() => boundedContext.Configure(
                    A<ISubscribeMessageHandlers>.Ignored,
                    A<IFeatureSelector>.Ignored,
                    A<IDeliverMessages>.Ignored,
                    A<IEventSourcedRepository>.Ignored)).MustHaveHappened();
            }
        }

        [Fact]
        public async Task ThrowsException_WhenTryingToConfigureJitneyAfterTheCompositionRootHasBeenStarted()
        {
            var testee = new CompositionRoot();

            using (await testee.StartAsync().ConfigureAwait(false))
            {
                Action action = () => testee.ConfigureJitney().DefineLocalEndpointAddress("unittest").UseInMemoryQueueJitney();
                action.Should().Throw<CompositionRootAlreadyStartedException>();
            }
        }

        [Fact]
        public async Task ThrowsException_WhenTryingToConfigureJitneyWithRunMethodAfterTheCompositionRootHasBeenStarted()
        {
            var testee = new CompositionRoot();

            using (await testee.StartAsync().ConfigureAwait(false))
            {
                Action action = () => testee.Run(ConfigureJitney);
                action.Should().Throw<CompositionRootAlreadyStartedException>();
            }
        }

        [Fact]
        public async Task ThrowsException_WhenTryingToConfigureEventStoreAfterTheCompositionRootHasBeenStarted()
        {
            var testee = new CompositionRoot();

            using (await testee.StartAsync().ConfigureAwait(false))
            {
                Action action = () => testee.ConfigureEventStore().UseSqlEventStore();
                action.Should().Throw<CompositionRootAlreadyStartedException>();
            }
        }

        [Fact]
        public async Task ThrowsException_WhenTryingToConfigureEventStoreWithRunMethodAfterTheCompositionRootHasBeenStarted()
        {
            var testee = new CompositionRoot();

            using (await testee.StartAsync().ConfigureAwait(false))
            {
                Action action = () => testee.Run(ConfigureEventStore);
                action.Should().Throw<CompositionRootAlreadyStartedException>();
            }
        }

        [Fact]
        public async Task ThrowsException_WhenTryingToRegisterBoundedContextAfterTheCompositionRootHasBeenStarted()
        {
            var testee = new CompositionRoot();

            using (await testee.StartAsync().ConfigureAwait(false))
            {
                Action action = () => testee.Register(A.Fake<IBoundedContext>());
                action.Should().Throw<CompositionRootAlreadyStartedException>();
            }
        }

        [Fact]
        public async Task ThrowsException_WhenTryingToRestartAfterTheCompositionRootHasBeenStarted()
        {
            var testee = new CompositionRoot();

            using (await testee.StartAsync().ConfigureAwait(false))
            {
                Func<Task> action = async () => await testee.StartAsync().ConfigureAwait(false);
                action.Should().Throw<CompositionRootAlreadyStartedException>();
            }
        }

        [Fact]
        public async Task CanRestart_WhenExecutionContextHasBeenStopped()
        {
            var testee = new CompositionRoot();
            var executionContext = await testee.StartAsync().ConfigureAwait(false);

            await executionContext.StopAsync().ConfigureAwait(false);

            Func<Task> action = async () => await testee.StartAsync().ConfigureAwait(false);
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public async Task CanRestart_WhenExecutionContextHasBeenDisposed()
        {
            var testee = new CompositionRoot();
            var executionContext = await testee.StartAsync().ConfigureAwait(false);

            executionContext.Dispose();

            Func<Task> action = async () => await testee.StartAsync().ConfigureAwait(false);
            action.Should().NotThrow<Exception>();
        }

        private static void ConfigureJitney(IConfigureThisJitney config)
        {
            config.DefineLocalEndpointAddress("unittest");
            config.UseInMemoryQueueJitney();
        }

        private static void ConfigureEventStore(IConfigureThisEventStore config)
        {
            config.UseSqlEventStore();
        }
    }
}