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
    public sealed partial class QuarterHourTest
    {

        #region Static Properties

        [Test]
        public void MinQuarterHour_Equals1stJanuaryYear1FirstQuarterHour()
        {
            var minQuarterHour = QuarterHour.MinQuarterHour;
            var jan0001FirstQuarterHour = new QuarterHour(1, 1, 1, 0, 0);
            Assert.AreEqual(jan0001FirstQuarterHour, minQuarterHour);
        }

        [Test]
        public void MaxQuarterHour_Equals31stDecemberYear9998LastQuarterHour()
        {
            var maxQuarterHour = QuarterHour.MaxQuarterHour;
            var thirtyFirstDec9998LastQuaterHour = new QuarterHour(9998, 12, 31, 23, 45);
            Assert.AreEqual(thirtyFirstDec9998LastQuaterHour, maxQuarterHour);
        }

        #endregion Static Properties

        #region Formatting and Parsing
        // TODO move all of this into t4 generated code?
        [TestCaseSource(nameof(ToStringTestItems))]
        public void ToString_EqualsExpectedResult(QuarterHourTestItem<string> testItem)
        {
            var quarterHour = testItem.Create();
            var formatted = quarterHour.ToString();
            Assert.AreEqual(testItem.ExpectedResult, formatted);
        }

        private static readonly IEnumerable<QuarterHourTestItem<string>> ToStringTestItems = new[]
        {
            new QuarterHourTestItem<string>
            {
                YearNum = 2018,
                MonthOfYear = 1,
                DayOfMonth = 5,
                HourOfDay = 11,
                MinuteOfDay = 0,
                ExpectedResult = "2018-01-05 11:00"
            },
            new QuarterHourTestItem<string>
            {
                YearNum = 2018,
                MonthOfYear = 1,
                DayOfMonth = 5,
                HourOfDay = 11,
                MinuteOfDay = 15,
                ExpectedResult = "2018-01-05 11:15"
            },
            new QuarterHourTestItem<string>
            {
                YearNum = 5,
                MonthOfYear = 11,
                DayOfMonth = 20,
                HourOfDay = 0,
                MinuteOfDay = 30,
                ExpectedResult = "0005-11-20 00:30"
            },
            new QuarterHourTestItem<string>
            {
                YearNum = 550,
                MonthOfYear = 12,
                DayOfMonth = 3,
                HourOfDay = 23,
                MinuteOfDay = 30,
                ExpectedResult = "0550-12-03 23:30"
            },
            new QuarterHourTestItem<string>
            {
                YearNum = 550,
                MonthOfYear = 12,
                DayOfMonth = 3,
                HourOfDay = 23,
                MinuteOfDay = 45,
                ExpectedResult = "0550-12-03 23:45"
            }
        };

        [TestCaseSource(nameof(ToStringTestItems))]
        public void Parse_EqualsExpectedResult(QuarterHourTestItem<string> testItem)
        {
            var quarterHourFromText = QuarterHour.Parse(testItem.ExpectedResult); // TODO bad use of Expected result?
            var expectedQuarterHour = testItem.Create();
            Assert.AreEqual(expectedQuarterHour, quarterHourFromText);
        }

        [TestCaseSource(nameof(InvalidQuarterHourTextTestItems))]
        public void Parse_WithInvalidText_ThrowsFormatArgumentException(string text)
        {
            Assert.Throws(Is.TypeOf<FormatException>(), () => QuarterHour.Parse(text));
        }

        private static readonly IEnumerable<string> InvalidQuarterHourTextTestItems = new[]
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

        private static readonly IEnumerable<QuarterHourTestItem> QuarterHourTestItems = new[]
        {
            new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 0, MinuteOfDay = 0},
            new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 15, HourOfDay = 12, MinuteOfDay = 15},
            new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 31, HourOfDay = 23, MinuteOfDay = 30},
            new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 0, MinuteOfDay = 15},
            new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 15, HourOfDay = 12, MinuteOfDay = 30},
            new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 31, HourOfDay = 23, MinuteOfDay = 45},
            // TODO put more items here
        };

        private static readonly IEnumerable<QuarterHourPairTestItem> QuarterHour1EarlierThanQuarterHour2 = new[]
        {
            new QuarterHourPairTestItem
            {
                QuarterHourTestItem1 = new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 3, MinuteOfDay = 0},
                QuarterHourTestItem2 = new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 3, MinuteOfDay = 15},
            },

            new QuarterHourPairTestItem
            {
                QuarterHourTestItem1 = new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 3, MinuteOfDay = 15},
                QuarterHourTestItem2 = new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 3, MinuteOfDay = 30},
            },
            new QuarterHourPairTestItem
            {
                QuarterHourTestItem1 = new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 4, MinuteOfDay = 30},
                QuarterHourTestItem2 = new QuarterHourTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1, HourOfDay = 4, MinuteOfDay = 45},
            },
            // TODO put more items here
        };

        // TODO put this into template?
        private static IEnumerable<QuarterHourPairTestItem> QuarterHour1LaterThanQuarterHour2 =>
                            QuarterHour1EarlierThanQuarterHour2.Select(halfHourPairTestItem => new QuarterHourPairTestItem
                            {
                                QuarterHourTestItem2 = halfHourPairTestItem.QuarterHourTestItem1,
                                QuarterHourTestItem1 = halfHourPairTestItem.QuarterHourTestItem2
                            });

        // TODO put this into template?
        private static IEnumerable<QuarterHourPairTestItem> NonEqualQuarterHourPairTestItems =>
                            QuarterHour1EarlierThanQuarterHour2.Union(QuarterHour1LaterThanQuarterHour2);

        // TODO put this into template?
        private static IEnumerable<QuarterHourPairTestItem> EqualQuarterHourPairTestItems =>
                                QuarterHourTestItems.Select(halfHourTestItem => new QuarterHourPairTestItem
                                {
                                    QuarterHourTestItem1 = halfHourTestItem,
                                    QuarterHourTestItem2 = halfHourTestItem
                                });

        // TODO put this into template?
        private static IEnumerable<QuarterHourPairTestItem> QuarterHour1LaterThanOrEqualToQuarterHour2 => QuarterHour1LaterThanQuarterHour2.Union(EqualQuarterHourPairTestItems);

        // TODO put this into template?
        private static IEnumerable<QuarterHourPairTestItem> QuarterHour1EarlierThanOrEqualToQuarterHour2 => QuarterHour1EarlierThanQuarterHour2.Union(EqualQuarterHourPairTestItems);

        #endregion Test Case and Value Sources

        #region Test Helper Classes

        /// <summary>
        /// Contains data necessary to construct a QuarterHour instance for use in parameterised unit tests.
        /// </summary>
        public class QuarterHourTestItem : ITestItem<QuarterHour>
        {
            public int YearNum { get; set; }
            public int MonthOfYear { get; set; }
            public int DayOfMonth { get; set; }
            public int HourOfDay { get; set; }
            public int MinuteOfDay { get; set; }

            public QuarterHour Create() => new QuarterHour(YearNum, MonthOfYear, DayOfMonth, HourOfDay, MinuteOfDay);

            public override string ToString()
            {
                return $"{nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay}, {nameof(MinuteOfDay)}: {MinuteOfDay}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a QuarterHour instance plus the expected result of an
        /// operation using QuarterHour which returns a single instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class QuarterHourTestItem<TResult> : QuarterHourTestItem
        {
            public TResult ExpectedResult { get; set; }

            public override string ToString()
            {
                return $"QuarterHour({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay}, {nameof(MinuteOfDay)}: {MinuteOfDay})"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a QuarterHour instance plus the expected result and parameter value
        /// of an operation using QuarterHour which returns a single instance and uses a single parameter.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class QuarterHourTestItem<TParameter, TResult> : QuarterHourTestItem<TResult>
        {
            public TParameter ParameterValue { get; set; }
            public string ParameterName { get; set; }

            public override string ToString()
            {
                return $"QuarterHour({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}, {nameof(HourOfDay)}: {HourOfDay}, {nameof(MinuteOfDay)}: {MinuteOfDay})"
                       + $", {ParameterName}: {ParameterValue}"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a pair of QuarterHour instances for use in parameterised unit tests.
        /// </summary>
        public class QuarterHourPairTestItem : ITestItemPair<QuarterHour>
        {
            public QuarterHourTestItem QuarterHourTestItem1 { get; set; }
            public QuarterHourTestItem QuarterHourTestItem2 { get; set; }

            public (QuarterHour timePeriod1, QuarterHour timePeriod2) CreatePair() =>
                (timePeriod1: QuarterHourTestItem1.Create(), timePeriod2: QuarterHourTestItem2.Create());

            public override string ToString()
            {
                return $"QuarterHour1: ({QuarterHourTestItem1}), QuarterHour2: ({QuarterHourTestItem2})";
            }
        }

        #endregion Test Helper Classes

    }
}
