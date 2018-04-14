//-------------------------------------------------------------------------------
// <copyright file="JitneyKernelExtensions.cs" company="frokonet.ch">
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
    using System.Threading.Tasks;

    using Ninject;

    /// <summary>
    /// Kernel extensions for the Jitney bus
    /// </summary>
    public static class JitneyKernelExtensions
    {
        /// <summary>
        /// Tells the Jitney bus to start receiving messages
        /// </summary>
        /// <param name="kernel">The Ninject kernel</param>
        public static void SignalJitneyToStartWork(this IKernel kernel)
        {
            try
            {
                kernel.Get<Jitney>().StartAsync().Wait();
            }
            catch (AggregateException aggregateException)
            {
                throw aggregateException.Flatten();
            }
        }

        /// <summary>
        /// Tells the Jitney bus to start receiving messages
        /// </summary>
        /// <param name="kernel">The Ninject kernel</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        public static Task SignalJitneyToStartWorkAsync(this IKernel kernel)
        {
            return kernel.Get<Jitney>().StartAsync();
        }

        /// <summary>
        /// Tells the Jitney bus to stop receiving messages
        /// </summary>
        /// <param name="kernel">The Ninject kernel</param>
        public static void SignalJitneyToStopWork(this IKernel kernel)
        {
            try
            {
                kernel.Get<Jitney>().StopAsync().Wait();
            }
            catch (AggregateException aggregateException)
            {
                throw aggregateException.Flatten();
            }
        }

        /// <summary>
        /// Tells the Jitney bus to stop receiving messages
        /// </summary>
        /// <param name="kernel">The Ninject kernel</param>
        /// <returns>A <see cref="Task"/> since this is an async method</returns>
        public static Task SignalJitneyToStopWorkAsync(this IKernel kernel)
        {
            return kernel.Get<Jitney>().StopAsync();
        }
    }
}