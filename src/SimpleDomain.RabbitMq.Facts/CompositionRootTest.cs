//-------------------------------------------------------------------------------
// <copyright file="CompositionRootTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.Bus;

    using Xunit;

    public class CompositionRootTest
    {
        [Fact(Skip = "Needs connection to local RabbitMQ server instance")]
        public async Task CanConfigureJitneyToUseMsmq()
        {
            var testee = new CompositionRoot();

            testee.ConfigureJitney()
                .DefineLocalEndpointAddress("simpledomain.compositionroot.test")
                .UseRabbitMqJitney();

            using (var executionContext = await testee.StartAsync().ConfigureAwait(false))
            {
                executionContext.Bus.Should().BeAssignableTo<MessageQueueJitney>();
            }
        }
    }
}