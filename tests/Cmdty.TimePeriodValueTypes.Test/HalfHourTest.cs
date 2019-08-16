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
    public sealed partial class HalfHourTest
    {

        // TODO put this into region
        [TestCaseSource(nameof(HalfHourTestItems))]
        public void Deconstruct_ReturnsComponentsUsedToConstruct(HalfHourTestItem testItem)
        {
            var hour = testItem.Create();
            (int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfDay) = hour;
            Assert.AreEqual(testItem.YearNum, year);
            Assert.AreEqual(testItem.MonthOfYear, monthOfYear);
            Assert.AreEqual(testItem.DayOfMonth, dayOfMonth);
            Assert.AreEqual(testItem.HourOfDay, hourOfDay);
            Assert.AreEqual(testItem.MinuteOfDay, minuteOfDay);
        }

        #region Static Properties

        [Test]
        public void MinHalfHour_Equals1stJanuaryYear1FirstHalfHour()
        {
            var minHalfHour = HalfHour.MinHalfHour;
            var jan0001FirstHalfHour = new HalfHour(1, 1, 1, 0, 0);
            Assert.AreEqual(jan0001FirstHalfHour, minHalfHour);
        }

        [Test]
        public void MaxHalfHour_Equals31stDecemberYear9998LastHalfHour()
        {
            var maxHalfHour = HalfHour.MaxHalfHour;
            var thirtyFirstDec9998LastHalfHour = new HalfHour(9998, 12, 31, 23, 30);
            Assert.AreEqual(thirtyFirstDec9998LastHalfHour, maxHalfHour);
        }

        #endregion Static Properties

        #region Formatting and Parsing
        // TODO move all of this into t4 generated code?
        [TestCaseSource(nameof(ToStringTestItems))]
        public void ToString_EqualsExpectedResult(HalfHourTestItem<string> testItem)
        {
            var halfHour = testItem.Create();
            var formatted = halfHour.ToString();
            Assert.AreEqual(testItem.ExpectedResult, formatted);
        }

        private static readonly IEnumerable<HalfHourTestItem<string>> ToStringTestItems = new[]
        {
            new HalfHourTestItem<string>
            {
                YearNum = 2018,
                MonthOfYear = 1,
                DayOfMonth = 5,
                HourOfDay = 11,
                MinuteOfDay = 0,
                ExpectedResult = "2018-01-05 11:00"
            },
            new HalfHourTestItem<string>
            {
                YearNum = 5,
                MonthOfYear = 11,
                DayOfMonth = 20,
                HourOfDay = 0,
                MinuteOfDay = 30,
                ExpectedResult = "0005-11-20 00:30"
            },
            new HalfHourTestItem<string>
            {
                YearNum = 550,
                MonthOfYear = 12,
                DayOfMonth = 3,
                HourOfDay = 23,
                MinuteOfDay = 30,
                ExpectedResult = "0550-12-03 23:30"
            }
        };

        [TestCaseSource(nameof(ToStringTestItems))]
        public void Parse_EqualsExpectedResult(HalfHourTestItem<string> testItem)
        {
            var halfHourFromText = HalfHour.Parse(testItem.ExpectedResult); // TODO bad use of Expected result?
            var expectedHalfHour = testItem.Create();
            Assert.AreEqual(expectedHalfHour, halfHourFromText);
        }

        [TestCaseSource(nameof(InvalidHalfHourTextTestItems))]
        public void Parse_WithInvalidText_ThrowsFormatArgumentException(string text)
        {
            Assert.Throws(Is.TypeOf<FormatException>(), () => HalfHour.Parse(text));
        }

        private static readonly IEnumerable<string> InvalidHalfHourTextTestItems = new[]
        {
            "2018/01/10 :00",
            "2018-02-29 23:300",
            "20180105 10:00",
            "2018-02-10T2:30",
            "2018-02-10 23:050Z",
            "2018-02-10 x",
            "2019-05-06 20:000"
            // TODO add more items
        };

        #endregion Formatting and Parsing

        #region Test Case and Value Sources

        private static readonly IEnumerable<HalfHourTestItem> HalfHourTestItems = new[]
        {
            new HalfHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 0, MinuteOfDay = 0},
            new HalfHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 15, HourOfDay = 12, MinuteOfDay = 30},
            new HalfHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 31, HourOfDay = 23, MinuteOfDay = 0}
            // TODO put more items here
        };

        private static readonly IEnumerable<HalfHourPairTestItem> HalfHour1EarlierThanHalfHour2 = new[]
        {
            new HalfHourPairTestItem
            {
                HalfHourTestItem1 = new HalfHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 3, MinuteOfDay = 0},
                HalfHourTestItem2 = new HalfHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 4, MinuteOfDay = 30}
            }
            // TODO put more items here
        };

        // TODO put this into template?
        private static IEnumerable<HalfHourPairTestItem> HalfHour1LaterThanHalfHour2 =>
                            HalfHour1EarlierThanHalfHour2.Select(halfHourPairTestItem => new HalfHourPairTestItem
                            {
                                HalfHourTestItem2 = halfHourPairTestItem.HalfHourTestItem1,
                                HalfHourTestItem1 = halfHourPairTestItem.HalfHourTestItem2
                            });

        // TODO put this into template?
        private static IEnumerable<HalfHourPairTestItem> NonEqualHalfHourPairTestItems =>
                            HalfHour1EarlierThanHalfHour2.Union(HalfHour1LaterThanHalfHour2);

        // TODO put this into template?
        private static IEnumerable<HalfHourPairTestItem> EqualHalfHourPairTestItems =>
                                HalfHourTestItems.Select(halfHourTestItem => new HalfHourPairTestItem
                                {
                                    HalfHourTestItem1 = halfHourTestItem,
                                    HalfHourTestItem2 = halfHourTestItem
                                });

        // TODO put this into template?
        private static IEnumerable<HalfHourPairTestItem> HalfHour1LaterThanOrEqualToHalfHour2 => HalfHour1LaterThanHalfHour2.Union(EqualHalfHourPairTestItems);

        // TODO put this into template?
        private static IEnumerable<HalfHourPairTestItem> HalfHour1EarlierThanOrEqualToHalfHour2 => HalfHour1EarlierThanHalfHour2.Union(EqualHalfHourPairTestItems);

        #endregion Test Case and Value Sources
        
        #region Test Helper Classes

        /// <summary>
        /// Contains data necessary to construct a HalfHour instance for use in parameterised unit tests.
        /// </summary>
        public class HalfHourTestItem : ITestItem<HalfHour>
        {
            public int YearNum { get; set; }
            public int MonthOfYear { get; set; }
            public int DayOfMonth { get; set; }
            public int HourOfDay { get; set; }
            public int MinuteOfDay { get; set; }

            public HalfHour Create() => new HalfHour(YearNum, MonthOfYear, DayOfMonth, HourOfDay, MinuteOfDay);

            public override string ToString()
            {
                return $"{nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay}, {nameof(MinuteOfDay)}: {MinuteOfDay}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a HalfHour instance plus the expected result of an
        /// operation using HalfHour which returns a single instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class HalfHourTestItem<TResult> : HalfHourTestItem
        {
            public TResult ExpectedResult { get; set; }

            public override string ToString()
            {
                return $"HalfHour({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay}, {nameof(MinuteOfDay)}: {MinuteOfDay})"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a HalfHour instance plus the expected result and parameter value
        /// of an operation using HalfHour which returns a single instance and uses a single parameter.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class HalfHourTestItem<TParameter, TResult> : HalfHourTestItem<TResult>
        {
            public TParameter ParameterValue { get; set; }
            public string ParameterName { get; set; }

            public override string ToString()
            {
                return $"HalfHour({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay}, {nameof(MinuteOfDay)}: {MinuteOfDay})"
                       + $", {ParameterName}: {ParameterValue}"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a pair of HalfHour instances for use in parameterised unit tests.
        /// </summary>
        public class HalfHourPairTestItem : ITestItemPair<HalfHour>
        {
            public HalfHourTestItem HalfHourTestItem1 { get; set; }
            public HalfHourTestItem HalfHourTestItem2 { get; set; }

            public (HalfHour timePeriod1, HalfHour timePeriod2) CreatePair() =>
                (timePeriod1: HalfHourTestItem1.Create(), timePeriod2: HalfHourTestItem2.Create());

            public override string ToString()
            {
                return $"HalfHour1: ({HalfHourTestItem1}), HalfHour2: ({HalfHourTestItem2})";
            }
        }

        #endregion Test Helper Classes

    }
}
