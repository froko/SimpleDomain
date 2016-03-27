//-------------------------------------------------------------------------------
// <copyright file="HeaderKeys.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    /// <summary>
    /// A directory for predefined header keys
    /// </summary>
    public static class HeaderKeys
    {
        public const string Sender = "Sender";
        public const string Recipient = "Recipient";
        public const string TimeSent = "TimeSent";

        public const string MessageType = "MessageType";

        public const string MessageId = "MessageId";
        public const string CorrelationId = "CorrelationId";
    }
}