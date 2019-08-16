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
using JetBrains.Annotations;

namespace Cmdty.TimePeriodValueTypes
{
    public static class TimePeriodExtensions
    {

        public static T Next<T>([NotNull] this T timePeriod) where T : ITimePeriod<T>
        {
            if (timePeriod == null) throw new ArgumentNullException(nameof(timePeriod));
            return timePeriod.Offset(1);
        }

        public static T Previous<T>([NotNull] this T timePeriod) where T : ITimePeriod<T>
        {
            if (timePeriod == null) throw new ArgumentNullException(nameof(timePeriod));
            return timePeriod.Offset(-1);
        }

        public static IEnumerable<T> EnumerateTo<T>([NotNull] this T start, [NotNull] T inclusiveEnd, 
                                                [NotNull] Func<T, bool> filter)
            where T : ITimePeriod<T>
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (inclusiveEnd == null) throw new ArgumentNullException(nameof(inclusiveEnd));
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            if (start.CompareTo(inclusiveEnd) > 0)
                throw new ArgumentException("start parameter cannot be after inclusiveEnd parameter");

            var enumeratedValue = start;

            while (enumeratedValue.CompareTo(inclusiveEnd) <= 0)
            {
                if (filter(enumeratedValue))
                {
                    yield return enumeratedValue;
                }
                enumeratedValue = enumeratedValue.Next();
            }
        }

        public static IEnumerable<T> EnumerateTo<T>([NotNull] this T start, [NotNull] T inclusiveEnd)
            where T : ITimePeriod<T>
        {
            return start.EnumerateTo(inclusiveEnd, timePeriod => true);
        }

        public static IEnumerable<Day> EnumerateBusinessDays([NotNull] this Day start,
                            [NotNull] Day inclusiveEnd, [NotNull] IEnumerable<Day> holidays)
        {
            if (holidays == null) throw new ArgumentNullException(nameof(holidays));

            return start.EnumerateTo(inclusiveEnd, day => day.DayOfWeek != DayOfWeek.Saturday && 
                                                          day.DayOfWeek != DayOfWeek.Sunday && 
                                                          !holidays.Contains(day));
        }

        public static IEnumerable<Day> EnumerateWeekdays([NotNull] this Day start, [NotNull] Day inclusiveEnd)
        {
            return start.EnumerateTo(inclusiveEnd, day => day.DayOfWeek != DayOfWeek.Saturday &&
                                                          day.DayOfWeek != DayOfWeek.Sunday);
        }
        
    }
}
