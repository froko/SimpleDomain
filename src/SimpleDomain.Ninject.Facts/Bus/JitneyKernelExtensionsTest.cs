//-------------------------------------------------------------------------------
// <copyright file="JitneyKernelExtensionsTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ninject;

    using Xunit;

    public class JitneyKernelExtensionsTest : IDisposable
    {
        private readonly IKernel kernel;

        public JitneyKernelExtensionsTest()
        {
            this.kernel = new StandardKernel();

            var jitneyConfiguration = new JitneyConfiguration(this.kernel);
            jitneyConfiguration.Register(config => new StartableJitney(config));
        }

        [Fact]
        public void CanSignalJitneyToStartWork()
        {
            StartableJitney.HasBeenStarted.Should().BeFalse();

            this.kernel.SignalJitneyToStartWork();

            StartableJitney.HasBeenStarted.Should().BeTrue();
        }

        [Fact]
        public void CanSignalJitneyToStopWork()
        {
            StartableJitney.HasBeenStopped.Should().BeFalse();

            this.kernel.SignalJitneyToStopWork();

            StartableJitney.HasBeenStopped.Should().BeTrue();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        private class StartableJitney : Jitney
        {
            public StartableJitney(IHaveJitneyConfiguration configuration)
                : base(configuration)
            {
                HasBeenStarted = false;
            }

            public static bool HasBeenStarted { get; private set; }

            public static bool HasBeenStopped { get; private set; }

            public override Task StartAsync()
            {
                HasBeenStarted = true;
                return Task.CompletedTask;
            }

            public override Task StopAsync()
            {
                HasBeenStopped = true;
                return Task.CompletedTask;
            }

            public override Task SendAsync<TCommand>(TCommand command)
            {
                throw new NotImplementedException();
            }

            public override Task PublishAsync<TEvent>(TEvent @event)
            {
                throw new NotImplementedException();
            }
        }
    }
}