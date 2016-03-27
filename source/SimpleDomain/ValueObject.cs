//-------------------------------------------------------------------------------
// <copyright file="ValueObject.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Base class for all Value Objects
    /// Taken from http://grabbagoft.blogspot.ch/2007/06/generic-value-object-equality.html
    /// </summary>
    /// <typeparam name="T">The type of the value object</typeparam>
    public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T>
    {
        private int? cachedHashCode;

        /// <summary>
        /// Checks two instances of value objects for their equality
        /// based on all their fields and properties
        /// </summary>
        /// <param name="x">The first value object</param>
        /// <param name="y">The second value object</param>
        /// <returns></returns>
        public static bool operator ==(ValueObject<T> x, ValueObject<T> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Checks two instances of value objects for their inequality
        /// based on all their fields and properties
        /// </summary>
        /// <param name="x">The first value object</param>
        /// <param name="y">The second value object</param>
        /// <returns></returns>
        public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var otherInstance = obj as T;

            return this.Equals(otherInstance);
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

                var fields = this.GetFields();
                var hashCode = StartValue;

                foreach (var field in fields)
                {
                    var value = field.GetValue(this);

                    if (value != null)
                    {
                        hashCode = (hashCode * Multiplier) + value.GetHashCode();
                    }
                }

                this.cachedHashCode = hashCode;
            }

            return this.cachedHashCode.Value;
        }

        /// <inheritdoc />
        public virtual bool Equals(T obj)
        {
            if (obj == null)
            {
                return false;
            }

            var thisType = this.GetType();
            var otherType = obj.GetType();

            if (thisType != otherType)
            {
                return false;
            }

            var fields = thisType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var field in fields)
            {
                var valueOfOtherInstance = field.GetValue(obj);
                var valueOfThisInstance = field.GetValue(this);

                if (valueOfOtherInstance == null)
                {
                    if (valueOfThisInstance != null)
                    {
                        return false;
                    }
                }
                else if (!valueOfOtherInstance.Equals(valueOfThisInstance))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            var thisType = this.GetType();
            var fields = new List<FieldInfo>();

            while (thisType != typeof(object))
            {
                if (thisType == null)
                {
                    continue;
                }

                fields.AddRange(thisType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                thisType = thisType.BaseType;
            }

            return fields;
        }
    }
}