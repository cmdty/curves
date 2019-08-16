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
using System.Text;
using Cmdty.TimePeriodValueTypes;

namespace Cmdty.TimeSeries
{
    internal static class TimeSeriesHelper
    {

        // TODO review code in this method and try to simplify
        internal static string FormatTimeSeries<TIndex, TData>(TimeSeries<TIndex, TData> timeSeries, 
                int maxNumRowsToPrint, Func<TIndex, string> indexFormatter, Func<TData, string> dataFormatter)
            where TIndex : ITimePeriod<TIndex>
        {
            if (maxNumRowsToPrint != -1 && maxNumRowsToPrint < 1)
                throw new ArgumentException("maxNumRowsToPrint must be either equal to -1, or greater than 0", nameof(maxNumRowsToPrint));

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Count = {timeSeries.Count}");

            if (maxNumRowsToPrint == -1 || maxNumRowsToPrint > 1)
            {
                int rowToPrintBreak = -1;

                if (maxNumRowsToPrint != -1 && timeSeries.Count + 1 > maxNumRowsToPrint)
                {
                    rowToPrintBreak = (maxNumRowsToPrint + 1) / 2 - 1;
                }

                var rowsFormatted = new List<(string IndexFormatted, string DataFormatted)>(timeSeries.Count);

                int rowToPrintBreakOriginal = rowToPrintBreak;
                for (var i = 0; i < timeSeries.Count; i++)
                {
                    if (i == rowToPrintBreak)
                    {
                        i = timeSeries.Count - i - 1 + (maxNumRowsToPrint & 1);
                        rowToPrintBreak = -1;
                    }
                    else
                    {
                        string indexFormatted = indexFormatter(timeSeries.Indices[i]);
                        string dataFormatted = dataFormatter(timeSeries[i]);
                        rowsFormatted.Add((indexFormatted, dataFormatted));
                    }
                }

                int maxTotalWidth;
                int maxIndexWidth = 0;
                if (rowsFormatted.Count == 0)
                {
                    maxTotalWidth = $"Count = {timeSeries.Count}".Length;
                }
                else
                {
                    maxIndexWidth = rowsFormatted.Max(tuple => tuple.IndexFormatted.Length);
                    maxTotalWidth = maxIndexWidth + rowsFormatted.Max(tuple => tuple.DataFormatted.Length) + 2;
                }

                for (var i = 0; i < rowsFormatted.Count; i++)
                {
                    if (i == rowToPrintBreakOriginal)
                    {
                        stringBuilder.AppendLine(new string('.', maxTotalWidth));
                    }
                    var (indexFormatted, dataFormatted) = rowsFormatted[i];
                    string row = indexFormatted.PadRight(maxIndexWidth + 2, ' ') +
                                 dataFormatted;
                    stringBuilder.AppendLine(row);
                }

                if (rowToPrintBreakOriginal >= rowsFormatted.Count)
                {
                    stringBuilder.AppendLine(new string('.', maxTotalWidth));
                }

            }
            return stringBuilder.ToString().TrimEnd();
        }

    }
}
