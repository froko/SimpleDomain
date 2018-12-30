//-------------------------------------------------------------------------------
// <copyright file="HandlerInvocationCache.cs" company="frokonet.ch">
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

namespace SimpleDomain.Bus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// The handler invocation cache
    /// </summary>
    public class HandlerInvocationCache : IHandlerInvocationCache
    {
        private readonly IDictionary<RuntimeTypeHandle, IList<AsyncMessageDelegate>> handlerCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerInvocationCache"/> class.
        /// </summary>
        public HandlerInvocationCache()
        {
            this.handlerCache = new Dictionary<RuntimeTypeHandle, IList<AsyncMessageDelegate>>();
        }

        /// <inheritdoc />
        public void Add(Type asyncHandlerType, Type messageType)
        {
            var interfaceGenericType = typeof(IHandleAsync<>);
            var handleMethod = GetMethod(asyncHandlerType, messageType, interfaceGenericType);
            if (handleMethod == null)
            {
                return;
            }

            this.Add(asyncHandlerType, messageType, handleMethod);
        }

        /// <inheritdoc />
        public async Task InvokeAsync(object handler, object message)
        {
            IList<AsyncMessageDelegate> methodList;
            if (!this.handlerCache.TryGetValue(handler.GetType().TypeHandle, out methodList))
            {
                return;
            }

            var handlerTasks = methodList
                .Where(x => x.CanHandle(message))
                .Select(x => x.InvokeAsync(handler, message));

            await Task.WhenAll(handlerTasks).ConfigureAwait(false);
        }

        private static Func<object, object, Task> GetMethod(Type targetType, Type messageType, Type interfaceGenericType)
        {
            var interfaceType = interfaceGenericType.MakeGenericType(messageType);

            if (interfaceType.IsAssignableFrom(targetType))
            {
                var methodInfo = targetType.GetInterfaceMap(interfaceType).TargetMethods.FirstOrDefault();
                if (methodInfo != null)
                {
                    var target = Expression.Parameter(typeof(object));
                    var param = Expression.Parameter(typeof(object));

                    var castTarget = Expression.Convert(target, targetType);
                    var castParam = Expression.Convert(param, methodInfo.GetParameters().First().ParameterType);
                    var execute = Expression.Call(castTarget, methodInfo, castParam);
                    return Expression.Lambda<Func<object, object, Task>>(execute, target, param).Compile();
                }
            }

            return null;
        }

        private void Add(Type asyncHandlerType, Type messageType, Func<object, object, Task> handleMethod)
        {
            var typeHandle = asyncHandlerType.TypeHandle;
            var asyncMessageDelegate = new AsyncMessageDelegate(messageType, handleMethod);

            IList<AsyncMessageDelegate> methodList;
            if (this.handlerCache.TryGetValue(typeHandle, out methodList))
            {
                if (methodList.Any(x => x.CanHandle(messageType)))
                {
                    return;
                }

                methodList.Add(asyncMessageDelegate);
            }
            else
            {
                this.handlerCache[typeHandle] = new List<AsyncMessageDelegate> { asyncMessageDelegate };
            }
        }
    }
}