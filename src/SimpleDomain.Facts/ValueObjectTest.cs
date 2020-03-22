//-------------------------------------------------------------------------------
// <copyright file="ValueObjectTest.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Taken from http://grabbagoft.blogspot.ch/2007/06/generic-value-object-equality.html
    /// with minor changes.
    /// </summary>
    public class ValueObjectTest
    {
        [Fact]
        public void AddressEqualsWorksWithIdenticalAddresses()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");

            address.Equals(address2).Should().BeTrue();
        }

        [Fact]
        public void AddressEqualsWorksWithNonIdenticalAddresses()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address2", "Austin", "TX");

            address.Equals(address2).Should().BeFalse();
        }

        [Fact]
        public void AddressEqualsWorksWithNulls()
        {
            var address = new Address(null, "Austin", "TX");
            var address2 = new Address("Address2", "Austin", "TX");

            address.Equals(address2).Should().BeFalse();
        }

        [Fact]
        public void AddressEqualsWorksWithNullsOnOtherObject()
        {
            var address = new Address("Address2", "Austin", "TX");
            var address2 = new Address("Address2", null, "TX");

            address.Equals(address2).Should().BeFalse();
        }

        [Fact]
        public void AddressEqualsIsReflexive()
        {
            var address = new Address("Address1", "Austin", "TX");

            address.Equals(address).Should().BeTrue();
        }

        [Fact]
        public void AddressEqualsIsSymmetric()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address2", "Austin", "TX");

            address.Equals(address2).Should().BeFalse();
            address2.Equals(address).Should().BeFalse();
        }

        [Fact]
        public void AddressEqualsIsTransitive()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");
            var address3 = new Address("Address1", "Austin", "TX");

            address.Equals(address2).Should().BeTrue();
            address2.Equals(address3).Should().BeTrue();
            address.Equals(address3).Should().BeTrue();
        }

        [Fact]
        public void AddressOperatorsWork()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");
            var address3 = new Address("Address2", "Austin", "TX");

            (address == address2).Should().BeTrue();
            (address2 != address3).Should().BeTrue();
        }

        [Fact]
        public void DerivedTypesBehaveCorrectly()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            address.Equals(address2).Should().BeFalse();
            (address == address2).Should().BeFalse();
        }

        [Fact]
        public void EqualValueObjectsHaveSameHashCode()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");

            address.GetHashCode().Should().Be(address2.GetHashCode());
        }

        [Fact]
        public void TransposedValuesGiveDifferentHashCodes()
        {
            var address = new Address(null, "Austin", "TX");
            var address2 = new Address("TX", "Austin", null);

            address.GetHashCode().Should().NotBe(address2.GetHashCode());
        }

        [Fact]
        public void UnequalValueObjectsHaveDifferentHashCodes()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address2", "Austin", "TX");

            address.GetHashCode().Should().NotBe(address2.GetHashCode());
        }

        [Fact]
        public void TransposedValuesOfFieldNamesGivesDifferentHashCodes()
        {
            var address = new Address("_city", null, null);
            var address2 = new Address(null, "_address1", null);

            address.GetHashCode().Should().NotBe(address2.GetHashCode());
        }

        [Fact]
        public void DerivedTypesHashCodesBehaveCorrectly()
        {
            var address = new ExpandedAddress("Address99999", "Apt 123", "New Orleans", "LA");
            var address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            address.GetHashCode().Should().NotBe(address2.GetHashCode());
        }

        internal class Address : ValueObject<Address>
        {
            public readonly string Address1;
            public readonly string City;
            public readonly string State;

            public Address(string address1, string city, string state)
            {
                this.Address1 = address1;
                this.City = city;
                this.State = state;
            }
        }

        internal class ExpandedAddress : Address
        {
            public readonly string Address2;

            public ExpandedAddress(string address1, string address2, string city, string state)
                : base(address1, city, state)
            {
                this.Address2 = address2;
            }
        }
    }
}