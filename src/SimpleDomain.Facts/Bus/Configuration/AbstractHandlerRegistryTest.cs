//-------------------------------------------------------------------------------
// <copyright file="AbstractHandlerRegistryTest.cs" company="frokonet.ch">
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
    using System;
    using System.Linq;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class AbstractHandlerRegistryTest
    {
        [Fact]
        public void CanGetRegisteredCommandHandler()
        {
            var testee = new CommandHandlerRegistry();

            testee.Register(typeof(ValueCommandHandler), typeof(ValueCommand));

            var handler = testee.GetCommandHandler(new ValueCommand(11));

            handler.Should().BeAssignableTo<ValueCommandHandler>();
        }

        [Fact]
        public void ReturnsNull_WhenAskingForUnregisteredCommandHandler()
        {
            var testee = new CommandHandlerRegistry();

            var handler = testee.GetCommandHandler(new ValueCommand(11));

            handler.Should().BeNull();
        }

        [Fact]
        public void CanGetRegisteredEventHandlers()
        {
            var testee = new EventHandlerRegistry();

            testee.Register(typeof(ValueEventHandler), typeof(ValueEvent));

            var handlers = testee.GetEventHandlers(new ValueEvent(11));

            handlers.Single().Should().BeAssignableTo<ValueEventHandler>();
        }

        [Fact]
        public void ReturnsEmptyList_WhenAskingForUnregisteredEventHandlers()
        {
            var testee = new EventHandlerRegistry();

            var handlers = testee.GetEventHandlers(new ValueEvent(11));

            handlers.Should().BeEmpty();
        }

        private class CommandHandlerRegistry : AbstractHandlerRegistry
        {
            protected override object Resolve(Type handlerType)
            {
                return new ValueCommandHandler();
            }
        }

        private class EventHandlerRegistry : AbstractHandlerRegistry
        {
            protected override object Resolve(Type handlerType)
            {
                return new ValueEventHandler();
            }
        }
    }
}