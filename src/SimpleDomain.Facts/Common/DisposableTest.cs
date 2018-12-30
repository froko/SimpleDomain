//-------------------------------------------------------------------------------
// <copyright file="DisposableTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Common
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class DisposableTest
    {
        private readonly IDisposable dependency;
        private readonly ConcreteDisposable testee;

        public DisposableTest()
        {
            this.dependency = A.Fake<IDisposable>();
            this.testee = new ConcreteDisposable(this.dependency);
        }

        [Fact]
        public void CanDisposeInstance()
        {
            this.testee.Dispose();
            this.testee.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void CanDisposeNestedDependency()
        {
            this.testee.Dispose();
            A.CallTo(() => this.dependency.Dispose()).MustHaveHappened();
        }

        [Fact]
        public void IsAbleToThrowException_WhenCallingMethodOnDisposedInstance()
        {
            this.testee.Dispose();
            this.testee.Invoking(t => t.DoSomething()).Should().Throw<ObjectDisposedException>();
        }

        internal class ConcreteDisposable : Disposable
        {
            private readonly IDisposable dependency;

            public ConcreteDisposable(IDisposable dependency)
            {
                this.dependency = dependency;
            }

            public void DoSomething()
            {
                this.ThrowExceptionIfDisposed();
            }

            protected override void DisposeManagedResources()
            {
                this.dependency.Dispose();
            }
        }
    }
}