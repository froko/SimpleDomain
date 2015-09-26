//-------------------------------------------------------------------------------
// <copyright file="EntityTest.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System;

    using FluentAssertions;

    using Xunit;

    public class EntityTest
    {
        [Fact]
        public void CanCreateEntityWithId()
        {
            var id = Guid.NewGuid();

            var testee = new ConcreteEntity(id);

            testee.Should().NotBeNull();
            testee.Id.Should().Be(id);
        }

        [Fact]
        public void CanProveEqualityById()
        {
            var id = Guid.NewGuid();

            var firstEntity = new ConcreteEntity(id);
            var secondEntity = new ConcreteEntity(id);

            firstEntity.Equals(secondEntity).Should().BeTrue();
        }

        [Fact]
        public void CanProveInequalityById()
        {
            var firstEntity = new ConcreteEntity(Guid.NewGuid());
            var secondEntity = new ConcreteEntity(Guid.NewGuid());

            firstEntity.Equals(secondEntity).Should().BeFalse();
        }

        [Fact]
        public void CanProveInequalityWithOtherEntity()
        {
            var id = Guid.NewGuid();

            var firstEntity = new ConcreteEntity(id);
            var secondEntity = new OtherEntity(id);

            firstEntity.Equals(secondEntity).Should().BeFalse();
        }

        [Fact]
        public void CanProveInequalityWithNull()
        {
            var testee = new ConcreteEntity(Guid.NewGuid());

            testee.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void CanGetHashCode()
        {
            var id = Guid.Empty;
            var testee = new ConcreteEntity(id);

            testee.GetHashCode().Should().Be(391); // (17 * 23) + 0
        }

        private class ConcreteEntity : Entity<Guid>
        {
            public ConcreteEntity(Guid id) : base(id)
            {
            }
        }

        private class OtherEntity : Entity<Guid>
        {
            public OtherEntity(Guid id) : base(id)
            {
            }
        }
    }
}