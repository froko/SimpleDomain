//-------------------------------------------------------------------------------
// <copyright file="InMemoryGiftcardOverviewEventHandler.cs" company="frokonet.ch">
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

namespace GiftcardSample.ReadStore.InMemory
{
    using System.Linq;
    using System.Threading.Tasks;

    using GiftcardSample.Events;

    using SimpleDomain;

    public class InMemoryGiftcardOverviewEventHandler :
        IHandleAsync<GiftcardCreated>,
        IHandleAsync<GiftcardActivated>,
        IHandleAsync<GiftcardRedeemed>,
        IHandleAsync<GiftcardLoaded>
    {
        private readonly IReadStore readStore;

        public InMemoryGiftcardOverviewEventHandler(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public Task HandleAsync(GiftcardCreated message)
        {
            return Task.Run(() => this.Handle(message));
        }

        public Task HandleAsync(GiftcardActivated message)
        {
            return Task.Run(() => this.Handle(message));
        }

        public Task HandleAsync(GiftcardRedeemed message)
        {
            return Task.Run(() => this.Handle(message));
        }

        public Task HandleAsync(GiftcardLoaded message)
        {
            return Task.Run(() => this.Handle(message));
        }

        private void Handle(GiftcardCreated message)
        {
            this.readStore.GiftcardOverviews.Add(new GiftcardOverview
            {
                CardId = message.CardId,
                CardNumber = message.CardNumber,
                CurrentBalance = message.InitialBalance,
                ValidUntil = message.ValidUntil,
                Status = GiftcardStatus.Deactivated
            });
        }

        private void Handle(GiftcardActivated message)
        {
            var giftcard = this.readStore.GiftcardOverviews.Single(g => g.CardId == message.CardId);
            giftcard.Status = GiftcardStatus.Activated;
        }

        private void Handle(GiftcardRedeemed message)
        {
            var giftcard = this.readStore.GiftcardOverviews.Single(g => g.CardId == message.CardId);
            giftcard.CurrentBalance -= message.Amount;
        }

        private void Handle(GiftcardLoaded message)
        {
            var giftcard = this.readStore.GiftcardOverviews.Single(g => g.CardId == message.CardId);
            giftcard.CurrentBalance += message.Amount;
        }
    }
}