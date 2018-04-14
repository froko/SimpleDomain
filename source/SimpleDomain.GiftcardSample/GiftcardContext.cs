//-------------------------------------------------------------------------------
// <copyright file="GiftcardContext.cs" company="frokonet.ch">
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

namespace GiftcardSample
{
    using System.Threading.Tasks;

    using GiftcardSample.Commands;
    using GiftcardSample.Domain;
    using GiftcardSample.Events;
    using GiftcardSample.ReadStore;
    using GiftcardSample.ReadStore.InMemory;

    using SimpleDomain;
    using SimpleDomain.Bus;

    public class GiftcardContext : IBoundedContext
    {
        private readonly ICardNumberQuery cardNumberQuery;
        private readonly InMemoryCardNumberEventHandler cardNumberEventHandler;
        private readonly InMemoryGiftcardOverviewEventHandler giftcardOverviewEventHandler;
        private readonly InMemoryGiftcardTransactionEventHandler giftcardTransactionEventHandler;

        public GiftcardContext(IReadStore readStore)
        {
            this.cardNumberQuery = new InMemoryCardNumberQuery(readStore);
            this.cardNumberEventHandler = new InMemoryCardNumberEventHandler(readStore);
            this.giftcardOverviewEventHandler = new InMemoryGiftcardOverviewEventHandler(readStore);
            this.giftcardTransactionEventHandler = new InMemoryGiftcardTransactionEventHandler(readStore);
        }

        public string Name => "Giftcards";

        private IEventSourcedRepository Repository { get; set; }

        public void Configure(ISubscribeMessageHandlers configuration, IDeliverMessages bus, IEventSourcedRepository repository)
        {
            this.Repository = repository;

            configuration.SubscribeCommandHandler<CreateGiftcard>(this.HandleAsync);
            configuration.SubscribeCommandHandler<ActivateGiftcard>(this.HandleAsync);
            configuration.SubscribeCommandHandler<RedeemGiftcard>(this.HandleAsync);
            configuration.SubscribeCommandHandler<LoadGiftcard>(this.HandleAsync);

            configuration.SubscribeEventHandler<GiftcardCreated>(this.cardNumberEventHandler.HandleAsync);

            configuration.SubscribeEventHandler<GiftcardCreated>(this.giftcardOverviewEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardActivated>(this.giftcardOverviewEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardRedeemed>(this.giftcardOverviewEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardLoaded>(this.giftcardOverviewEventHandler.HandleAsync);

            configuration.SubscribeEventHandler<GiftcardCreated>(this.giftcardTransactionEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardActivated>(this.giftcardTransactionEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardRedeemed>(this.giftcardTransactionEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardLoaded>(this.giftcardTransactionEventHandler.HandleAsync);
        }

        private async Task HandleAsync(CreateGiftcard command)
        {
            if (this.cardNumberQuery.IsAlreadyInUse(command.CardNumber))
            {
                throw new GiftcardException($"A giftcard with number {command.CardNumber} already exists.");
            }

            var giftcard = new Giftcard(
                command.CardNumber,
                command.InitialBalance,
                command.ValidUntil);

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        private async Task HandleAsync(ActivateGiftcard command)
        {
            var giftcard = await this.Repository
                .GetByIdAsync<Giftcard>(command.CardId)
                .ConfigureAwait(false);

            giftcard.Activate();

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        private async Task HandleAsync(RedeemGiftcard command)
        {
            var giftcard = await this.Repository
                .GetByIdAsync<Giftcard>(command.CardId)
                .ConfigureAwait(false);

            giftcard.Redeem(command.Amount);

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        private async Task HandleAsync(LoadGiftcard command)
        {
            var giftcard = await this.Repository
                .GetByIdAsync<Giftcard>(command.CardId)
                .ConfigureAwait(false);

            giftcard.Load(command.Amount);

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }
    }
}