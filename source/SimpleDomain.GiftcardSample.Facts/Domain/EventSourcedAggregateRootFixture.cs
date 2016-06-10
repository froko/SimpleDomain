//-------------------------------------------------------------------------------
// <copyright file="EventSourcedAggregateRootFixture.cs" company="frokonet.ch">
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
    using System.Linq;

    using FluentAssertions;

    using SimpleDomain;

    /// <summary>
    /// The event sourced AggregateRoot test fixture
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
    public abstract class EventSourcedAggregateRootFixture<TAggregateRoot> where TAggregateRoot : IEventSourcedAggregateRoot
    {
        private Func<TAggregateRoot> create;
        private Action<TAggregateRoot> execute;
        private EventHistory eventHistory;

        private Exception aggregateException;

        /// <summary>
        /// Creates a new instance of <see cref="EventSourcedAggregateRootFixture{TAggregateRoot}"/>
        /// </summary>
        protected EventSourcedAggregateRootFixture()
        {
            this.create = Activator.CreateInstance<TAggregateRoot>;
            this.execute = aggregate => { };
            this.eventHistory = EventHistory.Create();
        }

        /// <summary>
        /// Gets the current aggregate root under test
        /// </summary>
        protected TAggregateRoot Testee { get; private set; }

        /// <summary>
        /// Sets the aggregate root under test into the desired state
        /// by replaying events from history
        /// </summary>
        /// <param name="events">The event history</param>
        protected void LoadFromHistory(params IEvent[] events)
        {
            this.eventHistory = new EventHistory(events);
        }

        /// <summary>
        /// Creates a new aggregate root under test.
        /// You only need to to this if you want to test a no parameterless constructor
        /// </summary>
        /// <param name="func">The function to create the aggregate root</param>
        protected void Create(Func<TAggregateRoot> func)
        {
            this.create = func;
        }

        /// <summary>
        /// Executes behavior of the aggregate root under test
        /// </summary>
        /// <param name="action">The action to perform against the aggregate root</param>
        protected void Execute(Action<TAggregateRoot> action)
        {
            this.execute = action;
        }

        /// <summary>
        /// Checks the occurrence of a specific exception
        /// </summary>
        /// <typeparam name="TException">The type of the exception</typeparam>
        /// <param name="message">The expected exception message</param>
        protected void ShouldFailWith<TException>(string message) where TException : Exception
        {
            this.Run();
            this.aggregateException.Should().NotBeNull().And.BeAssignableTo<TException>();
            this.aggregateException.Message.Should().Be(message);
        }

        /// <summary>
        /// Checks the occurrence of a specific event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        protected void ShouldEmitEventLike<TEvent>() where TEvent : IEvent
        {
            this.Run();

            this.Testee.UncommittedEvents
                .OfType<VersionableEvent>()
                .Select(e => e.InnerEvent)
                .Should().ContainItemsAssignableTo<TEvent>();
        }

        /// <summary>
        /// Checks the occurrence of a specific event
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="expectedEvent">An instance of the expected event in order to compare the properties</param>
        protected void ShouldEmitEventLike<TEvent>(TEvent expectedEvent) where TEvent : class, IEvent
        {
            this.Run();
            this.CheckForUncommittedEventContent(expectedEvent);
        }

        private void Run()
        {
            try
            {
                this.Testee = this.create();
                this.Testee.LoadFromEventHistory(this.eventHistory);

                this.execute(this.Testee);
            }
            catch (Exception exception)
            {
                this.aggregateException = exception;
            }
        }

        private void CheckForUncommittedEventContent<TEvent>(TEvent expectedEvent) where TEvent : class, IEvent
        {
            var actualEvent = this.Testee.UncommittedEvents
                .OfType<VersionableEvent>()
                .Select(e => e.InnerEvent)
                .Last() as TEvent;

            actualEvent?.ShouldBeEquivalentTo(
                expectedEvent,
                options =>
                options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1000)).WhenTypeIs<DateTime>());
        }
    }
}