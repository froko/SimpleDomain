//-------------------------------------------------------------------------------
// <copyright file="DbCommandExtensions.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System.Data;

    /// <summary>
    /// Some useful DbCommand extensions
    /// </summary>
    public static class DbCommandExtensions
    {
        /// <summary>
        /// Adds a parameter to a DB command
        /// </summary>
        /// <param name="command">The DB command</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">The parameter value</param>
        public static void AddParameter(this IDbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;

            if (command.Parameters.Contains(parameterName))
            {
                var parameterToRemove = command.Parameters[parameterName];
                command.Parameters.Remove(parameterToRemove);
            }

            command.Parameters.Add(parameter);
        }
    }
}