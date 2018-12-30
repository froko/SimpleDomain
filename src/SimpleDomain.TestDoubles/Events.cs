//-------------------------------------------------------------------------------
// <copyright file="Events.cs" company="frokonet.ch">
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

namespace SimpleDomain.TestDoubles
{
    public class MyEvent : IEvent
    {
    }

    public class OtherEvent : IEvent
    {
    }

    public class ValueEvent : IEvent
    {
        public ValueEvent(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }
    }
}