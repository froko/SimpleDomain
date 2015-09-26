//-------------------------------------------------------------------------------
// <copyright file="GuardTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Common
{
    using System;

    using FluentAssertions;

    using Xunit;

    public class GuardTest
    {
        [Fact]
        public void ThrowsException_WhenArgumentIsNull()
        {
            object argument = null;

            Action action = () => Guard.NotNull(() => argument);

            action.ShouldThrow<ArgumentNullException>().Where(exception => exception.Message.Contains("argument"));
        }

        [Fact]
        public void DoesNothing_WhenArgumentIsNotNull()
        {
            var argument = new object();

            Action action = () => Guard.NotNull(() => argument);

            action.ShouldNotThrow<Exception>();
        }

        [Fact]
        public void ThrowsException_WhenStringIsNull()
        {
            string myString = null;

            Action action = () => Guard.NotNullOrEmpty(() => myString);

            action.ShouldThrow<ArgumentNullException>().Where(exception => exception.Message.Contains("myString"));
        }

        [Fact]
        public void ThrowsException_WhenStringIsEmpty()
        {
            var myString = string.Empty;

            Action action = () => Guard.NotNullOrEmpty(() => myString);

            action.ShouldThrow<ArgumentException>().Where(exception => exception.Message.Contains("myString"));
        }

        [Fact]
        public void DoesNothing_WhenStringIsNotNullOrEmpty()
        {
            const string MyString = "Hello World";

            Action action = () => Guard.NotNullOrEmpty(() => MyString);

            action.ShouldNotThrow<Exception>();
        }

        [Fact]
        public void ThrowsException_WhenExpectationIsFalse()
        {
            const string ExceptionMessage = "Something went wrong";

            Action action = () => Guard.IsTrue(false, ExceptionMessage);

            action.ShouldThrow<ArgumentException>().WithMessage(ExceptionMessage);
        }

        [Fact]
        public void DoesNothing_WhenExpectationIsTrue()
        {
            Action action = () => Guard.IsTrue(true, "Something went wrong");

            action.ShouldNotThrow<Exception>();
        }
    }
}