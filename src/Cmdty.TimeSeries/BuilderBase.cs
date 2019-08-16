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
using Cmdty.TimePeriodValueTypes;

namespace Cmdty.TimeSeries
{
    [Serializable]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public abstract class BuilderBase<TIndex, TData, TSeries> : IEnumerable
        where TIndex : ITimePeriod<TIndex>
        where TSeries : TimeSeries<TIndex, TData>
    {
        // TODO use SortedDictionary or SortedList as type of Data?
        protected readonly Dictionary<TIndex, TData> Data;

        protected BuilderBase()
        {
            Data = new Dictionary<TIndex, TData>();
        }

        protected BuilderBase(int capacity)
        {
            Data = new Dictionary<TIndex, TData>(capacity);
        }

        public void Add(TIndex index, TData data)
        {
            Data.Add(index, data);
        }

        public BuilderBase<TIndex, TData, TSeries> WithData(TIndex index, TData data)
        {
            Data.Add(index, data);
            return this;
        }

        public abstract TSeries Build();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public int Count => Data.Count;

        // TODO add indexer to get and set data
        // TODO add other methods, such as to get keys and data?
    }
}
