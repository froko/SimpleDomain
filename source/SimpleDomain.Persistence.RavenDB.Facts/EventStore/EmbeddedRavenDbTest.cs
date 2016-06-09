//-------------------------------------------------------------------------------
// <copyright file="EmbeddedRavenDbTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore
{
    using System;

    using Raven.Client;
    using Raven.Client.Embedded;
    using Raven.Client.Listeners;

    public abstract class EmbeddedRavenDbTest : IDisposable
    {
        protected EmbeddedRavenDbTest()
        {
            this.DocumentStore = new EmbeddableDocumentStore { RunInMemory = true }; // new DocumentStore { ConnectionStringName = "EventStore" };
            this.DocumentStore.Initialize();
            this.DocumentStore.Listeners.RegisterListener(new NoStaleQueriesListener());
        }

        protected IDocumentStore DocumentStore { get; }

        public void Dispose()
        {
            this.DocumentStore.Dispose();
        }
    }

    // Taken from http://stackoverflow.com/questions/10316721/ravendb-force-indexes-to-wait-until-not-stale-whilst-unit-testing
    public class NoStaleQueriesListener : IDocumentQueryListener
    {
        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queryCustomization.WaitForNonStaleResults();
        }
    }
}