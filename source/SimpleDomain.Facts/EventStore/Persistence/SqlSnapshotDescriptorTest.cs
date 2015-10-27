//-------------------------------------------------------------------------------
// <copyright file="SqlSnapshotDescriptorTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class SqlSnapshotDescriptorTest
    {
        [Fact]
        public void CanCreateParameterlessInstance()
        {
            var testee = new SqlSnapshotDescriptor();

            testee.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateInstanceWithParameters()
        {
            var aggregateType = typeof(MyDynamicEventSourcedAggregateRoot).FullName;
            var aggregateId = Guid.NewGuid();
            var snapshot = new MySnapshot(22).WithVersion(2);

            var testee = new SqlSnapshotDescriptor(aggregateType, aggregateId, snapshot);

            testee.AggregateType.Should().Be(aggregateType);
            testee.AggregateId.Should().Be(aggregateId);
            testee.Version.Should().Be(2);
            testee.Timestamp.Should().BeCloseTo(DateTime.Now, 1000);
            testee.SnapshotType.Should().Be("SimpleDomain.TestDoubles.MySnapshot");
            testee.Snapshot.Should().BeSameAs(snapshot);
            testee.SerializedSnapshot.Should().Contain("\"Value\":22").And.Contain("\"Version\":2");
        }
    }
}