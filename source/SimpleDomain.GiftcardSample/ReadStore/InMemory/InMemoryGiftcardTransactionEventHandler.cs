//-------------------------------------------------------------------------------
// <copyright file="InMemoryGiftcardTransactionEventHandler.cs" company="frokonet.ch">
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

namespace GiftcardSample.ReadStore.InMemory
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using GiftcardSample.Events;

    using SimpleDomain;

    public class InMemoryGiftcardTransactionEventHandler :
        IHandleAsync<GiftcardCreated>,
        IHandleAsync<GiftcardActivated>,
        IHandleAsync<GiftcardRedeemed>,
        IHandleAsync<GiftcardLoaded>
    {
        private readonly IReadStore readStore;

        public InMemoryGiftcardTransactionEventHandler(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public Task HandleAsync(GiftcardCreated message)
        {
            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = message.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = message.InitialBalance,
                Amount = 0,
                Revision = 0
            });

            return Task.FromResult(0);
        }

        public Task HandleAsync(GiftcardActivated message)
        {
            var previousTransaction = this.readStore.GiftcardTransactions.Last(g => g.CardId == message.CardId);
            var newRevision = previousTransaction.Revision;
            newRevision++;

            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = previousTransaction.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = previousTransaction.Balance,
                Amount = previousTransaction.Amount,
                Revision = newRevision
            });

            return Task.FromResult(0);
        }

        public Task HandleAsync(GiftcardRedeemed message)
        {
            var previousTransaction = this.readStore.GiftcardTransactions.Last(g => g.CardId == message.CardId);
            var newRevision = previousTransaction.Revision;
            newRevision++;

            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = previousTransaction.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = previousTransaction.Balance - message.Amount,
                Amount = message.Amount,
                Revision = newRevision
            });

            return Task.FromResult(0);
        }

        public Task HandleAsync(GiftcardLoaded message)
        {
            var previousTransaction = this.readStore.GiftcardTransactions.Last(g => g.CardId == message.CardId);
            var newRevision = previousTransaction.Revision;
            newRevision++;

            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = previousTransaction.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = previousTransaction.Balance + message.Amount,
                Amount = message.Amount,
                Revision = newRevision
            });

            return Task.FromResult(0);
        }
    }
}