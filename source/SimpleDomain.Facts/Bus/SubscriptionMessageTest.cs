//-------------------------------------------------------------------------------
// <copyright file="SubscriptionMessageTest.cs" company="frokonet.ch">
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

    using Xunit;

    public class SubscriptionMessageTest
    {
        [Fact]
        public void CanCreateInstance()
        {
            var handlingEnpoint = new EndpointAddress("localEndpoint");
            var testee = new SubscriptionMessage(handlingEnpoint, "SimpleDomain.TestDoubles.MyEvent");

            testee.HandlingEndpointAddress.Should().Be(handlingEnpoint);
            testee.MessageType.Should().Be("SimpleDomain.TestDoubles.MyEvent");

            testee.GetIntent().Should().Be(MessageIntent.SubscriptionMessage);
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsHandlingEndpointAddress()
        {
            Action action = () => { new SubscriptionMessage(null, "SimpleDomain.TestDoubles.MyEvent"); };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullOrEmptyStringAsMessageType()
        {
            var handlingEnpoint = new EndpointAddress("localEndpoint");

            Action nullStringAction = () => { new SubscriptionMessage(handlingEnpoint, null); };
            Action emptyStringAction = () => { new SubscriptionMessage(handlingEnpoint, string.Empty); };

            nullStringAction.ShouldThrow<ArgumentNullException>();
            emptyStringAction.ShouldThrow<ArgumentException>();
        }
    }
}