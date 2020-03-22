﻿//-------------------------------------------------------------------------------
// <copyright file="SnapshotTest.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2020
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
    using FluentAssertions;

    using SimpleDomain.TestDoubles;

    using Xunit;

    public class SnapshotTest
    {
        [Fact]
        public void CanFluentlySetVersion()
        {
            var snapshot = new MySnapshot(13).WithVersion(33);

            snapshot.Version.Should().Be(33);
        }
    }
}