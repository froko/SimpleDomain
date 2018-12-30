//-------------------------------------------------------------------------------
// <copyright file="AsyncMessageDelegateTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class AsyncMessageDelegateTest
    {
        private readonly AsyncMessageDelegate testee;

        public AsyncMessageDelegateTest()
        {
            this.testee = new AsyncMessageDelegate(
                typeof(ValueCommand),
                (handler, message) => (handler as IHandleAsync<ValueCommand>).HandleAsync(new ValueCommand(11)));
        }

        [Fact]
        public void CanDecideIfInstanceCanHandleMessageByType()
        {
            this.testee.CanHandle(typeof(ValueCommand)).Should().BeTrue();
            this.testee.CanHandle(typeof(ValueEvent)).Should().BeFalse();
        }

        [Fact]
        public void CanDecideIfInstanceCanHandleMessageByReference()
        {
            this.testee.CanHandle(new ValueCommand(11)).Should().BeTrue();
            this.testee.CanHandle(new ValueEvent(11)).Should().BeFalse();
        }

        [Fact]
        public async Task CanInvokeAsyncMethod()
        {
            var handler = new ValueCommandHandler();
            var command = new ValueCommand(11);

            await this.testee.InvokeAsync(handler, command).ConfigureAwait(false);

            handler.Value.Should().Be(11);
        }
    }
}