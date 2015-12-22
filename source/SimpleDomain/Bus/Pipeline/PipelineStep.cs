//-------------------------------------------------------------------------------
// <copyright file="PipelineStep.cs" company="frokonet.ch">
//   Copyright (c) 2015
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

namespace SimpleDomain.Bus.Pipeline
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The base class for all pipeline steps
    /// </summary>
    /// <typeparam name="TContext">The type of the pipeline context</typeparam>
    public abstract class PipelineStep<TContext> where TContext : PipelineContext
    {
        /// <summary>
        /// Gets the name of this pipeline step
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Invokes the pipeline step
        /// </summary>
        /// <param name="context">The pipeline context</param>
        /// <param name="next">The next action in the pipeline</param>
        public abstract Task InvokeAsync(TContext context, Func<Task> next);
    }
}