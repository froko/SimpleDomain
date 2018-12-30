//-------------------------------------------------------------------------------
// <copyright file="FeatureSelector.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The feature selector
    /// </summary>
    public class FeatureSelector : IFeatureSelector
    {
        private readonly IReadOnlyCollection<AbstractFeature> features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureSelector"/> class.
        /// </summary>
        /// <param name="features">The registered features</param>
        public FeatureSelector(IReadOnlyCollection<AbstractFeature> features)
        {
            this.features = features;
        }

        /// <inheritdoc />
        public T Select<T>() where T : AbstractFeature
        {
            var feature = this.features.SingleOrDefault(f => f is T);

            if (feature is null)
            {
                throw new FeatureNotFoundException<T>();
            }

            return (T)feature;
        }
    }
}