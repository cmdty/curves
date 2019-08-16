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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmdty.TimePeriodValueTypes;
using JetBrains.Annotations;

namespace Cmdty.TimeSeries
{
    [Serializable]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class TimeSeries<TIndex, TData> : IEnumerable<TimeSeriesPoint<TIndex, TData>>, IReadOnlyCollection<TData>, IReadOnlyDictionary<TIndex, TData>
        where TIndex : ITimePeriod<TIndex>
    {
        private static readonly Lazy<TimeSeries<TIndex, TData>> EmptyTimeSeries =
            new Lazy<TimeSeries<TIndex, TData>>(() => new TimeSeries<TIndex, TData>());
        private readonly TData[] _data;
        private protected readonly Lazy<TIndex[]> IndicesLazy;
        private readonly TIndex _start;
        private const int DefaultNumRowsToPrint = 10;

        /// <summary>
        /// Creates an empty TimeSeries instance.
        /// </summary>
        public TimeSeries() : this(new TIndex[0], new TData[0])
        {
        }

        public TimeSeries([NotNull] IEnumerable<TIndex> indices, [NotNull] IEnumerable<TData> data)
        {
            if (indices == null) throw new ArgumentNullException(nameof(indices));
            if (data == null) throw new ArgumentNullException(nameof(data));

            _data = data.ToArray();
            TIndex[] indexArray = indices.ToArray();

            if (indexArray.Length != _data.Length)
                throw new ArgumentException($"Arguments {nameof(indices)} and {nameof(data)} must contain the same number of elements");

            if (indexArray.Length > 1)
            {
                for (int i = 1; i < indexArray.Length; i++)
                {
                    if (indexArray[i].OffsetFrom(indexArray[i - 1]) != 1)
                    {
                        throw new ArgumentException($"Argument {nameof(indices)} does not consist of an ordered contiguous sequence");
                    }
                }
            }

            IndicesLazy = new Lazy<TIndex[]>(() => indexArray);

            if (_data.Length != 0)
                _start = indexArray[0];
        }

        public TimeSeries([NotNull] TIndex start, [NotNull] IEnumerable<TData> data)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (data == null) throw new ArgumentNullException(nameof(data));

            _start = start;
            _data = data.ToArray();

            if (_data.Length == 0)
                throw new ArgumentException("data must contain at least one item");

            IndicesLazy = new Lazy<TIndex[]>(() =>
            {
                var indexArray = new TIndex[_data.Length];
                indexArray[0] = Start;
                if (_data.Length > 1)
                {
                    for (int i = 1; i < _data.Length; i++)
                    {
                        indexArray[i] = Start.Offset(i);
                    }
                }
                return indexArray;
            });
        }

        private protected TimeSeries([NotNull] TData[] data, [NotNull] TIndex start, [NotNull] Lazy<TIndex[]> indices)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            _data = data ?? throw new ArgumentNullException(nameof(data));
            if (data.Length == 0)
                throw new ArgumentException("data must contain at least one item", nameof(data));

            _start = start;
            IndicesLazy = indices ?? throw new ArgumentNullException(nameof(indices));
        }

        public TIndex Start => IsEmpty ? 
            throw new InvalidOperationException("TimeSeries is empty.") : 
            _start;

        public bool IsEmpty => _data.Length == 0;

        // TODO use different exceptions that the IndexOutOfRange which will propagate from array getter
        public TData this[int index] => _data[index];

        // TODO unit test these methods
        public bool ContainsKey([NotNull] TIndex key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (IsEmpty)
                return false;

            int arrayPosition = key.OffsetFrom(Start);
            return arrayPosition >= 0 && arrayPosition < _data.Length;
        }

        public bool TryGetValue(TIndex key, out TData value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (IsEmpty)
            {
                value = default;
                return false;
            }

            int arrayPosition = key.OffsetFrom(Start);
            if (arrayPosition >= 0 && arrayPosition < _data.Length)
            {
                value = _data[arrayPosition];
                return true;
            }
            value = default;
            return false;
        }

        public TData this[[NotNull] TIndex index]
        {
            get
            {
                if (index == null) throw new ArgumentNullException(nameof(index));
                if (IsEmpty)
                    throw new IndexOutOfRangeException("Index was outside the bounds of the array.");

                int arrayPosition = index.OffsetFrom(Start);
                return _data[arrayPosition];
            }
        }

        IEnumerable<TIndex> IReadOnlyDictionary<TIndex, TData>.Keys => Indices;

        IEnumerable<TData> IReadOnlyDictionary<TIndex, TData>.Values => _data;

        public int Count => _data.Length;
        
        public TIndex End => IsEmpty ? 
            throw new InvalidOperationException("TimeSeries is empty.") : 
            Start.Offset(_data.Length - 1);

        public IReadOnlyList<TData> Data => _data;

        public IReadOnlyList<TIndex> Indices => IndicesLazy.Value;

        public override string ToString()
        {
            return ToString(DefaultNumRowsToPrint);
        }

        public string ToString(int maxNumRowsToPrint)
        {
            return TimeSeriesHelper.FormatTimeSeries(this, maxNumRowsToPrint, 
                index => index.ToString(),
                data => data.ToString());
        }

        IEnumerator<KeyValuePair<TIndex, TData>> IEnumerable<KeyValuePair<TIndex, TData>>.GetEnumerator()
        {
            foreach (var pair in _data.Zip(Indices, (data, index) => new {data, index}))
            {
                yield return new KeyValuePair<TIndex, TData>(pair.index, pair.data);
            }
        }

        IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
        {
            foreach (var data in _data)
            {
                yield return data;
            }
        }

        public IEnumerator<TimeSeriesPoint<TIndex, TData>> GetEnumerator()
        {
            TIndex[] indices = IndicesLazy.Value;
            for (int i = 0; i < indices.Length; i++)
            {
                yield return new TimeSeriesPoint<TIndex, TData>(indices[i], _data[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public static TimeSeries<TIndex, TData> Empty => EmptyTimeSeries.Value;
        
        [Serializable]
        public sealed class Builder : BuilderBase<TIndex, TData, TimeSeries<TIndex, TData>>
        {
            public Builder()
            {
            }

            public Builder(int capacity) : base(capacity)
            {
            }

            public override TimeSeries<TIndex, TData> Build()
            {
                return TimeSeries.FromDictionary(Data);
            }
            
        }

    }

    public static class TimeSeries
    {

        public static TimeSeries<TIndex, TData> FromDictionary<TIndex, TData>([NotNull] IDictionary<TIndex, TData> dictionary)
                            where TIndex : ITimePeriod<TIndex>
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            var orderedKeys = dictionary.Keys.OrderBy(index => index).ToArray();
            var orderedValues = orderedKeys.Select(index => dictionary[index]);
            return new TimeSeries<TIndex, TData>(orderedKeys, orderedValues);
        }

    }
}
