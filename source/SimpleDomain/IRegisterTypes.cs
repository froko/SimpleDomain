//-------------------------------------------------------------------------------
// <copyright file="IRegisterTypes.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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

    /// <summary>
    /// Wrapper interface for registering types in an IoC container
    /// </summary>
    public interface IRegisterTypes
    {
        /// <summary>
        /// Registers an interface-class pair
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface</typeparam>
        /// <typeparam name="TImplementation">The type of the class implementing the given interface</typeparam>
        void Register<TInterface, TImplementation>() where TImplementation : TInterface;

        /// <summary>
        /// Registers an instance for a given interface
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface</typeparam>
        /// <param name="instance">An instance implementing the given interface</param>
        void Register<TInterface>(TInterface instance);

        /// <summary>
        /// Registers an interface-class pair
        /// </summary>
        /// <param name="interfaceType">The type of the interface</param>
        /// <param name="implementationType">The type of the class implementing the given interface</param>
        void Register(Type interfaceType, Type implementationType);
    }
}