//-------------------------------------------------------------------------------
// <copyright file="GiftcardCommandHandler.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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
    using System.Threading.Tasks;

    using GiftcardSample.Commands;
    using GiftcardSample.Domain;
    using GiftcardSample.ReadStore;

    using SimpleDomain;

    public class GiftcardCommandHandler : 
        IHandleAsync<CreateGiftcard>, 
        IHandleAsync<ActivateGiftcard>, 
        IHandleAsync<RedeemGiftcard>, 
        IHandleAsync<LoadGiftcard>
    {
        private readonly IEventSourcedRepository repository;
        private readonly ICardNumberQuery cardNumberQuery;

        public GiftcardCommandHandler(IEventSourcedRepository repository, ICardNumberQuery cardNumberQuery)
        {
            this.repository = repository;
            this.cardNumberQuery = cardNumberQuery;
        }

        public async Task HandleAsync(CreateGiftcard message)
        {
            if (this.cardNumberQuery.IsAlreadyInUse(message.CardNumber))
            {
                throw new GiftcardException($"A giftcard with number {message.CardNumber} already exists.");
            }

            var giftcard = new Giftcard(
                message.CardNumber,
                message.InitialBalance,
                message.ValidUntil);

            await this.repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        public async Task HandleAsync(ActivateGiftcard message)
        {
            var giftcard = await this.repository
                .GetByIdAsync<Giftcard>(message.CardId)
                .ConfigureAwait(false);

            giftcard.Activate();

            await this.repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        public async Task HandleAsync(RedeemGiftcard message)
        {
            var giftcard = await this.repository
                .GetByIdAsync<Giftcard>(message.CardId)
                .ConfigureAwait(false);

            giftcard.Redeem(message.Amount);

            await this.repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        public async Task HandleAsync(LoadGiftcard message)
        {
            var giftcard = await this.repository
                .GetByIdAsync<Giftcard>(message.CardId)
                .ConfigureAwait(false);

            giftcard.Load(message.Amount);

            await this.repository.SaveAsync(giftcard).ConfigureAwait(false);
        }
    }
}