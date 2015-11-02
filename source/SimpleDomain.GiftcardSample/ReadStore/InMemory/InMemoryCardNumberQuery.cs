//-------------------------------------------------------------------------------
// <copyright file="InMemoryCardNumberQuery.cs" company="frokonet.ch">
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
    using System.Linq;

    public class InMemoryCardNumberQuery : ICardNumberQuery
    {
        private readonly IReadStore readStore;

        public InMemoryCardNumberQuery(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public bool IsAlreadyInUse(int cardNumber)
        {
            return this.readStore.CardNumbers.Any(n => n == cardNumber);
        }
    }
}