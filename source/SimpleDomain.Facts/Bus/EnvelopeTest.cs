//-------------------------------------------------------------------------------
// <copyright file="EnvelopeTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;
    
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class EnvelopeTest
    {
        [Fact]
        public void CanCreateInstanceWithConstructor()
        {
            var headers = new Dictionary<string, object> { { "Foo", "Foo" }, { "Bar", 42 } };
            var body = new ValueCommand(11);

            var testee = new Envelope(headers, body);

            testee.Headers.Should().BeSameAs(headers);
            testee.Body.Should().BeSameAs(body);
        }

        [Fact]
        public void CanCreateInstanceWithFactoryMethod()
        {
            var sender = new EndpointAddress("sender");
            var recipient = new EndpointAddress("recipient");
            var body = new ValueCommand(11);

            var testee = Envelope.Create(sender, recipient, body);

            testee.Headers.Should().ContainKey(HeaderKeys.Sender);
            testee.Headers.Should().ContainKey(HeaderKeys.Recipient);
            testee.Headers.Should().ContainKey(HeaderKeys.TimeSent);
            testee.Headers.Should().ContainKey(HeaderKeys.MessageId);
            testee.Headers.Should().ContainKey(HeaderKeys.CorrelationId);
            testee.Body.Should().BeSameAs(body);
        }

        [Fact]
        public void MessageIdAndCorrelationIdAreTheSame_WhenCreatingInstanceWithFactoryMethod()
        {
            var testee = CreateTestee();

            testee.Headers[HeaderKeys.MessageId].Should().Be(testee.Headers[HeaderKeys.CorrelationId]);
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsSender()
        {
            Action action = () => Envelope.Create(null, new EndpointAddress("recipient"), new ValueCommand(11));

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsRecipient()
        {
            Action action = () => Envelope.Create(new EndpointAddress("sender"), null, new ValueCommand(11));

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateInstanceWithNullAsBody()
        {
            Action action = () => Envelope.Create(new EndpointAddress("sender"), new EndpointAddress("recipient"), null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CanAddNewHeader()
        {
            var headers = new Dictionary<string, object>();
            var body = new ValueCommand(11);

            var testee = new Envelope(headers, body);

            testee.AddHeader("New Header", 42);

            testee.Headers.Should().ContainKey("New Header").And.ContainValue(42);
        }

        [Fact]
        public void DoesNothing_WhenAddingAnExistingHeader()
        {
            var headers = new Dictionary<string, object> { { "Existing Header", 42 } };
            var body = new ValueCommand(11);

            var testee = new Envelope(headers, body);

            testee.AddHeader("Existing Header", 666);

            testee.Headers.Should().HaveCount(1).And.ContainKey("Existing Header").And.ContainValue(42);
        }

        [Fact]
        public void ThrowsException_WhenTryingToAddHeaderWithNullAsKey()
        {
            var testee = CreateTestee();

            Action action = () => testee.AddHeader(null, 666);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CanReplaceExistingHeader()
        {
            var headers = new Dictionary<string, object> { { "Existing Header", 42 } };
            var body = new ValueCommand(11);

            var testee = new Envelope(headers, body);

            testee.ReplaceHeader("Existing Header", 666);

            testee.Headers.Should().HaveCount(1).And.ContainKey("Existing Header").And.ContainValue(666);
        }

        [Fact]
        public void AddsNewHeader_WhenReplacingNonExistingHeader()
        {
            var headers = new Dictionary<string, object> { { "Existing Header", 42 } };
            var body = new ValueCommand(11);

            var testee = new Envelope(headers, body);

            testee.ReplaceHeader("NonExisting Header", 666);

            testee.Headers.Should().HaveCount(2).And.ContainKey("NonExisting Header").And.ContainValue(666);
        }

        [Fact]
        public void ThrowsException_WhenTryingToReplaceHeaderWithNullAsKey()
        {
            var testee = CreateTestee();

            Action action = () => testee.ReplaceHeader(null, 666);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CanGetHeaderByItsKey()
        {
            var testee = CreateTestee();

            testee.GetHeader<Guid>(HeaderKeys.CorrelationId).Should().NotBeEmpty();
        }

        [Fact]
        public void ThrowsException_WhenTryingToGetHeaderWithNullAsKey()
        {
            var testee = CreateTestee();

            Action action = () => testee.GetHeader<string>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsException_WhenHeaderTypeIsNotOfGivenType()
        {
            var testee = CreateTestee();

            Action action = () => testee.GetHeader<string>(HeaderKeys.MessageId);

            action.ShouldThrow<InvalidCastException>();
        }

        [Fact]
        public void ThrowsException_WhenHeaderKeyIsNotFound()
        {
            var testee = CreateTestee();

            Action action = () => testee.GetHeader<string>("NotExistingHeaderKey");

            action.ShouldThrow<KeyNotFoundException>();
        }

        private static Envelope CreateTestee()
        {
            var sender = new EndpointAddress("sender");
            var recipient = new EndpointAddress("recipient");
            var body = new ValueCommand(11);

            return Envelope.Create(sender, recipient, body);
        }
    }
}