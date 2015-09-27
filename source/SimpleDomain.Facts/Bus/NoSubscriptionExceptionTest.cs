//-------------------------------------------------------------------------------
// <copyright file="NoSubscriptionExceptionTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class NoSubscriptionExceptionTest
    {
        [Fact]
        public void CanCreateInstance()
        {
            var @event = new ValueEvent(42);
            var testee = new NoSubscriptionException(@event);

            testee.Should().BeAssignableTo<Exception>();
            testee.Message.Should().Be("Cannot process message of type SimpleDomain.TestDoubles.ValueEvent since no subscription was found.");
        }
    }
}