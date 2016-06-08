//-------------------------------------------------------------------------------
// <copyright file="EventStoreFactoryTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Configuration
{
    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.EventStore.Persistence;

    using Xunit;

    public class EventStoreFactoryTest
    {
        [Fact]
        public void CanCreateInstance()
        {
            var testee = new EventStoreFactory();

            testee.Create(A.Fake<AbstractEventStoreConfiguration>(), A.Fake<IDeliverMessages>())
                .Should().BeAssignableTo<InMemoryEventStore>();
        }

        [Fact]
        public void CanRegisterEventStoreCreation()
        {
            var configuration = A.Fake<AbstractEventStoreConfiguration>();
            var testee = new EventStoreFactory();

            A.CallTo(() => configuration.Get<DbConnectionFactory>(SqlEventStore.ConnectionFactory))
                .Returns(new DbConnectionFactory());

            testee.Register(config => new SqlEventStore(config));

            testee.Create(configuration, A.Fake<IDeliverMessages>())
                .Should().BeAssignableTo<SqlEventStore>();
        }
    }
}