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
using Cmdty.TimeSeries;
using JetBrains.Annotations;

namespace Cmdty.Curves
{
    [Serializable]
    public sealed class DoubleCurve<TIndex> : DoubleTimeSeries<TIndex>
        where TIndex : ITimePeriod<TIndex>
    {
        private static readonly Lazy<DoubleCurve<TIndex>> EmptyCurve =
            new Lazy<DoubleCurve<TIndex>>(() => new DoubleCurve<TIndex>());
        private readonly Func<TIndex, double> _weighting;

        public DoubleCurve()
        {
        }

        public DoubleCurve([NotNull] IEnumerable<TIndex> indices, [NotNull] IEnumerable<double> data,
                            [NotNull] Func<TIndex, double> weighting) 
            : base(indices, data)
        {
            _weighting = weighting ?? throw new ArgumentNullException(nameof(weighting));
        }

        public DoubleCurve([NotNull] TIndex start, [NotNull] IEnumerable<double> data,
                            [NotNull] Func<TIndex, double> weighting) 
            : base(start, data)
        {
            _weighting = weighting ?? throw new ArgumentNullException(nameof(weighting));
        }

        public double Price([NotNull] TIndex start, [NotNull] TIndex end)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));

            if (IsEmpty)
                throw new InvalidOperationException("TimeSeries is empty.");

            if (start.CompareTo(end) > 0)
                throw new ArgumentException("start parameter cannot be after end parameter.");

            if (start.CompareTo(Start) < 0)
                throw new ArgumentException("start parameter cannot be before Start index of current instance.", nameof(start));

            if (end.CompareTo(End) > 0)
                throw new ArgumentException("end parameter cannot be after end index of current instance.", nameof(end));

            double sumWeightedPrice = 0.0;
            double sumWeight = 0.0;

            foreach (TIndex index in start.EnumerateTo(end))
            {
                double weight = _weighting(index);
                sumWeight += weight;
                sumWeightedPrice += this[index] * weight;
            }

            return sumWeightedPrice / sumWeight;
        }

        public double Price<TPeriod>([NotNull] TPeriod period)
            where TPeriod : ITimePeriod<TPeriod>
        {
            if (period == null) throw new ArgumentNullException(nameof(period));

            var first = period.First<TIndex>();
            var last = period.Last<TIndex>();

            return Price(first, last);
        }

        public new static DoubleCurve<TIndex> Empty => EmptyCurve.Value;

        [Serializable]
        public new sealed class Builder : BuilderBase<TIndex, double, DoubleCurve<TIndex>>
        {
            private readonly Func<TIndex, double> _weighting;

            public Builder([NotNull] Func<TIndex, double> weighting)
            {
                _weighting = weighting ?? throw new ArgumentNullException(nameof(weighting));
            }

            public Builder(int capacity, Func<TIndex, double> weighting) : base(capacity)
            {
                _weighting = weighting ?? throw new ArgumentNullException(nameof(weighting));
            }

            public override DoubleCurve<TIndex> Build()
            {
                return DoubleCurve.FromDictionary(Data, _weighting);
            }

        }

    }

    public static class DoubleCurve
    {

        public static DoubleCurve<TIndex> FromDictionary<TIndex>([NotNull] IDictionary<TIndex, double> dictionary, 
                                        Func<TIndex, double> weighting)
            where TIndex : ITimePeriod<TIndex>
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            var orderedKeys = dictionary.Keys.OrderBy(index => index).ToArray();
            var orderedValues = orderedKeys.Select(index => dictionary[index]);
            return new DoubleCurve<TIndex>(orderedKeys, orderedValues, weighting);
        }

    }

}
