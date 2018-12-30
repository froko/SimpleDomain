//-------------------------------------------------------------------------------
// <copyright file="VersionableEvent.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
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
    using SimpleDomain.Common;

    /// <summary>
    /// Wrapper around an event which carries a version property
    /// </summary>
    public class VersionableEvent : IEvent, INeedVersion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionableEvent"/> class.
        /// </summary>
        /// <param name="innerEvent">The inner event</param>
        public VersionableEvent(IEvent innerEvent)
        {
            Guard.NotNull(() => innerEvent);
            this.InnerEvent = innerEvent;
        }

        /// <summary>
        /// Gets the inner event
        /// </summary>
        public IEvent InnerEvent { get; private set; }

        /// <summary>
        /// Gets the version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Builder method to set the version
        /// </summary>
        /// <param name="version">The version</param>
        /// <returns>The <see cref="VersionableEvent"/> itself since this is a builder method</returns>
        public VersionableEvent With(int version)
        {
            this.Version = version;
            return this;
        }
    }
}