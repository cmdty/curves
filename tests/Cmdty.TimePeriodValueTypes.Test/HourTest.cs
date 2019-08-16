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
using NUnit.Framework;

namespace Cmdty.TimePeriodValueTypes.Test
{
    public sealed partial class HourTest
    {
        // TODO put this into region
        [TestCaseSource(nameof(HourTestItems))]
        public void Deconstruct_ReturnsComponentsUsedToConstruct(HourTestItem testItem)
        {
            var hour = testItem.Create();
            (int year, int monthOfYear, int dayOfMonth, int hourOfDay) = hour;
            Assert.AreEqual(testItem.YearNum, year);
            Assert.AreEqual(testItem.MonthOfYear, monthOfYear);
            Assert.AreEqual(testItem.DayOfMonth, dayOfMonth);
            Assert.AreEqual(testItem.HourOfDay, hourOfDay);
        }

        #region Static Properties

        [Test]
        public void MinHour_Equals1stJanuaryYear1FirstHour()
        {
            var minHour = Hour.MinHour;
            var jan0001FirstHour = new Hour(1, 1, 1, 0);
            Assert.AreEqual(jan0001FirstHour, minHour);
        }

        [Test]
        public void MaxHour_Equals31stDecemberYear9998LastHour()
        {
            var maxHour = Hour.MaxHour;
            var thirtyFirstDec9998LastHour = new Hour(9998, 12, 31, 23);
            Assert.AreEqual(thirtyFirstDec9998LastHour, maxHour);
        }

        #endregion Static Properties

        #region Formatting and Parsing
        // TODO move all of this into t4 generated code?
        [TestCaseSource(nameof(ToStringTestItems))]
        public void ToString_EqualsExpectedResult(HourTestItem<string> hourTestItem)
        {
            var day = hourTestItem.Create();
            var formatted = day.ToString();
            Assert.AreEqual(hourTestItem.ExpectedResult, formatted);
        }

        private static readonly IEnumerable<HourTestItem<string>> ToStringTestItems = new[]
        {
            new HourTestItem<string>
            {
                YearNum = 2018,
                MonthOfYear = 1,
                DayOfMonth = 5,
                HourOfDay = 11,
                ExpectedResult = "2018-01-05 11:00"
            },
            new HourTestItem<string>
            {
                YearNum = 5,
                MonthOfYear = 11,
                DayOfMonth = 20,
                HourOfDay = 0,
                ExpectedResult = "0005-11-20 00:00"
            },
            new HourTestItem<string>
            {
                YearNum = 550,
                MonthOfYear = 12,
                DayOfMonth = 3,
                HourOfDay = 23,
                ExpectedResult = "0550-12-03 23:00"
            }
        };

        [TestCaseSource(nameof(ToStringTestItems))]
        public void Parse_EqualsExpectedResult(HourTestItem<string> hourTestItem)
        {
            var dayFromText = Hour.Parse(hourTestItem.ExpectedResult); // TODO bad use of Expected result?
            var expectedDay = hourTestItem.Create();
            Assert.AreEqual(expectedDay, dayFromText);
        }

        [TestCaseSource(nameof(InvalidHourTextTestItems))]
        public void Parse_WithInvalidText_ThrowsFormatArgumentException(string text)
        {
            Assert.Throws(Is.TypeOf<FormatException>(), () => Hour.Parse(text));
        }

        private static readonly IEnumerable<string> InvalidHourTextTestItems = new[]
        {
			// Bad format in date part
            "2018/01/10 00",
            "2018-02-29 23",
            "20180105 10",
            "2018-02-10 2",
            "2018-02-10 23Z",
            "2018-02-10 x",
			// TODO add more items
        };

        // TODO add unit tests for ToString(string format, IFormatProvider formatProvider) and ToString(string format)

        #endregion Formatting and Parsing

        #region Test Case and Value Sources

        private static readonly IEnumerable<HourTestItem> HourTestItems = new[]
        {
            new HourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 0},
            new HourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 15, HourOfDay = 12},
            new HourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 31, HourOfDay = 23}
            // TODO put more items here
        };

        private static readonly IEnumerable<HourPairTestItem> Hour1EarlierThanHour2 = new[]
        {
            new HourPairTestItem
            {
                HourTestItem1 = new HourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 3},
                HourTestItem2 = new HourTestItem{YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 4}
            }
            // TODO put more items here
        };

        // TODO put this into template?
        private static IEnumerable<HourPairTestItem> Hour1LaterThanHour2 =>
                            Hour1EarlierThanHour2.Select(hourPairTestItem => new HourPairTestItem
                            {
                                HourTestItem2 = hourPairTestItem.HourTestItem1,
                                HourTestItem1 = hourPairTestItem.HourTestItem2
                            });

        // TODO put this into template?
        private static IEnumerable<HourPairTestItem> NonEqualHourPairTestItems =>
                            Hour1EarlierThanHour2.Union(Hour1LaterThanHour2);

        // TODO put this into template?
        private static IEnumerable<HourPairTestItem> EqualHourPairTestItems =>
                                HourTestItems.Select(hourTestItem => new HourPairTestItem
                                {
                                    HourTestItem1 = hourTestItem,
                                    HourTestItem2 = hourTestItem
                                });

        // TODO put this into template?
        private static IEnumerable<HourPairTestItem> Hour1LaterThanOrEqualToHour2 => Hour1LaterThanHour2.Union(EqualHourPairTestItems);

        // TODO put this into template?
        private static IEnumerable<HourPairTestItem> Hour1EarlierThanOrEqualToHour2 => Hour1EarlierThanHour2.Union(EqualHourPairTestItems);

        #endregion Test Case and Value Sources
        
        #region Test Helper Classes

        /// <summary>
        /// Contains data necessary to construct an Hour instance for use in parameterised unit tests.
        /// </summary>
        public class HourTestItem : ITestItem<Hour>
        {
            public int YearNum { get; set; }
            public int MonthOfYear { get; set; }
            public int DayOfMonth { get; set; }
            public int HourOfDay { get; set; }

            public Hour Create() => new Hour(YearNum, MonthOfYear, DayOfMonth, HourOfDay);

            public override string ToString()
            {
                return $"{nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct an Hour instance plus the expected result of an
        /// operation using Hour which returns a single instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class HourTestItem<TResult> : HourTestItem
        {
            public TResult ExpectedResult { get; set; }

            public override string ToString()
            {
                return $"Hour({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay})"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct an Hour instance plus the expected result and parameter value
        /// of an operation using Hour which returns a single instance and uses a single parameter.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class HourTestItem<TParameter, TResult> : HourTestItem<TResult>
        {
            public TParameter ParameterValue { get; set; }
            public string ParameterName { get; set; }

            public override string ToString()
            {
                return $"Hour({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay})"
                       + $", {ParameterName}: {ParameterValue}"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a pair of Hour instances for use in parameterised unit tests.
        /// </summary>
        public class HourPairTestItem : ITestItemPair<Hour>
        {
            public HourTestItem HourTestItem1 { get; set; }
            public HourTestItem HourTestItem2 { get; set; }

            public (Hour timePeriod1, Hour timePeriod2) CreatePair() =>
                (timePeriod1: HourTestItem1.Create(), timePeriod2: HourTestItem2.Create());

            public override string ToString()
            {
                return $"Hour1: ({HourTestItem1}), Hour2: ({HourTestItem2})";
            }
        }

        #endregion Test Helper Classes

    }
}
