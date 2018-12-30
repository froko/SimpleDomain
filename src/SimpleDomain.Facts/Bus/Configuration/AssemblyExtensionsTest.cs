//-------------------------------------------------------------------------------
// <copyright file="AssemblyExtensionsTest.cs" company="frokonet.ch">
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
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class AssemblyExtensionsTest
    {
        [Fact]
        public void CanFindAsyncHandlerTypesInAssembly()
        {
            var assembly = typeof(ValueCommand).Assembly;
            var asyncHandlerTypes = assembly.GetAsyncHandlerTypes().ToList();

            asyncHandlerTypes.Should().Contain(typeof(ValueCommandHandler));
            asyncHandlerTypes.Should().Contain(typeof(ValueEventHandler));
        }

        [Fact]
        public void ReturnsEmptyListIfNoAsyncHandlerTypeWasFound()
        {
            var assembly = typeof(ICommand).Assembly;
            var asyncHandlerTypes = assembly.GetAsyncHandlerTypes();

            asyncHandlerTypes.Should().BeEmpty();
        }
    }
}