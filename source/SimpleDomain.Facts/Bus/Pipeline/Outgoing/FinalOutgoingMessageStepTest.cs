//-------------------------------------------------------------------------------
// <copyright file="FinalOutgoingMessageStepTest.cs" company="frokonet.ch">
//   Copyright (c) 2015
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

namespace SimpleDomain.Bus.Pipeline.Outgoing
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class FinalOutgoingMessageStepTest
    {
        [Fact]
        public async Task CreatesNewEnvelopeForCommands()
        {
            var testee = new FinalOutgoingMessageStep();
            var outgoingMessageContext = A.Fake<OutgoingMessageContext>();
            var configuration = A.Fake<IHavePipelineConfiguration>();
            var endpointAddress = new EndpointAddress("recipient");

            A.CallTo(() => outgoingMessageContext.Message).Returns(new ValueCommand(11));
            A.CallTo(() => outgoingMessageContext.Configuration).Returns(configuration);

            A.CallTo(() => configuration.GetConsumingEndpointAddress(A<ICommand>.Ignored))
                .Returns(endpointAddress);
            
            await testee.InvokeAsync(outgoingMessageContext, null);

            A.CallTo(() => outgoingMessageContext.CreateEnvelope(endpointAddress)).MustHaveHappened();
        }

        [Fact]
        public async Task CreatesNewEnvelopesForEvents()
        {
            var testee = new FinalOutgoingMessageStep();
            var outgoingMessageContext = A.Fake<OutgoingMessageContext>();
            var configuration = A.Fake<IHavePipelineConfiguration>();
            var endpointAddress1 = new EndpointAddress("recipient1");
            var endpointAddress2 = new EndpointAddress("recipient2");

            A.CallTo(() => outgoingMessageContext.Message).Returns(new ValueEvent(11));
            A.CallTo(() => outgoingMessageContext.Configuration).Returns(configuration);

            A.CallTo(() => configuration.GetSubscribedEndpointAddresses(A<IEvent>.Ignored))
                .Returns(new[] { endpointAddress1, endpointAddress2 });

            await testee.InvokeAsync(outgoingMessageContext, null);

            A.CallTo(() => outgoingMessageContext.CreateEnvelope(endpointAddress1)).MustHaveHappened();
            A.CallTo(() => outgoingMessageContext.CreateEnvelope(endpointAddress2)).MustHaveHappened();
        }

        [Fact]
        public async Task CreatesNewEnvelopeForSubscriptionMessages()
        {
            var testee = new FinalOutgoingMessageStep();
            var outgoingMessageContext = A.Fake<OutgoingMessageContext>();
            var configuration = A.Fake<IHavePipelineConfiguration>();
            var subscriptionMessage = new SubscriptionMessage(new EndpointAddress("recipient"), typeof(ValueEvent).FullName);
            var endpointAddress = new EndpointAddress("publisher");

            A.CallTo(() => outgoingMessageContext.Message).Returns(subscriptionMessage);
            A.CallTo(() => outgoingMessageContext.Configuration).Returns(configuration);

            A.CallTo(() => configuration.GetPublishingEndpointAddress(A<string>.Ignored))
                .Returns(endpointAddress);

            await testee.InvokeAsync(outgoingMessageContext, null);

            A.CallTo(() => outgoingMessageContext.CreateEnvelope(endpointAddress)).MustHaveHappened();
        }

        [Fact]
        public async Task DoesNotCallNext()
        {
            var next = A.Fake<Func<Task>>();
            var testee = new FinalOutgoingMessageStep();

            await testee.InvokeAsync(A.Fake<OutgoingMessageContext>(), next);

            A.CallTo(() => next.Invoke()).MustNotHaveHappened();
        }
    }
}