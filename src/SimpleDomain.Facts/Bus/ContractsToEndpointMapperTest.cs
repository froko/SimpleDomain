//-------------------------------------------------------------------------------
// <copyright file="ContractsToEndpointMapperTest.cs" company="frokonet.ch">
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
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class ContractsToEndpointMapperTest
    {
        [Fact]
        public void CanMapContractsToQueueName()
        {
            const string QueueName = "RemoteQueue";

            var remoteEndpointAddress = new EndpointAddress(QueueName);

            var configuration = A.Fake<IConfigureThisJitney>();
            var localEndpointAddress = new EndpointAddress("local");
            var contractMap = new Dictionary<Type, EndpointAddress>();
            var assembly = typeof(MyCommand).Assembly;

            var testee = new ContractsToEndpointMapper(configuration, localEndpointAddress, contractMap, assembly);

            var result = testee.To(QueueName);

            contractMap.Should()
                .Contain(typeof(MyCommand), remoteEndpointAddress).And
                .Contain(typeof(OtherCommand), remoteEndpointAddress).And
                .Contain(typeof(MyEvent), remoteEndpointAddress).And
                .Contain(typeof(OtherEvent), remoteEndpointAddress).And
                .Contain(typeof(MyMessage), remoteEndpointAddress).And
                .Contain(typeof(OtherMessage), remoteEndpointAddress);

            result.Should().Be(configuration);
        }

        [Fact]
        public void CanMapContractsToQueueNameAndMachineName()
        {
            const string QueueName = "RemoteQueue";
            const string MachineName = "RemoteMachine";

            var remoteEndpointAddress = new EndpointAddress(QueueName, MachineName);

            var configuration = A.Fake<IConfigureThisJitney>();
            var localEndpointAddress = new EndpointAddress("local");
            var contractMap = new Dictionary<Type, EndpointAddress>();
            var assembly = typeof(MyCommand).Assembly;

            var testee = new ContractsToEndpointMapper(configuration, localEndpointAddress, contractMap, assembly);

            var result = testee.To(QueueName, MachineName);

            contractMap.Should()
                .Contain(typeof(MyCommand), remoteEndpointAddress).And
                .Contain(typeof(OtherCommand), remoteEndpointAddress).And
                .Contain(typeof(MyEvent), remoteEndpointAddress).And
                .Contain(typeof(OtherEvent), remoteEndpointAddress).And
                .Contain(typeof(MyMessage), remoteEndpointAddress).And
                .Contain(typeof(OtherMessage), remoteEndpointAddress);

            result.Should().Be(configuration);
        }

        [Fact]
        public void CanMapContractsToTheLocalEndpoint()
        {
            var configuration = A.Fake<IConfigureThisJitney>();
            var localEndpointAddress = new EndpointAddress("local");
            var contractMap = new Dictionary<Type, EndpointAddress>();
            var assembly = typeof(MyCommand).Assembly;

            var testee = new ContractsToEndpointMapper(configuration, localEndpointAddress, contractMap, assembly);

            var result = testee.ToMe();

            contractMap.Should()
                .Contain(typeof(MyCommand), localEndpointAddress).And
                .Contain(typeof(OtherCommand), localEndpointAddress).And
                .Contain(typeof(MyEvent), localEndpointAddress).And
                .Contain(typeof(OtherEvent), localEndpointAddress).And
                .Contain(typeof(MyMessage), localEndpointAddress).And
                .Contain(typeof(OtherMessage), localEndpointAddress);

            result.Should().Be(configuration);
        }
    }
}