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
    public static class BootstrapperExtensions
    {

        public static IBootstrapperAddOptionalParameters<T> AddContracts<T>([NotNull] this IBootstrapper<T> bootstrapper,
                                            [NotNull] IEnumerable<Contract<T>> contracts)
            where T : ITimePeriod<T>
        {
            if (bootstrapper == null) throw new ArgumentNullException(nameof(bootstrapper));
            if (contracts == null) throw new ArgumentNullException(nameof(contracts));
            
            var contractList = contracts.ToList();

            if (contractList.Count < 1)
                throw new ArgumentException("contracts parameter must contain at least one element.", nameof(contracts));

            IBootstrapperAddOptionalParameters<T> addOptionalParameters = bootstrapper.AddContract(contractList[0]);

            foreach (var contract in contractList.Skip(1))
            {
                addOptionalParameters = addOptionalParameters.AddContract(contract);
            }
            
            return addOptionalParameters;
        }

        public static IBootstrapperAddOptionalParameters<T> AddContract<T>(this Bootstrapper<T> bootstrapper, 
                                            T start, T end, double price)
            where T : ITimePeriod<T>
        {
            return bootstrapper.AddContract(new Contract<T>(start, end, price));
        }

        public static IBootstrapperAddOptionalParameters<T> AddContract<T, TPrice>(this Bootstrapper<T> bootstrapper,
                                            TPrice timePeriod, double price)
            where T : ITimePeriod<T>
            where TPrice : ITimePeriod<TPrice>
        {
            return bootstrapper.AddContract(Contract<T>.Create(timePeriod, price));
        }

        public static IBootstrapperAddOptionalParameters<T> AddContract<T>(this IBootstrapperAddOptionalParameters<T> addSecondContract,
                                                    T start, T end, double price)
            where T : ITimePeriod<T>
        {
            return addSecondContract.AddContract(new Contract<T>(start, end, price));
        }

        public static IBootstrapperAddOptionalParameters<T> AddContract<T, TPrice>(this IBootstrapperAddOptionalParameters<T> addFirstContract,
                                                        TPrice timePeriod, double price)
            where T : ITimePeriod<T>
            where TPrice : ITimePeriod<TPrice>
        {
            return addFirstContract.AddContract(Contract<T>.Create(timePeriod, price));
        }
        
    }
}