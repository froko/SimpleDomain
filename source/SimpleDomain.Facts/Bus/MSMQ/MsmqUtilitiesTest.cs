//-------------------------------------------------------------------------------
// <copyright file="MsmqUtilitiesTest.cs" company="frokonet.ch">
//   Copyright (c) 2016
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

namespace SimpleDomain.Bus.MSMQ
{
    using System.Messaging;
    using System.Transactions;

    using FluentAssertions;

    using Xunit;

    public class MsmqUtilitiesTest
    {
        [Fact]
        public void CanGetFormattedQueueNameForLocalEndpointAddress()
        {
            var localEndpointAddress = new EndpointAddress("myQueue");
            
            var formattedQueueName = MsmqUtilities.GetFormattedQueueName(localEndpointAddress);

            formattedQueueName.Should().Be(@".\Private$\myQueue");
        }

        [Fact]
        public void CanGetFormattedQueueNameForRemoteEndpointAddress()
        {
            var remoteEndpointAddress = new EndpointAddress("remoteQueue", "remoteMachine");
            
            var formattedQueueName = MsmqUtilities.GetFormattedQueueName(remoteEndpointAddress);

            formattedQueueName.Should().Be(@"FormatName:Direct=OS:remoteMachine\Private$\remoteQueue");
        }

        [Fact]
        public void ReturnsSingleMessageQueueTransactionType_WhenNoTransactionScopeIsInvolved()
        {
            MsmqUtilities.GetTransactionType().Should().Be(MessageQueueTransactionType.Single);
        }

        [Fact]
        public void ReturnsAutomaticMessageQueueTransactionType_WhenTransactionScopeIsInvolved()
        {
            using (var transactionScope = new TransactionScope())
            {
                MsmqUtilities.GetTransactionType().Should().Be(MessageQueueTransactionType.Automatic);
            }
        }
    }
}