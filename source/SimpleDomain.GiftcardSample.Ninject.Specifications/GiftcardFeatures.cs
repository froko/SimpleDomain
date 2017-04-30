//-------------------------------------------------------------------------------
// <copyright file="GiftcardFeatures.cs" company="frokonet.ch">
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

namespace GiftcardSample.Ninject
{
    using System;

    using FluentAssertions;

    using GiftcardSample.Commands;
    using GiftcardSample.Domain;
    using GiftcardSample.Events;
    using GiftcardSample.ReadStore;

    using Xbehave;

    public class GiftcardFeatures : BaseFeatures
    {
        private readonly Guid cardId;
        private readonly int cardNumber;
        private readonly decimal balance;
        private readonly DateTime validUntil;

        public GiftcardFeatures()
        {
            this.cardId = Guid.NewGuid();
            this.cardNumber = 12345;
            this.balance = 100m;
            this.validUntil = DateTime.Today.AddDays(30);
        }

        [Scenario]
        public void CreateGiftcard()
        {
            "When a giftcard is created"
                .x(async () => await this.Bus.SendAsync(new CreateGiftcard(this.cardNumber, this.balance, this.validUntil)));

            "The new giftcard is persisted in the overview read store"
                .x(() => this.OverviewQuery.FindAll()
                    .Should().HaveValidReadModel(this.cardNumber, this.balance, this.validUntil, GiftcardStatus.Deactivated));

            "A new giftcard transaction is written"
                .x(() => this.TransactionQuery.Find(this.cardNumber)
                    .Should().HaveValidReadModel(this.cardNumber, "GiftcardCreated", this.balance, 0m));
        }

        [Scenario]
        public void CreateExistingGiftcard()
        {
            Action action = () => { };

            "Given a created giftcard"
                .x(async () => await this.PrepareEventsAsync(this.cardId, new GiftcardCreated(this.cardId, this.cardNumber, this.balance, this.validUntil)));

            "When the same giftcard is created"
                .x(() => action = () => this.Bus.SendAsync(new CreateGiftcard(this.cardNumber, this.balance, this.validUntil)).Wait());

            "A GiftcardException should be thrown"
                .x(() => action.ShouldThrow<GiftcardException>().WithMessage("A giftcard with number 12345 already exists."));
        }

        [Scenario]
        public void ActivateGiftcard()
        {
            "Given a created giftcard"
                .x(async () => await this.PrepareEventsAsync(this.cardId, new GiftcardCreated(this.cardId, this.cardNumber, this.balance, this.validUntil)));

            "When the giftcard is activated"
                .x(async () => await this.Bus.SendAsync(new ActivateGiftcard(this.cardId)));

            "The new giftcard is activated in the overview read store as well"
                .x(() => this.OverviewQuery.FindAll()
                    .Should().HaveValidReadModel(this.cardNumber, this.balance, this.validUntil, GiftcardStatus.Activated));

            "A new giftcard transaction is written"
                .x(() => this.TransactionQuery.Find(this.cardNumber)
                    .Should().HaveValidReadModel(this.cardNumber, "GiftcardActivated", this.balance, 0m));
        }

        [Scenario]
        public void RedeemGiftcard()
        {
            const decimal Amount = 50m;

            "Given a created and activated giftcard"
                .x(async () => await this.PrepareEventsAsync(
                    this.cardId,
                    new GiftcardCreated(this.cardId, this.cardNumber, this.balance, this.validUntil),
                    new GiftcardActivated(this.cardId)));

            "When the giftcard is redeemed"
                .x(async () => await this.Bus.SendAsync(new RedeemGiftcard(this.cardId, Amount)));

            "The balance of the giftcard is decreased by the redemption amount in the read store"
                .x(() => this.OverviewQuery.FindAll()
                    .Should().HaveValidReadModel(this.cardNumber, this.balance - Amount, this.validUntil, GiftcardStatus.Activated));

            "A new giftcard transaction is written"
                .x(() => this.TransactionQuery.Find(this.cardNumber)
                    .Should().HaveValidReadModel(this.cardNumber, "GiftcardRedeemed", this.balance - Amount, Amount));
        }

        [Scenario]
        public void LoadGiftcard()
        {
            const decimal Amount = 50m;

            "Given a created and activated giftcard"
                .x(async () => await this.PrepareEventsAsync(
                    this.cardId,
                    new GiftcardCreated(this.cardId, this.cardNumber, this.balance, this.validUntil),
                    new GiftcardActivated(this.cardId)));

            "When the giftcard is loaded"
                .x(async () => await this.Bus.SendAsync(new LoadGiftcard(this.cardId, Amount)));

            "The balance of the giftcard is increased by the load amount in the read store"
                .x(() => this.OverviewQuery.FindAll()
                    .Should().HaveValidReadModel(this.cardNumber, this.balance + Amount, this.validUntil, GiftcardStatus.Activated));

            "A new giftcard transaction is written"
                .x(() => this.TransactionQuery.Find(this.cardNumber)
                    .Should().HaveValidReadModel(this.cardNumber, "GiftcardLoaded", this.balance + Amount, Amount));
        }
    }
}