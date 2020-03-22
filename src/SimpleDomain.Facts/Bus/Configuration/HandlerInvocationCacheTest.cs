//-------------------------------------------------------------------------------
// <copyright file="HandlerInvocationCacheTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class HandlerInvocationCacheTest
    {
        private readonly HandlerInvocationCache testee;

        public HandlerInvocationCacheTest()
        {
            this.testee = new HandlerInvocationCache();
        }

        [Fact]
        public async Task CanInvokeAsyncHandlerWithMessage_WhenTheirTypeRelationshipWasAddedBefore()
        {
            this.testee.Add(typeof(ValueCommandHandler), typeof(ValueCommand));

            var handler = new ValueCommandHandler();
            var command = new ValueCommand(11);

            await this.testee.InvokeAsync(handler, command).ConfigureAwait(false);

            handler.Value.Should().Be(11);
        }

        [Fact]
        public async Task DoesNotInvokeAsyncHandler_WhenItWasNotAddedBefore()
        {
            var handler = new ValueCommandHandler();
            var command = new ValueCommand(11);

            await this.testee.InvokeAsync(handler, command).ConfigureAwait(false);

            handler.Value.Should().Be(0);
        }
    }
}