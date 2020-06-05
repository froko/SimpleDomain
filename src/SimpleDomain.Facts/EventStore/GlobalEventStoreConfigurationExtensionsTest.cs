//-------------------------------------------------------------------------------
// <copyright file="GlobalEventStoreConfigurationExtensionsTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
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
    using System.Collections.Generic;

    using FakeItEasy;

    using SimpleDomain.EventStore.Configuration;
    using SimpleDomain.EventStore.Persistence;

    using Xunit;

    public class GlobalEventStoreConfigurationExtensionsTest
    {
        [Fact]
        public void CanRegisterInMemoryEventStore()
        {
            var configuration = A.Fake<AbstractEventStoreConfiguration>();
            configuration.UseInMemoryEventStore();

            A.CallTo(() => configuration.AddConfigurationItem(InMemoryEventStore.EventDescriptors, A<List<EventDescriptor>>.Ignored)).MustHaveHappened();
            A.CallTo(() => configuration.AddConfigurationItem(InMemoryEventStore.SnapshotDescriptors, A<List<SnapshotDescriptor>>.Ignored)).MustHaveHappened();
            A.CallTo(() => configuration.Register(A<Func<IHaveEventStoreConfiguration, IEventStore>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void CanRegisterSqlEventStore()
        {
            var configuration = A.Fake<AbstractEventStoreConfiguration>();
            configuration.UseSqlEventStore();

            A.CallTo(() => configuration.AddConfigurationItem(SqlEventStore.ConnectionFactory, A<DbConnectionFactory>.Ignored)).MustHaveHappened();
            A.CallTo(() => configuration.Register(A<Func<IHaveEventStoreConfiguration, IEventStore>>.Ignored)).MustHaveHappened();
        }
    }
}