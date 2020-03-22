//-------------------------------------------------------------------------------
// <copyright file="EndpointAddress.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;

    using Newtonsoft.Json;

    using SimpleDomain.Common;

    /// <summary>
    /// Represents an endpoint address
    /// </summary>
    public class EndpointAddress : ValueObject<EndpointAddress>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointAddress"/> class.
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        public EndpointAddress(string queueName)
        {
            Guard.NotNullOrEmpty(() => queueName);

            this.QueueName = queueName;
            this.MachineName = Environment.MachineName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointAddress"/> class.
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="machineName">The name of the remote machine</param>
        [JsonConstructor]
        public EndpointAddress(string queueName, string machineName)
        {
            Guard.NotNullOrEmpty(() => queueName);
            Guard.NotNullOrEmpty(() => machineName);

            this.QueueName = queueName;
            this.MachineName = machineName;
        }

        /// <summary>
        /// Gets the name of the queue
        /// </summary>
        public string QueueName { get; }

        /// <summary>
        /// Gets the name of the machine where the queue resides
        /// </summary>
        public string MachineName { get; }

        /// <summary>
        /// Gets a value indicating whether this endpoint is local to the current machine
        /// </summary>
        [JsonIgnore]
        public bool IsLocal => this.MachineName == Environment.MachineName;

        /// <summary>
        /// Parses a given string to an <see cref="EndpointAddress"/>
        /// </summary>
        /// <param name="address">The address in a form like <example>myqueue@mymachine</example></param>
        /// <returns>A new instance of an <see cref="EndpointAddress"/></returns>
        public static EndpointAddress Parse(string address)
        {
            Guard.NotNullOrEmpty(() => address);
            Guard.IsTrue(address.Contains("@"), "address must contain an @ in order to split the queue and machine name");

            var queueName = address.Split('@')[0];
            var machineName = address.Split('@')[1];

            return new EndpointAddress(queueName, machineName);
        }

        /// <summary>
        /// Creates a new sub scope address based on the given address
        /// </summary>
        /// <param name="subScope">The name of the sub scope</param>
        /// <returns>A new instance of an <see cref="EndpointAddress"/></returns>
        public EndpointAddress CreateSubScopeAddress(string subScope)
        {
            Guard.NotNullOrEmpty(() => subScope);

            var subQueueName = $"{this.QueueName}.{subScope}";

            return new EndpointAddress(subQueueName, this.MachineName);
        }

        /// <summary>
        /// Creates a new retry sub scope address based on the given address
        /// </summary>
        /// <returns>A new instance of <see cref="EndpointAddress"/></returns>
        public EndpointAddress CreateRetrySubScopeAddress()
        {
            return this.CreateSubScopeAddress("retries");
        }

        /// <summary>
        /// Creates a new error sub scope address based on the given address
        /// </summary>
        /// <returns>A new instance of an <see cref="EndpointAddress"/></returns>
        public EndpointAddress CreateErrorSubScopeAddress()
        {
            return this.CreateSubScopeAddress("error");
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.QueueName}@{this.MachineName}";
        }

        /// <inheritdoc />
        public override bool Equals(EndpointAddress other)
        {
            return other != null && this.QueueName == other.QueueName && this.MachineName == other.MachineName;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var otherInstance = obj as EndpointAddress;

            return this.Equals(otherInstance);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                const int StartValue = 17;
                const int Multiplier = 23;

                var hashCode = StartValue;

                hashCode = (hashCode * Multiplier) + this.QueueName.GetHashCode();
                hashCode = (hashCode * Multiplier) + this.MachineName.GetHashCode();

                return hashCode;
            }
        }
    }
}