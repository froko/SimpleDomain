//-------------------------------------------------------------------------------
// <copyright file="EndpointAddressTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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

    public class EndpointAddressTest
    {
        [Fact]
        public void IsValueObject()
        {
            new EndpointAddress("myQueue").Should().BeAssignableTo<ValueObject<EndpointAddress>>();
        }

        [Fact]
        public void CanCreateNewInstance_WithQueueName()
        {
            var testee = new EndpointAddress("myQueue");

            testee.QueueName.Should().Be("myQueue");
            testee.MachineName.Should().NotBeNullOrEmpty();
            testee.ToString().Should().Contain("myQueue@");
        }

        [Fact]
        public void CanCreateNewInstance_WithQueueNameAndMachineName()
        {
            var testee = new EndpointAddress("remoteQueue", "remoteMachine");

            testee.QueueName.Should().Be("remoteQueue");
            testee.MachineName.Should().Be("remoteMachine");
            testee.ToString().Should().Be("remoteQueue@remoteMachine");
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateAnEndpointAddressWithNullOrEmptyStringAsQueueName()
        {
            Action nullStringAction = () => { new EndpointAddress(null); };
            Action emptyStringAction = () => { new EndpointAddress(string.Empty); };

            nullStringAction.Should().Throw<ArgumentNullException>();
            emptyStringAction.Should().Throw<ArgumentException>();

            nullStringAction = () => { new EndpointAddress(null, "remoteMachine"); };
            emptyStringAction = () => { new EndpointAddress(string.Empty, "remoteMachine"); };

            nullStringAction.Should().Throw<ArgumentNullException>();
            emptyStringAction.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateAnEndpointAddressWithNullOrEmptyStringAsMachineName()
        {
            Action nullStringAction = () => { new EndpointAddress("remoteQueue", null); };
            Action emptyStringAction = () => { new EndpointAddress("remoteQueue", string.Empty); };

            nullStringAction.Should().Throw<ArgumentNullException>();
            emptyStringAction.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CanParseEndpointAddressAsString()
        {
            var testee = EndpointAddress.Parse("myQueue@myMachine");

            testee.QueueName.Should().Be("myQueue");
            testee.MachineName.Should().Be("myMachine");
        }

        [Fact]
        public void ThrowsException_WhenTryingToParseNullOrEmptyString()
        {
            Action nullStringAction = () => EndpointAddress.Parse(null);
            Action emptyStringAction = () => EndpointAddress.Parse(string.Empty);

            nullStringAction.Should().Throw<ArgumentNullException>();
            emptyStringAction.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToParseAStringThatContainsNoAtSeparator()
        {
            Action invalidStringAction = () => EndpointAddress.Parse("invalidStringWithNoAtSeperator");

            invalidStringAction.Should().Throw<ArgumentException>()
                .Where(exception => exception.Message.Contains("must contain an @"));
        }

        [Fact]
        public void CanCreateSubScopeAddress()
        {
            var testee = new EndpointAddress("myQueue", "myMachine");
            var subScopeAddress = testee.CreateSubScopeAddress("Test");

            subScopeAddress.ToString().Should().Be("myQueue.Test@myMachine");
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateSubScopeAddressNullOrEmptyString()
        {
            var testee = new EndpointAddress("myQueue", "myMachine");

            Action nullStringAction = () => testee.CreateSubScopeAddress(null);
            Action emptyStringAction = () => testee.CreateSubScopeAddress(string.Empty);

            nullStringAction.Should().Throw<ArgumentNullException>();
            emptyStringAction.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CanCreateRetrySubScopeAddress()
        {
            var testee = new EndpointAddress("myQueue", "myMachine");
            var retrySubScopeAddress = testee.CreateRetrySubScopeAddress();

            retrySubScopeAddress.ToString().Should().Be("myQueue.retries@myMachine");
        }

        [Fact]
        public void CanCreateErrorSubScopeAddress()
        {
            var testee = new EndpointAddress("myQueue", "myMachine");
            var errorSubScopeAddress = testee.CreateErrorSubScopeAddress();

            errorSubScopeAddress.ToString().Should().Be("myQueue.error@myMachine");
        }

        [Fact]
        public void CanDistinguishBetweenLocalAndRemote()
        {
            var localEndpointAddress = new EndpointAddress("myQueue");
            var remoteEndpointAddress = new EndpointAddress("remoteQueue", "remoteMachine");

            localEndpointAddress.IsLocal.Should().BeTrue();
            remoteEndpointAddress.IsLocal.Should().BeFalse();
        }
    }
}