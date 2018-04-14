//-------------------------------------------------------------------------------
// <copyright file="PipelineContext.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Pipeline
{
    /// <summary>
    /// The base class for all pipeline contexts
    /// </summary>
    public abstract class PipelineContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineContext"/> class.
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHavePipelineConfiguration"/></param>
        protected PipelineContext(IHavePipelineConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the pipeline configuration
        /// </summary>
        public virtual IHavePipelineConfiguration Configuration { get; private set; }
    }
}