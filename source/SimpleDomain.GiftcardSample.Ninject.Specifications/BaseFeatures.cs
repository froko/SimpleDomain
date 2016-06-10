//-------------------------------------------------------------------------------
// <copyright file="BaseFeatures.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Ninject;

    using GiftcardSample.Domain;
    using GiftcardSample.ReadStore;

    using SimpleDomain;
    using SimpleDomain.Bus;
    using SimpleDomain.EventStore;

    public abstract class BaseFeatures
    {
        private readonly IKernel kernel;

        protected BaseFeatures()
        {
            this.kernel = new StandardKernel(
                new JitneyModule(),
                new EventStoreModule(),
                new ReadStoreModule());

            this.kernel.SignalJitneyToStartWork();
        }

        protected Jitney Bus => this.kernel.Get<Jitney>();

        protected IGiftcardOverviewQuery OverviewQuery => this.kernel.Get<IGiftcardOverviewQuery>();

        protected IGiftcardTransactionQuery TransactionQuery => this.kernel.Get<IGiftcardTransactionQuery>();

        private IEventStore EventStore => this.kernel.Get<IEventStore>();

        protected async Task PrepareEventsAsync(Guid cardId, params IEvent[] events)
        {
            var expectedVersion = events.Length - 1;
            var version = 0;

            var versionableEvents = events.Select(@event => new VersionableEvent(@event).With(version++));

            using (var eventStream = await this.EventStore.OpenStreamAsync<Giftcard>(cardId).ConfigureAwait(false))
            {
                await eventStream
                    .SaveAsync(versionableEvents, expectedVersion, new Dictionary<string, object>())
                    .ConfigureAwait(false);
            }
        }
    }
}