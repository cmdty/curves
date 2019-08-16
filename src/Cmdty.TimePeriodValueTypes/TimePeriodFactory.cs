#region License
// Copyright (c) 2019 Jake Fowler
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without 
// restriction, including without limitation the rights to use, 
// copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following 
// conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Cmdty.TimePeriodValueTypes
{
    public static class TimePeriodFactory<T>
                    where T : ITimePeriod<T>
    {
        // TODO look at whether dictionary is necessary as key type information is already held in the generic type T of TimePeriodFactory
        private static readonly ConcurrentDictionary<Type, Func<DateTime, T>> Creators = 
            new ConcurrentDictionary<Type, Func<DateTime, T>>();
        // TODO after benchmarking, potentially populate this dictionary with factory methods of known TimePeriod types

        public static T FromDateTime(DateTime dateTime)
        {
            var timePeriodType = typeof(T);
            var creator = Creators.GetOrAdd(timePeriodType, ConstructCreator);
            return creator(dateTime);
        }

        // TODO benchmark performance
        private static Func<DateTime, T> ConstructCreator(Type type)
        {
            var dateTimeParameter = Expression.Parameter(typeof(DateTime));
            // TODO look at other overloads of Type.GetMethod which specify argument types
            MethodInfo staticFactoryMethod = type.GetMethod("FromDateTime", BindingFlags.Public | BindingFlags.Static);

            // TODO look for constructors which accept single DateTime parameters

            if (staticFactoryMethod == null)
                throw new ArgumentException($"Type {type.Name} does not contain public static method FromDateTime");

            var expression = Expression.Call(staticFactoryMethod, dateTimeParameter);
            var lambda2 = Expression.Lambda<Func<DateTime, T>>(expression, dateTimeParameter);
            return lambda2.Compile();
        }

    }
    public static class TimePeriodFactory
    {

        public static T FromDateTime<T>(DateTime dateTime)
            where T : ITimePeriod<T>
        {
            return TimePeriodFactory<T>.FromDateTime(dateTime);
        }

    }
}
