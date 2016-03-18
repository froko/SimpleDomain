//-------------------------------------------------------------------------------
// <copyright file="FluentTestingExtensions.cs" company="frokonet.ch">
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

namespace GiftcardSample.Ninject
{
    using System;

    using FluentAssertions.Collections;

    using GiftcardSample.ReadStore;

    public static class FluentTestingExtensions
    {
        public static void HaveValidReadModel(
            this GenericCollectionAssertions<GiftcardOverview> readStore,
            int cardNumber,
            decimal balance,
            DateTime validUntil,
            GiftcardStatus giftcardStatus)
        {
            readStore.Contain(giftcardOverview => 
                giftcardOverview.CardNumber == cardNumber &&
                giftcardOverview.CurrentBalance == balance &&
                giftcardOverview.ValidUntil == validUntil &&
                giftcardOverview.Status == giftcardStatus);
        }

        public static void HaveValidReadModel(
            this GenericCollectionAssertions<GiftcardTransaction> readStore,
            int cardNumber,
            string eventText,
            decimal balance,
            decimal amount)
        {
            readStore.Contain(giftcardTransaction =>
                giftcardTransaction.CardNumber == cardNumber &&
                giftcardTransaction.ValutaDate == DateTime.Today &&
                giftcardTransaction.Event == eventText &&
                giftcardTransaction.Balance == balance &&
                giftcardTransaction.Amount == amount);
        }
    }
}