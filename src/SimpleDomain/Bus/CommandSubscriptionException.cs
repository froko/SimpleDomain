//-------------------------------------------------------------------------------
// <copyright file="CommandSubscriptionException.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;

    /// <summary>
    /// The exception that is thrown when trying to add more than one handler per command
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    [Serializable]
    public class CommandSubscriptionException<TCommand> : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSubscriptionException{TCommand}"/> class.
        /// </summary>
        public CommandSubscriptionException()
            : base(string.Format(ExceptionMessages.CannotSubscribeMoreThanOneHandlerForCommand, typeof(TCommand).FullName))
        {
        }
    }
}