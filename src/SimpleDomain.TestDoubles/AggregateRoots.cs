//-------------------------------------------------------------------------------
// <copyright file="AggregateRoots.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
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

namespace SimpleDomain.TestDoubles
{
    using System;

    public class MyDynamicEventSourcedAggregateRoot : DynamicEventSourcedAggregateRoot
    {
        public MyDynamicEventSourcedAggregateRoot()
        {
        }

        public MyDynamicEventSourcedAggregateRoot(Guid aggregateId)
        {
            this.Id = aggregateId;
        }

        public int Value { get; private set; }

        public override ISnapshot CreateSnapshot()
        {
            return new MySnapshot(this.Value).WithVersion(this.Version);
        }

        public override void LoadFromSnapshot(ISnapshot snapshot)
        {
            var concreteAggregateRootSnapshot = snapshot as MySnapshot;

            if (concreteAggregateRootSnapshot == null)
            {
                return;
            }

            this.Version = concreteAggregateRootSnapshot.Version;
            this.Value = concreteAggregateRootSnapshot.Value;
        }

        public new void ApplyEvent(IEvent @event)
        {
            base.ApplyEvent(@event);
        }

        public void ChangeValue(int value)
        {
            base.ApplyEvent(new ValueEvent(value));
        }

        // ReSharper disable once UnusedMember.Local
        private void Apply(ValueEvent @event)
        {
            this.Value = @event.Value;
        }
    }

    public class OtherDynamicEventSourcedAggregateRoot : DynamicEventSourcedAggregateRoot
    {
        public OtherDynamicEventSourcedAggregateRoot WithVersion(int version)
        {
            this.Version = version;
            return this;
        }
    }

    public class MyStaticEventSourcedAggregateRoot : StaticEventSourcedAggregateRoot
    {
        public MyStaticEventSourcedAggregateRoot()
        {
            this.RegisterTransition<ValueEvent>(this.Apply);
        }

        public MyStaticEventSourcedAggregateRoot(Guid aggregateId)
        {
            this.Id = aggregateId;
        }

        public int Value { get; private set; }

        public new void ApplyEvent(IEvent @event)
        {
            base.ApplyEvent(@event);
        }

        private void Apply(ValueEvent @event)
        {
            this.Value = @event.Value;
        }
    }

    public class MyStateBasedAggregateRoot : StateBasedAggregateRoot<MyState>
    {
        public MyStateBasedAggregateRoot()
        {
            this.State = new MyState();
        }

        public MyStateBasedAggregateRoot(MyState state) : base(state)
        {
        }

        public int Value
        {
            get { return this.State.Value; }
        }

        public void UpdateValue(int value)
        {
            this.State.Value = value;
            this.ApplyEvent(new ValueEvent(value));
        }
    }

    public class MyState
    {
        public int Value { get; set; }
    }
}