//-------------------------------------------------------------------------------
// <copyright file="VersionableEventTest.cs" company="frokonet.ch">
//   Copyright (c) 2014
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

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class VersionableEventTest
    {
        [Fact]
        public void CanCreateInstanceWithInnerEvent()
        {
            var innerEvent = new ValueEvent(42);
            var instance = new VersionableEvent(innerEvent);

            instance.InnerEvent.Should().Be(innerEvent);
        }

        [Fact]
        public void InstanceIsAssignableToIEvent()
        {
            var innerEvent = new ValueEvent(42);
            var instance = new VersionableEvent(innerEvent);

            instance.Should().BeAssignableTo<IEvent>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceAndInnerEventIsNull()
        {
            Action action = () => { new VersionableEvent(null); };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CanFluentlyAssignVersion()
        {
            var innerEvent = new ValueEvent(42);
            var instance = new VersionableEvent(innerEvent).With(2);

            instance.Version.Should().Be(2);
        }
    }
}