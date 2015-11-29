//-------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTest.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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
    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class TypeExtensionsTest
    {
        [Fact]
        public void CanDecideIfGivenTypeImplementsAsyncHandlerInterface()
        {
            typeof(ValueCommandHandler).ImplementsAsyncHandlerInterface().Should().BeTrue();
            typeof(ValueEventHandler).ImplementsAsyncHandlerInterface().Should().BeTrue();

            typeof(IHandleAsync<ValueCommand>).ImplementsAsyncHandlerInterface().Should().BeFalse();
            typeof(IHandleAsync<ValueEvent>).ImplementsAsyncHandlerInterface().Should().BeFalse();

            typeof(ValueCommand).ImplementsAsyncHandlerInterface().Should().BeFalse();
            typeof(ValueEvent).ImplementsAsyncHandlerInterface().Should().BeFalse();
        }

        [Fact]
        public void CanGetAllMessageTypesOfAGivenAsyncHandlerType()
        {
            typeof(ValueCommandHandler).GetAllMessageTypeThisTypeCanHandle()
                .Should().HaveCount(1).And.Contain(typeof(ValueCommand));

            typeof(ValueEventHandler).GetAllMessageTypeThisTypeCanHandle()
                .Should().HaveCount(1).And.Contain(typeof(ValueEvent));
        }
    }
}