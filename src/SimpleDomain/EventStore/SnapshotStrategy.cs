//-------------------------------------------------------------------------------
// <copyright file="SnapshotStrategy.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2019
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

    /// <summary>
    /// The snapshot strategy which defines when to take a snapshot of an aggregate root
    /// </summary>
    public class SnapshotStrategy
    {
        private readonly int threshold;
        private readonly Type aggregateType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotStrategy"/> class.
        /// </summary>
        /// <param name="threshold">The version threshold on which a snapshot is taken</param>
        public SnapshotStrategy(int threshold) : this(threshold, typeof(IEventSourcedAggregateRoot))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotStrategy"/> class.
        /// </summary>
        /// <param name="threshold">The version threshold on which a snapshot is taken</param>
        /// <param name="aggregateType">The type of the aggregate root</param>
        public SnapshotStrategy(int threshold, Type aggregateType)
        {
            this.threshold = threshold;
            this.aggregateType = aggregateType;
        }

        /// <summary>
        /// Returns true if the given type of the aggregate root
        /// is assignable to the internal type and the internal type itself
        /// is not the type of the <see cref="EventSourcedAggregateRoot"/> base class
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
        /// <returns><c>True</c> if the given type of the aggregate root is assignable to this or <c>false</c> if not</returns>
        public bool AppliesToThisAggregateRoot<TAggregateRoot>() where TAggregateRoot : IEventSourcedAggregateRoot
        {
            return this.IsAssignableToMe<TAggregateRoot>() && this.IamNotTheAggregateRootBaseType();
        }

        /// <summary>
        /// Checks if the given aggregate root needs to be snapshotted due to its version
        /// compared with the threshold.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
        /// <param name="aggregateRoot">The aggregate root</param>
        /// <returns><c>True</c> if the aggregate root version is equal or a multiple of the threshold value or <c>false</c> if not</returns>
        public bool NeedsSnapshot<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : IEventSourcedAggregateRoot
        {
            return this.IsAssignableToMe<TAggregateRoot>() && this.IsMultipleOfThreshold(aggregateRoot.Version);
        }

        private bool IsAssignableToMe<TAggregateRoot>() where TAggregateRoot : IEventSourcedAggregateRoot
        {
            return this.aggregateType.IsAssignableFrom(typeof(TAggregateRoot));
        }

        private bool IamNotTheAggregateRootBaseType()
        {
            return this.aggregateType != typeof(IEventSourcedAggregateRoot);
        }

        private bool IsMultipleOfThreshold(int version)
        {
            return (version != 0) && (version % this.threshold == 0);
        }
    }
}