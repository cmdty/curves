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
using System.Collections.Generic;
using System.Linq;
using Cmdty.TimePeriodValueTypes;
using JetBrains.Annotations;

namespace Cmdty.Curves
{
    public static class MaxSmoothnessSplineCurveBuilderExtensions
    {

        public static ISplineAddOptionalParameters<T> AddContracts<T>([NotNull] this IMaxSmoothSplineCurveBuilder<T> splineCurveBuilder,
            [NotNull] IEnumerable<Contract<T>> contracts)
            where T : ITimePeriod<T>
        {
            if (splineCurveBuilder == null) throw new ArgumentNullException(nameof(splineCurveBuilder));
            if (contracts == null) throw new ArgumentNullException(nameof(contracts));

            var contractList = contracts.ToList();

            if (contractList.Count < 1)
                throw new ArgumentException("contracts must have at least one element", nameof(contracts));

            ISplineAddOptionalParameters<T> addSecondContract = splineCurveBuilder.AddContract(contractList[0]);

            foreach (var contract in contractList.Skip(1))
            {
                addSecondContract = addSecondContract.AddContract(contract);
            }

            return addSecondContract;
        }

        public static ISplineAddOptionalParameters<T> AddContract<T>(this MaxSmoothnessSplineCurveBuilder<T> splineCurveBuilder,
            T start, T end, double price)
            where T : ITimePeriod<T>
        {
            return splineCurveBuilder.AddContract(new Contract<T>(start, end, price));
        }

        public static ISplineAddOptionalParameters<TResult> AddContract<TResult, TContract>
                            (this MaxSmoothnessSplineCurveBuilder<TResult> splineCurveBuilder, TContract singlePeriod, double price)
            where TResult : ITimePeriod<TResult>
            where TContract : ITimePeriod<TContract>
        {
            return splineCurveBuilder.AddContract(Contract<TResult>.Create(singlePeriod, price));
        }

        public static ISplineAddOptionalParameters<T> AddContract<T>(this ISplineAddOptionalParameters<T> addFirstContract,
            T start, T end, double price)
            where T : ITimePeriod<T>
        {
            return addFirstContract.AddContract(new Contract<T>(start, end, price));
        }

        public static ISplineAddOptionalParameters<TResult> AddContract<TResult, TContract>
                            (this ISplineAddOptionalParameters<TResult> splineCurveBuilder, TContract singlePeriod, double price)
            where TResult : ITimePeriod<TResult>
            where TContract : ITimePeriod<TContract>
        {
            return splineCurveBuilder.AddContract(Contract<TResult>.Create(singlePeriod, price));
        }
        
    }
}
