//-------------------------------------------------------------------------------
// <copyright file="GiftcardTest.cs" company="frokonet.ch">
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

namespace GiftcardSample.Domain
{
    using System;

    using FluentAssertions;

    using GiftcardSample.Events;

    using Xunit;

    public class GiftcardTest : EventSourcedAggregateRootFixture<Giftcard>
    {
        private readonly Guid cardId;

        public GiftcardTest()
        {
            this.cardId = Guid.NewGuid();
        }

        [Fact]
        public void CanCreateInstanceWithDefaultConstructor()
        {
            new Giftcard().Should().NotBeNull();
        }

        [Fact]
        public void CanCreateGiftcardWithNumberAndBalanceAndDate()
        {
            this.Create(() => new Giftcard(12345, 100m, DateTime.Today.AddDays(1)));

            this.ShouldEmitEventLike<GiftcardCreated>();
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateGiftcardWithNegativeBalance()
        {
            this.Create(() => new Giftcard(12345, -13m, DateTime.Today.AddDays(1)));

            this.ShouldFailWith<GiftcardException>("Cannot create giftcard with a negative initial balance.");
        }

        [Fact]
        public void ThrowsException_WhenTryingToCreateAnExpiredGiftcard()
        {
            this.Create(() => new Giftcard(12345, 100m, DateTime.Today.AddDays(-1)));

            this.ShouldFailWith<GiftcardException>("Cannot create an already expired giftcard.");
        }

        [Fact]
        public void CanActivateAnInactiveGiftcard()
        {
            this.LoadFromHistory(new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(1)));

            this.Execute(g => g.Activate());

            this.ShouldEmitEventLike(new GiftcardActivated(this.cardId));
        }

        [Fact]
        public void ThrowsException_WhenTryingToActivateAnAlreadyActivatedGiftcard()
        {
            this.LoadFromHistory(
                new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(1)),
                new GiftcardActivated(this.cardId));

            this.Execute(g => g.Activate());

            this.ShouldFailWith<GiftcardException>("Cannot activate an already activated giftcard.");
        }

        [Fact]
        public void ThrowsException_WhenTryingToActivateAnExpiredGiftcard()
        {
            this.LoadFromHistory(new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(-1)));

            this.Execute(g => g.Activate());

            this.ShouldFailWith<GiftcardException>("Cannot activate an expired giftcard.");
        }

        [Fact]
        public void CanRedeemAnActiveGiftcardWithEnoughBalance()
        {
            this.LoadFromHistory(
                new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(1)),
                new GiftcardActivated(this.cardId));

            this.Execute(g => g.Redeem(20m));

            this.ShouldEmitEventLike(new GiftcardRedeemed(this.cardId, 20m));
        }

        [Fact]
        public void ThrowsException_WhenTryingToRedeemAnInactiveGiftcard()
        {
            this.LoadFromHistory(new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(1)));

            this.Execute(g => g.Redeem(20m));

            this.ShouldFailWith<GiftcardException>("Cannot redeem an inactive giftcard.");
        }

        [Fact]
        public void ThrowsException_WhenTryingToRedeemAnExpiredGiftcard()
        {
            this.LoadFromHistory(
                new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(-1)),
                new GiftcardActivated(this.cardId));

            this.Execute(g => g.Redeem(20m));

            this.ShouldFailWith<GiftcardException>("Cannot redeem an expired giftcard.");
        }

        [Fact]
        public void ThrowsException_WhenTryingToRedeemMoreThanTheActualBalance()
        {
            this.LoadFromHistory(
                new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(1)),
                new GiftcardActivated(this.cardId));

            this.Execute(g => g.Redeem(125m));

            this.ShouldFailWith<GiftcardException>("Cannot redeem more than the actual balance.");
        }

        [Fact]
        public void CanLoadAnActiveGiftcard()
        {
            this.LoadFromHistory(
                new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(1)),
                new GiftcardActivated(this.cardId));

            this.Execute(g => g.Load(50m));

            this.ShouldEmitEventLike(new GiftcardLoaded(this.cardId, 50m));
        }

        [Fact]
        public void ThrowsException_WhenTryingToLoadAnInactiveGiftcard()
        {
            this.LoadFromHistory(new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(1)));

            this.Execute(g => g.Load(50m));

            this.ShouldFailWith<GiftcardException>("Cannot load an inactive giftcard.");
        }

        [Fact]
        public void ThrowsException_WhenTryingToLoadAnExpiredGiftcard()
        {
            this.LoadFromHistory(
                new GiftcardCreated(this.cardId, 12345, 100m, DateTime.Today.AddDays(-1)),
                new GiftcardActivated(this.cardId));

            this.Execute(g => g.Load(50m));

            this.ShouldFailWith<GiftcardException>("Cannot load an expired giftcard.");
        }
    }
}