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
using Cmdty.TimePeriodValueTypes;

namespace Cmdty.Curves
{
    public sealed class Contract<T> where T : ITimePeriod<T>
    {
        public T Start { get; }
        public T End { get; } // TODO this is inclusive end, which is inconsistent with ITimePeriod which where End property is exclusive. Rename something?
        public double Price { get; }

        public Contract(T singlePeriod, double price) : this(singlePeriod, singlePeriod, price)
        {
        }

        public Contract(T start, T end, double price)
        {
            if (start.CompareTo(end) > 0 )
                throw new ArgumentException("start cannot be after end");
            Start = start;
            End = end;
            Price = price;
        }

        public static Contract<T> Create<TPrice>(TPrice timePeriod, double price)
            where TPrice : ITimePeriod<TPrice>
        {
            return new Contract<T>(timePeriod.First<T>(), timePeriod.Last<T>(), price);
        }

        public override string ToString()
        {
            return $"{nameof(Start)}: {Start}, {nameof(End)}: {End}, {nameof(Price)}: {Price}";
        }
    }

    public static class Contract
    {
        public static Contract<T> Create<T>(T start, T end, double price) where T : ITimePeriod<T>
        {
            return new Contract<T>(start, end, price);
        }
        public static Contract<T> CreateForSinglePeriod<T>(T singleTimePeriod, double price) where T : ITimePeriod<T>
        {
            return new Contract<T>(singleTimePeriod, price);
        }


    }
}
