//-------------------------------------------------------------------------------
// <copyright file="InMemoryTraceListenerTest.cs" company="frokonet.ch">
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

namespace SimpleDomain.Common
{
    using System;
    using System.Diagnostics;

    using FluentAssertions;

    using SimpleDomain.Common.Logging;

    using Xunit;

    public class InMemoryTraceListenerTest
    {
        private const string LogText = "This is a log text";

        private const string DebugLevel = "[Debug]";
        private const string InfoLevel = "[Info]";
        private const string WarningLevel = "[Warning]";
        private const string ErrorLevel = "[Error]";

        private static readonly string ClassName = typeof(InMemoryTraceListenerTest).Name;
        private static readonly ILogger Logger = LoggerFactory.Create<InMemoryTraceListenerTest>();

        public InMemoryTraceListenerTest()
        {
            Trace.Listeners.Add(InMemoryTraceListener.Instance);
        }

        [Fact]
        public void LoggedDebugMessagesAreAddedToTheInMemoryTraceListener()
        {
            Logger.Debug(LogText);

            InMemoryTraceListener.LogMessages.Should().Contain(s =>
                s.Contains(DebugLevel) &&
                s.Contains(ClassName) &&
                s.Contains(LogText));

            LogText.Should().HaveBeenLogged().WithDebugLevel();
        }

        [Fact]
        public void LoggedInfoMessagesAreAddedToTheInMemoryTraceListener()
        {
            Logger.Info(LogText);

            InMemoryTraceListener.LogMessages.Should().Contain(s =>
                s.Contains(InfoLevel) &&
                s.Contains(ClassName) &&
                s.Contains(LogText));

            LogText.Should().HaveBeenLogged().WithInfoLevel();
        }

        [Fact]
        public void LoggedWarningMessagesAreAddedToTheInMemoryTraceListener()
        {
            Logger.Warn(LogText);

            InMemoryTraceListener.LogMessages.Should().Contain(s =>
                s.Contains(WarningLevel) &&
                s.Contains(ClassName) &&
                s.Contains(LogText));

            LogText.Should().HaveBeenLogged().WithWarningLevel();
        }

        [Fact]
        public void LoggedErrorMessagesAreAddedToTheInMemoryTraceListener()
        {
            Logger.Error(new Exception(), LogText);

            InMemoryTraceListener.LogMessages.Should().Contain(s =>
                s.Contains(ErrorLevel) &&
                s.Contains(ClassName) &&
                s.Contains(LogText));

            LogText.Should().HaveBeenLogged().WithErrorLevel();
        }
    }
}