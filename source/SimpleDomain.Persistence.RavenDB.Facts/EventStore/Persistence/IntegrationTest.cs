//-------------------------------------------------------------------------------
// <copyright file="IntegrationTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.EventStore.Configuration;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class IntegrationTest : EmbeddedRavenDbTest
    {
        private readonly IEventSourcedRepository repository;

        public IntegrationTest()
        {
            var factory = new EventStoreFactory();
            var configuration = new ContainerLessEventStoreConfiguration(factory);
            configuration.UseRavenEventStore(this.DocumentStore);

            var bus = A.Fake<IDeliverMessages>();

            var eventStore = factory.Create(configuration, bus);

            this.repository = new EventStoreRepository(eventStore).WithGlobalSnapshotStrategy(10);
        }

        [Fact]
        public async Task CanSaveAndRetrieveAggregateRoots()
        {
            var aggregateId = Guid.NewGuid();
            var aggregateRoot = new MyDynamicEventSourcedAggregateRoot(aggregateId);

            aggregateRoot.ChangeValue(0);
            aggregateRoot.ChangeValue(1);
            aggregateRoot.ChangeValue(2);
            aggregateRoot.ChangeValue(3);
            aggregateRoot.ChangeValue(4);
            aggregateRoot.ChangeValue(5);
            aggregateRoot.ChangeValue(6);
            aggregateRoot.ChangeValue(7);
            aggregateRoot.ChangeValue(8);
            aggregateRoot.ChangeValue(9);
            aggregateRoot.ChangeValue(10);

            await this.repository.SaveAsync(aggregateRoot).ConfigureAwait(false); // Adds eleven events and creates a snapshot

            aggregateRoot.ChangeValue(11);
            aggregateRoot.ChangeValue(12);

            await this.repository.SaveAsync(aggregateRoot).ConfigureAwait(false); // Adds two more events

            var aggregateRootFromEventStore = await this.repository
                .GetByIdAsync<MyDynamicEventSourcedAggregateRoot>(aggregateId)
                .ConfigureAwait(false);

            aggregateRootFromEventStore.Value.Should().Be(12);
            aggregateRootFromEventStore.Version.Should().Be(12);
        }
    }
}