//-------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System;

    /// <summary>
    /// Base class for all Entities
    /// </summary>
    /// <typeparam name="T">The type of the entity key</typeparam>
    public abstract class Entity<T> : IEquatable<Entity<T>>
    {
        private int? cachedHashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{T}"/> class.
        /// </summary>
        /// <param name="id">The value of the entity key</param>
        protected Entity(T id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the entity key
        /// </summary>
        public T Id { get; protected set; }

        /// <inheritdoc />
        public bool Equals(Entity<T> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return this.GetType() == other.GetType() && this.Id.Equals(other.Id);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Entity<T>);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (this.cachedHashCode.HasValue)
            {
                return this.cachedHashCode.Value;
            }

            unchecked
            {
                const int StartValue = 17;
                const int Multiplier = 23;

                this.cachedHashCode = (StartValue * Multiplier) + this.Id.GetHashCode();
            }

            return this.cachedHashCode.Value;
        }
    }
}