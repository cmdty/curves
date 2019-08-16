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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Cmdty.TimePeriodValueTypes.Test
{
    [TestFixture]
    public sealed class TimePeriodExtensionsTest
    {

        [Test]
        public void EnumerateTo_NoFiler_AsExpected()
        {
            var start = new Day(2018, 3, 12);
            var end = new Day(2018, 3, 14);

            var list = start.EnumerateTo(end).ToList();

            var expectedList = new List<Day>
            {
                new Day(2018, 3, 12),
                new Day(2018, 3, 13),
                new Day(2018, 3, 14)
            };

            Assert.AreEqual(expectedList, list);
        }

        [Test]
        public void EnumerateTo_WithFilter_AsExpected()
        {
            var start = Month.CreateMarch(2019);
            var end = Month.CreateMay(2019);

            var list = start.EnumerateTo(end, month => month.MonthOfYear > 3).ToList();

            var expectedList = new List<Month>
            {
                Month.CreateApril(2019),
                Month.CreateMay(2019)
            };

            Assert.AreEqual(expectedList, list);
        }

        // TODO EnumerateTo exceptions

        [Test]
        public void EnumerateWeekdays_AsExpected()
        {
            var start = new Day(2019, 1, 10); // Thursday
            var end = new Day(2019, 1, 14);   // Monday

            var weekdays = start.EnumerateWeekdays(end);

            var expectedWeekdays = new List<Day>
            {
                new Day(2019, 1, 10),
                new Day(2019, 1, 11),
                new Day(2019, 1, 14)
            };

            Assert.AreEqual(expectedWeekdays, weekdays);
        }

        [Test]
        public void EnumerateBusinessDays_AsExpected()
        {
            var start = new Day(2019, 1, 10); // Thursday
            var end = new Day(2019, 1, 15);   // Tuesday

            var holidays = new List<Day>{new Day(2019, 1, 14)};

            var weekdays = start.EnumerateBusinessDays(end, holidays);

            var expectedWeekdays = new List<Day>
            {
                new Day(2019, 1, 10),
                new Day(2019, 1, 11),
                new Day(2019, 1, 15)
            };

            Assert.AreEqual(expectedWeekdays, weekdays);
        }

        // TODO move into per type test classes
        [Test]
        public void First_AsExpected()
        {
            var month = Month.CreateMarch(2019);
            var firstDay = month.First<Day>();
            var expectedFirstDay = new Day(2019, 3, 1);
            Assert.AreEqual(expectedFirstDay, firstDay);
        }

        // TODO move into per type test classes
        [Test]
        public void Last_AsExpected()
        {
            var month = Month.CreateMarch(2019);
            var lastDay = month.Last<Day>();
            var expectedLastDay = new Day(2019, 3, 31);
            Assert.AreEqual(expectedLastDay, lastDay);
        }

        // TODO move into per type test classes
        [Test]
        public void Expand_AsExpected()
        {
            var year = new CalendarYear(2019);
            var monthsInYear = year.Expand<Month>();

            var expectedMonthsInYear = new[]
            {
                Month.CreateJanuary(2019),
                Month.CreateFebruary(2019),
                Month.CreateMarch(2019),
                Month.CreateApril(2019),
                Month.CreateMay(2019),
                Month.CreateJune(2019),
                Month.CreateJuly(2019),
                Month.CreateAugust(2019),
                Month.CreateSeptember(2019),
                Month.CreateOctober(2019),
                Month.CreateNovember(2019),
                Month.CreateDecember(2019)
            };

            Assert.AreEqual(expectedMonthsInYear, monthsInYear);
        }

    }
}
