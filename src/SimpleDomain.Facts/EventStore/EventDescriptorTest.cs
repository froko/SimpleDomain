//-------------------------------------------------------------------------------
// <copyright file="EventDescriptorTest.cs" company="frokonet.ch">
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

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class EventDescriptorTest
    {
        [Fact]
        public void CanCreateParameterlessInstance()
        {
            var testee = new EventDescriptor();

            testee.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateInstanceWithParameters()
        {
            var aggregateType = typeof(MyDynamicEventSourcedAggregateRoot).FullName;
            var aggregateId = Guid.NewGuid();
            var versionableEvent = new VersionableEvent(new ValueEvent(11)).With(1);
            var headers = new Dictionary<string, object> { { "UserName", "Patrick" }, { "MagicNumber", 42 } };

            var testee = new EventDescriptor(aggregateType, aggregateId, versionableEvent, headers);

            testee.AggregateType.Should().Be(aggregateType);
            testee.AggregateId.Should().Be(aggregateId);
            testee.Version.Should().Be(1);
            testee.Timestamp.Should().BeCloseTo(DateTime.Now, 1000);
            testee.EventType.Should().Be("SimpleDomain.TestDoubles.ValueEvent");
            testee.Event.Should().BeSameAs(versionableEvent.InnerEvent);
            testee.Headers.Should().Contain("UserName", "Patrick").And.Contain("MagicNumber", 42);
        }
    }
}