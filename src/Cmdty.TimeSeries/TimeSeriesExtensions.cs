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
using System.Threading;
using Cmdty.TimePeriodValueTypes;

namespace Cmdty.TimeSeries
{
    public static class TimeSeriesExtensions
    {
        private const int DefaultNumRowsToPrint = 10;

        public static string ToString<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries,
            string indexFormat, IFormatProvider indexFormatProvider, string dataFormat,
            IFormatProvider dataFormatProvider, int maxNumRowsToPrint)
            where TIndex : ITimePeriod<TIndex>, IFormattable
            where TData : IFormattable
        {
            return TimeSeriesHelper.FormatTimeSeries(timeSeries, maxNumRowsToPrint,
                index => index.ToString(indexFormat, indexFormatProvider),
                data => data.ToString(dataFormat, dataFormatProvider));
        }

        public static string ToString<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries, 
                string indexFormat, IFormatProvider indexFormatProvider, string dataFormat, 
                IFormatProvider dataFormatProvider)
            where TIndex : ITimePeriod<TIndex>, IFormattable
            where TData : IFormattable
        {
            return timeSeries.ToString(indexFormat, indexFormatProvider, dataFormat, dataFormatProvider,
                DefaultNumRowsToPrint);
        }

        public static string ToString<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries,
            string indexFormat, string dataFormat, IFormatProvider formatProvider)
            where TIndex : ITimePeriod<TIndex>, IFormattable
            where TData : IFormattable
        {
            return timeSeries.ToString(indexFormat, formatProvider, dataFormat, formatProvider);
        }

        public static string ToString<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries,
            string indexFormat, string dataFormat, IFormatProvider formatProvider, int maxNumRowsToPrint)
            where TIndex : ITimePeriod<TIndex>, IFormattable
            where TData : IFormattable
        {
            return timeSeries.ToString(indexFormat, formatProvider, dataFormat, formatProvider, maxNumRowsToPrint);
        }

        public static string ToString<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries,
            string indexFormat, string dataFormat)
            where TIndex : ITimePeriod<TIndex>, IFormattable
            where TData : IFormattable
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            return timeSeries.ToString(indexFormat, currentCulture, dataFormat, 
                currentCulture);
        }

        public static string ToString<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries,
            string indexFormat, string dataFormat, int maxNumRowsToPrint)
            where TIndex : ITimePeriod<TIndex>, IFormattable
            where TData : IFormattable
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            return timeSeries.ToString(indexFormat, currentCulture, dataFormat,
                currentCulture, maxNumRowsToPrint);
        }

        public static string FormatIndex<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries,
                    string indexFormat, IFormatProvider indexFormatProvider, int maxNumRowsToPrint)
            where TIndex : ITimePeriod<TIndex>, IFormattable
        {
            return TimeSeriesHelper.FormatTimeSeries(timeSeries, maxNumRowsToPrint,
                index => index.ToString(indexFormat, indexFormatProvider),
                data => data.ToString());
        }

        public static string FormatIndex<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries,
                                    string indexFormat, IFormatProvider indexFormatProvider)
            where TIndex : ITimePeriod<TIndex>, IFormattable
        {
            return timeSeries.FormatIndex(indexFormat, indexFormatProvider, DefaultNumRowsToPrint);
        }

        public static string FormatIndex<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries, 
                string indexFormat, int maxNumRowsToPrint)
            where TIndex : ITimePeriod<TIndex>, IFormattable
        {
            return timeSeries.FormatIndex(indexFormat, Thread.CurrentThread.CurrentCulture, maxNumRowsToPrint);
        }

        public static string FormatData<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries, string dataFormat, 
                IFormatProvider dataFormatProvider, int maxNumRowsToPrint)
            where TIndex : ITimePeriod<TIndex>
            where TData : IFormattable
        {
            return TimeSeriesHelper.FormatTimeSeries(timeSeries, maxNumRowsToPrint,
                index => index.ToString(),
                data => data.ToString(dataFormat, dataFormatProvider));
        }

        public static string FormatData<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries, string dataFormat,
                    IFormatProvider dataFormatProvider)
            where TIndex : ITimePeriod<TIndex>
            where TData : IFormattable
        {
            return timeSeries.FormatData(dataFormat, dataFormatProvider, DefaultNumRowsToPrint);
        }

        public static string FormatData<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries, 
                string dataFormat, int maxNumRowsToPrint)
            where TIndex : ITimePeriod<TIndex>
            where TData : IFormattable
        {
            return timeSeries.FormatData(dataFormat, Thread.CurrentThread.CurrentCulture, maxNumRowsToPrint);
        }


        public static string FormatData<TIndex, TData>(this TimeSeries<TIndex, TData> timeSeries, string dataFormat)
            where TIndex : ITimePeriod<TIndex>
            where TData : IFormattable
        {
            return timeSeries.FormatData(dataFormat, Thread.CurrentThread.CurrentCulture, DefaultNumRowsToPrint);
        }

    }
}
