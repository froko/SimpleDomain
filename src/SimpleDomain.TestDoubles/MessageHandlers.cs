//-------------------------------------------------------------------------------
// <copyright file="MessageHandlers.cs" company="frokonet.ch">
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

namespace SimpleDomain.TestDoubles
{
    using System.Threading.Tasks;

    public class ValueCommandHandler : IHandleAsync<ValueCommand>
    {
        public int Value { get; private set; }

        public Task HandleAsync(ValueCommand message)
        {
            return Task.Run(() => this.Handle(message));
        }

        private void Handle(ValueCommand message)
        {
            this.Value = message.Value;
        }
    }

    public class ValueEventHandler : IHandleAsync<ValueEvent>
    {
        public int Value { get; private set; }

        public Task HandleAsync(ValueEvent message)
        {
            return Task.Run(() => this.Handle(message));
        }

        private void Handle(ValueEvent message)
        {
            this.Value = message.Value;
        }
    }

    [PreventAutomaticHandlerRegistration]
    public class NonRegisterableValueEventHandler : IHandleAsync<ValueEvent>
    {
        public Task HandleAsync(ValueEvent message)
        {
            return Task.CompletedTask;
        }
    }

    public class MyEventHandler : IHandleAsync<MyEvent>
    {
        public Task HandleAsync(MyEvent message)
        {
            return Task.CompletedTask;
        }
    }
}