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

namespace Cmdty.TimeSeries
{
    [Serializable]
    public class DoubleTimeSeries<TIndex> : TimeSeries<TIndex, double>
        where TIndex : ITimePeriod<TIndex>
    {
        private static readonly Lazy<DoubleTimeSeries<TIndex>> EmptyTimeSeries =
            new Lazy<DoubleTimeSeries<TIndex>>(() => new DoubleTimeSeries<TIndex>());

        public DoubleTimeSeries()
        {
        }

        public DoubleTimeSeries([NotNull] IEnumerable<TIndex> indices, [NotNull] IEnumerable<double> data) 
            : base(indices, data)
        {
        }

        public DoubleTimeSeries([NotNull] TIndex start, [NotNull] IEnumerable<double> data) 
            : base(start, data)
        {
        }

        internal DoubleTimeSeries([NotNull] double[] data, [NotNull] TIndex start, [NotNull] Lazy<TIndex[]> indices)
            : base(data, start, indices)
        {
        }

        public DoubleTimeSeries<TIndex> Add(double number)
        {
            return TransformData(d => d + number);
        }

        public DoubleTimeSeries<TIndex> Subtract(double number)
        {
            return TransformData(d => d - number);
        }

        public DoubleTimeSeries<TIndex> Multiply(double number)
        {
            return TransformData(d => d * number);
        }

        public DoubleTimeSeries<TIndex> Divide(double number)
        {
            return TransformData(d => d / number);
        }


        private DoubleTimeSeries<TIndex> TransformData([NotNull] Func<double, double> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (IsEmpty)
                return this;

            var doubleArray = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                doubleArray[i] = selector(this[i]);
            }
            return new DoubleTimeSeries<TIndex>(doubleArray, Start, IndicesLazy);
        }
        
        private DoubleTimeSeries<TIndex> Add([NotNull] DoubleTimeSeries<TIndex> doubleTimeSeries)
        {
            return BinaryOperation(doubleTimeSeries, (d1, d2) => d1 + d2);
        }

        private DoubleTimeSeries<TIndex> Subtract([NotNull] DoubleTimeSeries<TIndex> doubleTimeSeries)
        {
            return BinaryOperation(doubleTimeSeries, (d1, d2) => d1 - d2);
        }

        private DoubleTimeSeries<TIndex> Multiply([NotNull] DoubleTimeSeries<TIndex> doubleTimeSeries)
        {
            return BinaryOperation(doubleTimeSeries, (d1, d2) => d1 * d2);
        }

        private DoubleTimeSeries<TIndex> Divide([NotNull] DoubleTimeSeries<TIndex> doubleTimeSeries)
        {
            return BinaryOperation(doubleTimeSeries, (d1, d2) => d1 / d2);
        }

        private DoubleTimeSeries<TIndex> BinaryOperation(DoubleTimeSeries<TIndex> otherDoubleTimeSeries, 
                                            Func<double, double, double> selector)
        {
            if (otherDoubleTimeSeries == null) throw new ArgumentNullException(nameof(otherDoubleTimeSeries));
            if (otherDoubleTimeSeries.IsEmpty)
                return this;

            if (IsEmpty)
                throw new InvalidOperationException("TimeSeries is empty.");

            int startTransformIndex = otherDoubleTimeSeries.Start.OffsetFrom(Start);
            if (startTransformIndex < 0)
                throw new ArgumentException(
                    "DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance.");

            int endTransformIndex = otherDoubleTimeSeries.End.OffsetFrom(Start);
            if (endTransformIndex > Count - 1)
                throw new ArgumentException(
                    "DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance.");

            var doubleArray = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                if (i < startTransformIndex || i > endTransformIndex)
                {
                    doubleArray[i] = this[i];
                }
                else
                {
                    int otherIndex = i - startTransformIndex;
                    doubleArray[i] = selector(this[i], otherDoubleTimeSeries[otherIndex]);
                }
            }

            return new DoubleTimeSeries<TIndex>(doubleArray, Start, IndicesLazy);
        }

        public static DoubleTimeSeries<TIndex> operator +(DoubleTimeSeries<TIndex> doubleTimeSeries, double number)
        {
            return doubleTimeSeries.Add(number);
        }

        public static DoubleTimeSeries<TIndex> operator -(DoubleTimeSeries<TIndex> doubleTimeSeries, double number)
        {
            return doubleTimeSeries.Subtract(number);
        }

        public static DoubleTimeSeries<TIndex> operator *(DoubleTimeSeries<TIndex> doubleTimeSeries, double number)
        {
            return doubleTimeSeries.Multiply(number);
        }

        public static DoubleTimeSeries<TIndex> operator /(DoubleTimeSeries<TIndex> doubleTimeSeries, double number)
        {
            return doubleTimeSeries.Divide(number);
        }

        public static DoubleTimeSeries<TIndex> operator +(double number, DoubleTimeSeries<TIndex> doubleTimeSeries)
        {
            return doubleTimeSeries.Add(number);
        }

        public static DoubleTimeSeries<TIndex> operator *(double number, DoubleTimeSeries<TIndex> doubleTimeSeries)
        {
            return doubleTimeSeries.Multiply(number);
        }

        public static DoubleTimeSeries<TIndex> operator +(DoubleTimeSeries<TIndex> left, DoubleTimeSeries<TIndex> right)
        {
            return left.Add(right);
        }
        
        public static DoubleTimeSeries<TIndex> operator -(DoubleTimeSeries<TIndex> left, DoubleTimeSeries<TIndex> right)
        {
            return left.Subtract(right);
        }

        public static DoubleTimeSeries<TIndex> operator *(DoubleTimeSeries<TIndex> left, DoubleTimeSeries<TIndex> right)
        {
            return left.Multiply(right);
        }

        public static DoubleTimeSeries<TIndex> operator /(DoubleTimeSeries<TIndex> left, DoubleTimeSeries<TIndex> right)
        {
            return left.Divide(right);
        }

        public new static DoubleTimeSeries<TIndex> Empty => EmptyTimeSeries.Value;

        [Serializable]
        public new sealed class Builder : BuilderBase<TIndex, double, DoubleTimeSeries<TIndex>>
        {

            public Builder()
            {
            }

            public Builder(int capacity) : base(capacity)
            {
            }

            public override DoubleTimeSeries<TIndex> Build()
            {
                return DoubleTimeSeries.FromDictionary(Data);
            }

        }

    }

    public static class DoubleTimeSeries
    {

        public static DoubleTimeSeries<TIndex> FromDictionary<TIndex>([NotNull] IDictionary<TIndex, double> dictionary)
            where TIndex : ITimePeriod<TIndex>
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            var orderedKeys = dictionary.Keys.OrderBy(index => index).ToArray();
            var orderedValues = orderedKeys.Select(index => dictionary[index]);
            return new DoubleTimeSeries<TIndex>(orderedKeys, orderedValues);
        }

    }

}
