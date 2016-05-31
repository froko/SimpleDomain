//-------------------------------------------------------------------------------
// <copyright file="JitneyModule.cs" company="frokonet.ch">
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

namespace GiftcardSample.Ninject
{
    using System.Collections.Generic;
    using System.Reflection;
    
    using GiftcardSample.Commands;
    using GiftcardSample.ReadStore.InMemory;

    using global::Ninject.Modules;

    using SimpleDomain.Bus;
    
    public class JitneyModule : NinjectModule
    {
        public override void Load()
        {
            var configuration = new JitneyConfiguration(this.Kernel);

            configuration.DefineLocalEndpointAddress("gc.sample");
            configuration.SetSubscriptionStore(new FileSubscriptionStore());
            configuration.MapContracts(typeof(CreateGiftcard).Assembly).ToMe();
            configuration.SubscribeMessageHandlers(GetHandlerAssemblies());

            configuration.AddPipelineStep(new LogIncommingEnvelopeStep());
            
            configuration.UseSimpleJitney();
        }

        private static IEnumerable<Assembly> GetHandlerAssemblies()
        {
            yield return typeof(GiftcardCommandHandler).Assembly;
            yield return typeof(InMemoryCardNumberEventHandler).Assembly;
        } 
    }
}