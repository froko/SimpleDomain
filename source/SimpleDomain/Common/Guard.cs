// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Guard.cs" company="frokonet.ch">
//   Copyright (C) frokonet.ch, 2014-2018
//   //   Licensed under the Apache License, Version 2.0 (the "License");
//   //   you may not use this file except in compliance with the License.
//   //   You may obtain a copy of the License at
//   //       http://www.apache.org/licenses/LICENSE-2.0
//   //   Unless required by applicable law or agreed to in writing, software
//   //   distributed under the License is distributed on an "AS IS" BASIS,
//   //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   //   See the License for the specific language governing permissions and
//   //   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SimpleDomain.Common
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    /// <summary>
    /// Common guard class for argument validation.
    /// </summary>
    [DebuggerStepThrough]
    public static class Guard
    {
        /// <summary>
        /// Ensures the value of the given <paramref name="argumentExpression"/> is not null.
        /// Throws <see cref="ArgumentNullException"/> otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="argumentExpression">The function to get the value</param>
        public static void NotNull<T>(Expression<Func<T>> argumentExpression) where T : class
        {
            if (GetValue(argumentExpression) == null)
            {
                throw new ArgumentNullException(GetParameterName(argumentExpression), ExceptionMessages.ParameterCannotBeNull);
            }
        }

        /// <summary>
        /// Ensures the string value of the given <paramref name="argumentExpression"/> is not null or empty.
        /// Throws <see cref="ArgumentNullException"/> in the first case, or
        /// <see cref="ArgumentException"/> in the latter.
        /// </summary>
        /// <param name="argumentExpression">The function to get the value</param>
        public static void NotNullOrEmpty(Expression<Func<string>> argumentExpression)
        {
            var value = GetValue(argumentExpression);

            if (value == null)
            {
                throw new ArgumentNullException(GetParameterName(argumentExpression), ExceptionMessages.ParameterCannotBeNull);
            }

            if (value.Length == 0)
            {
                throw new ArgumentException(GetParameterName(argumentExpression), ExceptionMessages.ParameterCannotBeEmpty);
            }
        }

        /// <summary>
        /// Ensures that a given boolean expectation is true.
        /// Throws <see cref="ArgumentException"/> when it is false.
        /// </summary>
        /// <param name="expectation">The boolean expectation</param>
        /// <param name="exceptionMessage">The exception message</param>
        public static void IsTrue(bool expectation, string exceptionMessage)
        {
            if (!expectation)
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        /// Ensures that a given value is between a given range
        /// </summary>
        /// <param name="numericValue">The value</param>
        /// <returns>An instance of <see cref="IAmBetween{T}"/></returns>
        public static IAmBetween<int> Value(int numericValue)
        {
            return new Int32BetweenGuard(numericValue);
        }

        private static T GetValue<T>(Expression<Func<T>> reference) where T : class
        {
            return reference.Compile().Invoke();
        }

        private static string GetParameterName(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var memberExpression = lambdaExpression?.Body as MemberExpression;

            return memberExpression?.Member.Name ?? string.Empty;
        }
    }
}