//-------------------------------------------------------------------------------
// <copyright file="DbConnectionFactory.cs" company="frokonet.ch">
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

namespace SimpleDomain.EventStore.Persistence
{
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    /// <summary>
    /// The database connection factory
    /// </summary>
    public class DbConnectionFactory
    {
        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <param name="connectionStringName">The connection string name</param>
        /// <returns>An already opened database connection</returns>
        public virtual SqlConnection Create(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            var connection = new SqlConnection(connectionString);

            connection.Open();

            return connection;
        }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <param name="connectionStringName">The connection string name</param>
        /// <returns>An already opened database connection</returns>
        public virtual async Task<SqlConnection> CreateAsync(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            var connection = new SqlConnection(connectionString);

            await connection.OpenAsync().ConfigureAwait(false);

            return connection;
        }
    }
}