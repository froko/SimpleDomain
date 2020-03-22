﻿//-------------------------------------------------------------------------------
// <copyright file="InMemorySubscriptionStoreTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class InMemorySubscriptionStoreTest
    {
        [Fact]
        public async Task CanStoreAndReceiveSubscriptions()
        {
            var @event = new ValueEvent(42);
            var testee = new InMemorySubscriptionStore();

            var firstSubscriptionMessage = new SubscriptionMessage(new EndpointAddress("Queue1"), typeof(ValueEvent).FullName);
            var secondSubscriptionMessage = new SubscriptionMessage(new EndpointAddress("Queue2"), typeof(ValueEvent).FullName);

            await testee.SaveAsync(firstSubscriptionMessage).ConfigureAwait(false);
            await testee.SaveAsync(secondSubscriptionMessage).ConfigureAwait(false);

            var subscriptions = testee.GetSubscribedEndpoints(@event);

            subscriptions.Should()
                .Contain(address => address.QueueName == "Queue1").And
                .Contain(address => address.QueueName == "Queue2");
        }
    }
}