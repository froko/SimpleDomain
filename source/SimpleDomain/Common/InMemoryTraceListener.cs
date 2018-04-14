//-------------------------------------------------------------------------------
// <copyright file="InMemoryTraceListener.cs" company="frokonet.ch">
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

namespace SimpleDomain.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// A trace listener which holds all traced messages in a list
    /// </summary>
    public class InMemoryTraceListener : TraceListener
    {
        private static readonly Lazy<InMemoryTraceListener> InternalInstance = new Lazy<InMemoryTraceListener>(() => new InMemoryTraceListener());
        private static readonly List<string> InternalLogMessages = new List<string>();

        private static bool locked;

        /// <summary>
        /// Gets a singleton instance of this class
        /// </summary>
        public static TraceListener Instance => InternalInstance.Value;

        /// <summary>
        /// Gets all recorded log messages
        /// </summary>
        public static IEnumerable<string> LogMessages => InternalLogMessages;

        /// <summary>
        /// Clears all recorded log messages
        /// </summary>
        public static void ClearLogMessages()
        {
            WaitForUnlock();
            InternalLogMessages.Clear();
        }

        /// <summary>
        /// Locks the listener
        /// </summary>
        public static void Lock()
        {
            locked = true;
        }

        /// <summary>
        /// Unlocks the listener
        /// </summary>
        public static void Unlock()
        {
            locked = false;
        }

        /// <inheritdoc />
        public override void Write(string message)
        {
            WaitForUnlock();
            InternalLogMessages.Add(message);
        }

        /// <inheritdoc />
        public override void WriteLine(string message)
        {
            WaitForUnlock();
            InternalLogMessages.Add(message);
        }

        private static void WaitForUnlock()
        {
            while (locked)
            {
                Thread.Sleep(100);
            }
        }
    }
}