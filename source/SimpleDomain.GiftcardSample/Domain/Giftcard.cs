//-------------------------------------------------------------------------------
// <copyright file="Giftcard.cs" company="frokonet.ch">
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

    using GiftcardSample.Events;

    using SimpleDomain;

    public class Giftcard : StaticEventSourcedAggregateRoot
    {
        private decimal balance;
        private DateTime validUntil;
        private bool isActivated;

        public Giftcard()
        {
            this.RegisterTransition<GiftcardCreated>(this.Apply);
            this.RegisterTransition<GiftcardActivated>(this.Apply);
            this.RegisterTransition<GiftcardRedeemed>(this.Apply);
            this.RegisterTransition<GiftcardLoaded>(this.Apply);
        }

        public Giftcard(int cardNumber, decimal initialBalance, DateTime validUntil) : this()
        {
            if (initialBalance < 0)
            {
                throw new GiftcardException("Cannot create giftcard with a negative initial balance.");
            }

            if (validUntil < DateTime.Today)
            {
                throw new GiftcardException("Cannot create an already expired giftcard.");
            }

            this.ApplyEvent(new GiftcardCreated(Guid.NewGuid(), cardNumber, initialBalance, validUntil));
        }

        public void Activate()
        {
            if (this.isActivated)
            {
                throw new GiftcardException("Cannot activate an already activated giftcard.");
            }

            if (this.validUntil < DateTime.Today)
            {
                throw new GiftcardException("Cannot activate an expired giftcard.");
            }

            this.ApplyEvent(new GiftcardActivated(this.Id));
        }

        public void Redeem(decimal amount)
        {
            if (!this.isActivated)
            {
                throw new GiftcardException("Cannot redeem an inactive giftcard.");
            }

            if (this.validUntil < DateTime.Today)
            {
                throw new GiftcardException("Cannot redeem an expired giftcard.");
            }

            if (this.balance < amount)
            {
                throw new GiftcardException("Cannot redeem more than the actual balance.");
            }

            this.ApplyEvent(new GiftcardRedeemed(this.Id, amount));
        }

        public void Load(decimal amount)
        {
            if (!this.isActivated)
            {
                throw new GiftcardException("Cannot load an inactive giftcard.");
            }

            if (this.validUntil < DateTime.Today)
            {
                throw new GiftcardException("Cannot load an expired giftcard.");
            }

            this.ApplyEvent(new GiftcardLoaded(this.Id, amount));
        }

        private void Apply(GiftcardCreated @event)
        {
            this.Id = @event.CardId;
            this.balance = @event.InitialBalance;
            this.validUntil = @event.ValidUntil;
            this.isActivated = false;
        }

        private void Apply(GiftcardActivated @event)
        {
            this.isActivated = true;
        }

        private void Apply(GiftcardRedeemed @event)
        {
            this.balance -= @event.Amount;
        }

        private void Apply(GiftcardLoaded @event)
        {
            this.balance += @event.Amount;
        }
    }
}