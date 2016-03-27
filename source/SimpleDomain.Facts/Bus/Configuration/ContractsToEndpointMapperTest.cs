//-------------------------------------------------------------------------------
// <copyright file="ContractsToEndpointMapperTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class ContractsToEndpointMapperTest
    {
        [Fact]
        public void CanMapContractsToARemoteEndpoint()
        {
            var remoteEndpointAddress = new EndpointAddress("remote");

            var localEndpointAddress = new EndpointAddress("local");
            var contractMap = new Dictionary<Type, EndpointAddress>();
            var assembly = typeof(MyCommand).Assembly;

            var testee = new ContractsToEndpointMapper(localEndpointAddress, contractMap, assembly);

            testee.To(remoteEndpointAddress);

            contractMap.Should()
                .Contain(typeof(MyCommand), remoteEndpointAddress).And
                .Contain(typeof(OtherCommand), remoteEndpointAddress).And
                .Contain(typeof(MyEvent), remoteEndpointAddress).And
                .Contain(typeof(OtherEvent), remoteEndpointAddress).And
                .Contain(typeof(MyMessage), remoteEndpointAddress).And
                .Contain(typeof(OtherMessage), remoteEndpointAddress);
        }

        [Fact]
        public void CanMapContractsToTheLocalEndpoint()
        {
            var localEndpointAddress = new EndpointAddress("local");
            var contractMap = new Dictionary<Type, EndpointAddress>();
            var assembly = typeof(MyCommand).Assembly;

            var testee = new ContractsToEndpointMapper(localEndpointAddress, contractMap, assembly);

            testee.ToMe();

            contractMap.Should()
                .Contain(typeof(MyCommand), localEndpointAddress).And
                .Contain(typeof(OtherCommand), localEndpointAddress).And
                .Contain(typeof(MyEvent), localEndpointAddress).And
                .Contain(typeof(OtherEvent), localEndpointAddress).And
                .Contain(typeof(MyMessage), localEndpointAddress).And
                .Contain(typeof(OtherMessage), localEndpointAddress);
        }
    }
}