//-------------------------------------------------------------------------------
// <copyright file="HandlerRegistryTest.cs" company="frokonet.ch">
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

    using FluentAssertions;

    using Ninject;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class HandlerRegistryTest : IDisposable
    {
        private readonly IKernel kernel;
        private readonly HandlerRegistry testee;

        public HandlerRegistryTest()
        {
            this.kernel = new StandardKernel();
            this.testee = new HandlerRegistry(this.kernel);
        }

        [Fact]
        public void CanResolvePreviouslyRegisteredCommandHandler()
        {
            this.testee.Register(typeof(ValueCommandHandler), typeof(ValueCommand));

            this.testee.GetCommandHandler(new ValueCommand(11)).Should().NotBeNull();
        }

        [Fact]
        public void CanResolvePreviouslyRegisteredEventHandler()
        {
            this.testee.Register(typeof(ValueEventHandler), typeof(ValueEvent));

            this.testee.GetEventHandlers(new ValueEvent(11)).Should().HaveCount(1);
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }
}