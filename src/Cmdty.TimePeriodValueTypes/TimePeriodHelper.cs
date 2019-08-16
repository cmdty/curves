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

namespace Cmdty.TimePeriodValueTypes
{
    internal static class TimePeriodHelper
    {

        private static readonly int[] NumDaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private const int NumDaysInFebOnLeapYear = 29;

        // TODO move to Preconditions class
        internal static void CheckDayOfMonth(string paramName, int year, int monthOfYear, int day)
        {
            if (day < 1)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Parameter {paramName} has an invalid value of {day} for a day of month. Value must be above 1.");
            }

            if (day <= 28) // No need to look up days of month as this value will definitely be ok
            {
                return;
            }

            int numDaysInMonth = monthOfYear == 2 && IsLeapYear(year) ? NumDaysInFebOnLeapYear : NumDaysPerMonth[monthOfYear - 1];

            if (day > numDaysInMonth)
            {
                throw new ArgumentOutOfRangeException(paramName, 
                    $"Parameter {paramName} has an invalid value of {day} for a day of month. Value must be less than or equal to {numDaysInMonth}, the number of days in month number {monthOfYear} on year {year}.");
            }
        }

        private static bool IsLeapYear(int year)
        {
            // For rules of leap year: https://en.wikipedia.org/wiki/Leap_year#Gregorian_calendar
            // Checking if year is multiple of 4 using bitwise and, see: https://en.wikipedia.org/wiki/Modulo_operation#Performance_issues
            return (year & 3) == 0 && (year % 100 != 0 || year % 400 == 0);
        }

        internal static T2 First<T1, T2>(T1 timePeriod) // TODO provide optional parameter to handle what happens if result Start property doesn't equal timePeriod.Start?
            where T1 : ITimePeriod<T1>
            where T2 : ITimePeriod<T2>
        {
            return TimePeriodFactory.FromDateTime<T2>(timePeriod.Start);
        }

        internal static T2 Last<T1, T2>(T1 timePeriod) // TODO provide optional parameter to handle what happens if result End property doesn't equal timePeriod.End?
            where T1 : ITimePeriod<T1>
            where T2 : ITimePeriod<T2>
        {
            return TimePeriodFactory.FromDateTime<T2>(timePeriod.End).Previous();
        }

        // TODO error handling for when expanding from small to larger type, e.g. Day to CalendarYear?
        internal static IEnumerable<T2> Expand<T1, T2>(T1 timePeriod) // TODO provide optional parameter for cases when time periods don't exactly match up?
            where T1 : ITimePeriod<T1>
            where T2 : ITimePeriod<T2>
        {
            var end = timePeriod.End;
            var expanded = First<T1, T2>(timePeriod);

            do
            {
                yield return expanded;
                expanded = expanded.Next();
            } while (expanded.End <= end);
        }

    }
}
