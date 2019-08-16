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
    public sealed partial class DayTest
    {
        // TODO put this into region
        [TestCaseSource(nameof(DayTestItems))]
        public void Deconstruct_ReturnsComponentsUsedToConstruct(DayTestItem testItem)
        {
            var day = testItem.Create();
            (int year, int monthOfYear, int dayOfMonth) = day;
            Assert.AreEqual(testItem.YearNum, year);
            Assert.AreEqual(testItem.MonthOfYear, monthOfYear);
            Assert.AreEqual(testItem.DayOfMonth, dayOfMonth);
        }
        
        #region Constructors
        [TestCaseSource(nameof(YearOutOfRangeTestItems))]
        public void Constructor_WithYearParameterOutOfRange_ThrowsArgumentOutOfRangeException(DayTestItem dayTestItem)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => dayTestItem.Create());
        }

        private static readonly IEnumerable<DayTestItem> YearOutOfRangeTestItems = new[]
        {
            // Year too early
            new DayTestItem{YearNum = 0, MonthOfYear = 1, DayOfMonth = 1},
            new DayTestItem{YearNum = -10, MonthOfYear = 4, DayOfMonth = 1},
            new DayTestItem{YearNum = -950, MonthOfYear = 12, DayOfMonth = 1},
            // Year too late
            new DayTestItem{YearNum = 9999, MonthOfYear = 1, DayOfMonth = 1},
            new DayTestItem{YearNum = 10564, MonthOfYear = 6, DayOfMonth = 1},
            new DayTestItem{YearNum = 45669, MonthOfYear = 12, DayOfMonth = 1}
        };

        [TestCaseSource(nameof(MonthOutOfRangeTestItems))]
        public void Constructor_WithMonthParameterOutOfRange_ThrowsArgumentOutOfRangeException(DayTestItem dayTestItem)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => dayTestItem.Create());
        }

        private static readonly IEnumerable<DayTestItem> MonthOutOfRangeTestItems = new[]
        {
            // Month number too low
            new DayTestItem{YearNum = 10, MonthOfYear = 0, DayOfMonth = 1},
            new DayTestItem{YearNum = 950, MonthOfYear = -1, DayOfMonth = 1},
            new DayTestItem{YearNum = 1979, MonthOfYear = -10, DayOfMonth = 1}, 
            // Month number too high
            new DayTestItem{YearNum = 10, MonthOfYear = 13, DayOfMonth = 1},
            new DayTestItem{YearNum = 950, MonthOfYear = 15, DayOfMonth = 1},
            new DayTestItem{YearNum = 1979, MonthOfYear = 250, DayOfMonth = 1}
        };

        [TestCaseSource(nameof(DayOutOfRangeTestItems))]
        public void Constructor_WithDayParameterOutOfRange_ThrowsArgumentOutOfRangeException(DayTestItem dayTestItem)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => dayTestItem.Create());
        }

        private static readonly IEnumerable<DayTestItem> DayOutOfRangeTestItems = new[]
        {
            // Day number too low
            new DayTestItem{YearNum = 10, MonthOfYear = 1, DayOfMonth = 0},
            new DayTestItem{YearNum = 950, MonthOfYear = 1, DayOfMonth = -1},
            new DayTestItem{YearNum = 1979, MonthOfYear = 1, DayOfMonth = -20},
            // Day number too high January
            new DayTestItem{YearNum = 1, MonthOfYear = 1, DayOfMonth = 32},
            new DayTestItem{YearNum = 950, MonthOfYear = 1, DayOfMonth = 32},
            new DayTestItem{YearNum = 1979, MonthOfYear = 1, DayOfMonth = 32},
            // Day number too high February (non-leap year so 28 days)
            new DayTestItem{YearNum = 500, MonthOfYear = 2, DayOfMonth = 29}, // Century year, not divisible by 400
            new DayTestItem{YearNum = 2018, MonthOfYear = 2, DayOfMonth = 29}, // Year not divisible by 4
            new DayTestItem{YearNum = 2019, MonthOfYear = 2, DayOfMonth = 29}, // Year not divisible by 4
            // Day number too high February (leap year so 29 days)
            new DayTestItem{YearNum = 404, MonthOfYear = 2, DayOfMonth = 30}, // Year divisible by 4
            new DayTestItem{YearNum = 1600, MonthOfYear = 2, DayOfMonth = 30}, // Century year, but divisible by 400
            new DayTestItem{YearNum = 2000, MonthOfYear = 2, DayOfMonth = 30}, // Century year, but divisible by 400
            new DayTestItem{YearNum = 2016, MonthOfYear = 2, DayOfMonth = 30}, // Year divisible by 4
            // Day number too high March
            new DayTestItem{YearNum = 1, MonthOfYear = 3, DayOfMonth = 32},
            new DayTestItem{YearNum = 950, MonthOfYear = 3, DayOfMonth = 32},
            new DayTestItem{YearNum = 1979, MonthOfYear = 3, DayOfMonth = 32},
            // Day number too high April
            new DayTestItem{YearNum = 1, MonthOfYear = 4, DayOfMonth = 31},
            new DayTestItem{YearNum = 950, MonthOfYear = 4, DayOfMonth = 31},
            new DayTestItem{YearNum = 1979, MonthOfYear = 4, DayOfMonth = 31},
            // Day number too high May
            new DayTestItem{YearNum = 1, MonthOfYear = 5, DayOfMonth = 32},
            new DayTestItem{YearNum = 950, MonthOfYear = 5, DayOfMonth = 32},
            new DayTestItem{YearNum = 1979, MonthOfYear = 5, DayOfMonth = 32},
            // Day number too high June
            new DayTestItem{YearNum = 1, MonthOfYear = 6, DayOfMonth = 31},
            new DayTestItem{YearNum = 950, MonthOfYear = 6, DayOfMonth = 31},
            new DayTestItem{YearNum = 1979, MonthOfYear = 6, DayOfMonth = 31},
            // Day number too high July
            new DayTestItem{YearNum = 1, MonthOfYear = 7, DayOfMonth = 32},
            new DayTestItem{YearNum = 950, MonthOfYear = 7, DayOfMonth = 32},
            new DayTestItem{YearNum = 1979, MonthOfYear = 7, DayOfMonth = 32},
            // Day number too high August
            new DayTestItem{YearNum = 1, MonthOfYear = 8, DayOfMonth = 32},
            new DayTestItem{YearNum = 950, MonthOfYear = 8, DayOfMonth = 32},
            new DayTestItem{YearNum = 1979, MonthOfYear = 8, DayOfMonth = 32},
            // Day number too high September
            new DayTestItem{YearNum = 1, MonthOfYear = 9, DayOfMonth = 31},
            new DayTestItem{YearNum = 950, MonthOfYear = 9, DayOfMonth = 31},
            new DayTestItem{YearNum = 1979, MonthOfYear = 9, DayOfMonth = 31},
            // Day number too high October
            new DayTestItem{YearNum = 1, MonthOfYear = 10, DayOfMonth = 32},
            new DayTestItem{YearNum = 950, MonthOfYear = 10, DayOfMonth = 32},
            new DayTestItem{YearNum = 1979, MonthOfYear = 10, DayOfMonth = 32},
            // Day number too high November
            new DayTestItem{YearNum = 1, MonthOfYear = 11, DayOfMonth = 31},
            new DayTestItem{YearNum = 950, MonthOfYear = 11, DayOfMonth = 31},
            new DayTestItem{YearNum = 1979, MonthOfYear = 11, DayOfMonth = 31},
            // Day number too high December
            new DayTestItem{YearNum = 1, MonthOfYear = 12, DayOfMonth = 32},
            new DayTestItem{YearNum = 950, MonthOfYear = 12, DayOfMonth = 32},
            new DayTestItem{YearNum = 1979, MonthOfYear = 12, DayOfMonth = 32}
        };

        [Test]
        public void DefaultConstructor_ReturnsInstanceEqualTo1stJan0001()
        {
            var day = new Day();
            var firstJan0001 = new Day(1, 1, 1);
            Assert.AreEqual(firstJan0001, day);
        }

        // TODO further leap year tests:
        // offset and subtraction across leap and non-leap years is correct
        // Constructor on non-leap year is fine for 29th

        #endregion Constructors

        #region Static Factory Methods

        [Test]
        public void FromDateTime_ReturnsDayForYearMonthAndDayOfParameterYearAndMonth(
                        [ValueSource(nameof(DateTimesForFromDateTimeTest))] DateTime dateTime)
        {
            var day = Day.FromDateTime(dateTime);
            var expectedDay = new Day(dateTime.Year, dateTime.Month, dateTime.Day);
            Assert.AreEqual(expectedDay, day);
        }

        private static readonly DateTime[] DateTimesForFromDateTimeTest =
        {
            new DateTime(50, 1, 1),
            new DateTime(2018, 12, 15),
            new DateTime(8569, 12, 31)
        };

        #endregion Static Factory Methods

        #region Instance Properties

        [TestCaseSource(nameof(DayTestItems))]
        public void Start_ShouldEqualDateTimeForStartOfDay(DayTestItem dayTestItem)
        {
            var day = dayTestItem.Create();
            var expectedStart = new DateTime(dayTestItem.YearNum, dayTestItem.MonthOfYear, dayTestItem.DayOfMonth);
            Assert.AreEqual(expectedStart, day.Start);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void End_ShouldEqualDateTimeForStartOfNextDay(DayTestItem dayTestItem)
        {
            var day = dayTestItem.Create();
            var expectedEnd = new DateTime(dayTestItem.YearNum, dayTestItem.MonthOfYear, dayTestItem.DayOfMonth).AddDays(1);
            Assert.AreEqual(expectedEnd, day.End);
        }

        [Test]
        public void Start_ForMinDay_EqualsFirstOfJanYear1()
        {
            // Checking that Start doesn't do anything unwanted at the boundaries
            var minDay = Day.MinDay;
            var firstOfJan0001 = new DateTime(1, 1, 1);
            Assert.AreEqual(firstOfJan0001, minDay.Start);
        }

        [Test]
        public void End_ForMaxDay_EqualsFirstOfJanYear9999()
        {
            // Checking that End doesn't do anything unwanted at the boundaries
            var maxDay = Day.MaxDay;
            var firstOfJan9999 = new DateTime(9999, 1, 1);
            Assert.AreEqual(firstOfJan9999, maxDay.End);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void YearProperty_EqualsYearUsedToConstruct(DayTestItem dayTestItem)
        {
            var day = dayTestItem.Create();
            Assert.AreEqual(dayTestItem.YearNum, day.Year);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void MonthOfYearProperty_EqualsMonthOfYearUsedToConstruct(DayTestItem dayTestItem)
        {
            var day = dayTestItem.Create();
            Assert.AreEqual(dayTestItem.MonthOfYear, day.MonthOfYear);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void DayOfYearProperty_EqualsDayUsedToConstruct(DayTestItem dayTestItem)
        {
            var day = dayTestItem.Create();
            Assert.AreEqual(dayTestItem.DayOfMonth, day.DayOfMonth);
        }

        #endregion Instance Properties

        #region Static Properties

        [Test]
        public void MinDay_Equals1stJanuaryYear1()
        {
            var minDay = Day.MinDay;
            var firstJan0001 = new Day(1, 1, 1);
            Assert.AreEqual(firstJan0001, minDay);
        }

        [Test]
        public void MaxDay_Equals31stDecemberYear9998()
        {
            var maxDay = Day.MaxDay;
            var thirtyFirstDec9998 = new Day(9998, 12, 31);
            Assert.AreEqual(thirtyFirstDec9998, maxDay);
        }

        #endregion Static Properties

        #region Time Period Arithmetic

        // TODO finish these, based on MonthTest
        //[TestCaseSource(nameof(OffsetTestItems))]
        //public void Offset_ReturnsExpectedResult(DayTestItem<int, Day> dayTestItem)
        //{
        //    var startDay = dayTestItem.Create();
        //    var offsetDay = startDay.Offset(dayTestItem.ParameterValue);
        //    Assert.AreEqual(dayTestItem.ExpectedResult, offsetDay);
        //}

        //private static readonly IEnumerable<DayTestItem<int, Day>> OffsetTestItems = new[]
        //{
        //    new DayTestItem<int, Day>
        //    {
        //        MonthOfYear = 1,
        //        YearNum = 5,
        //        ParameterName = "Offset",
        //        ParameterValue = 1,
        //        ExpectedResult = new Day(5, 2)
        //    },
        //    new DayTestItem<int, Day>
        //    {
        //        MonthOfYear = 6,
        //        YearNum = 1979,
        //        ParameterName = "Offset",
        //        ParameterValue = 10,
        //        ExpectedResult = new Day(1980, 4)
        //    },
        //    new DayTestItem<int, Day>
        //    {
        //        MonthOfYear = 6,
        //        YearNum = 1979,
        //        ParameterName = "Offset",
        //        ParameterValue = -7,
        //        ExpectedResult = new Day(1978, 11)
        //    }
        //};

        // TODO check addition and subtraction with too large numbers results in ArgumentOutOfRangeExceptions

        [TestCaseSource(nameof(PlusPlusTestItems))]
        public void PlusPlusOperator_EqualsNextDay(DayPairTestItem dayPairTestItem)
        {
            var (day, nextDay) = dayPairTestItem.CreatePair();
            day++;
            Assert.AreEqual(nextDay, day);
        }

        private static readonly IEnumerable<DayPairTestItem> PlusPlusTestItems = new[]
        {
            new DayPairTestItem
            {
                DayTestItem1 = new DayTestItem {YearNum = 2018, MonthOfYear = 12, DayOfMonth = 31},
                DayTestItem2 = new DayTestItem {YearNum = 2019, MonthOfYear = 1, DayOfMonth = 1}
            },
            new DayPairTestItem
            {
                DayTestItem1 = new DayTestItem {YearNum = 10, MonthOfYear = 2, DayOfMonth = 15},
                DayTestItem2 = new DayTestItem {YearNum = 10, MonthOfYear = 2, DayOfMonth = 16}
            },
            new DayPairTestItem
            {
                DayTestItem1 = new DayTestItem {YearNum = 9950, MonthOfYear = 5, DayOfMonth = 31},
                DayTestItem2 = new DayTestItem {YearNum = 9950, MonthOfYear = 6, DayOfMonth = 1}
            }
        };

        // TODO eliminate either PlusPlusTestItems or MinusMinusTestItems and just reverse the other?

        [TestCaseSource(nameof(MinusMinusTestItems))]
        public void MinusMinusOperator_EqualsPreviousDay(DayPairTestItem dayPairTestItem)
        {
            var (day, previousDay) = dayPairTestItem.CreatePair();
            day--;
            Assert.AreEqual(previousDay, day);
        }

        private static readonly IEnumerable<DayPairTestItem> MinusMinusTestItems = new[]
        {
            new DayPairTestItem
            {
                DayTestItem1 = new DayTestItem {YearNum = 2019, MonthOfYear = 1, DayOfMonth = 1},
                DayTestItem2 = new DayTestItem {YearNum = 2018, MonthOfYear = 12, DayOfMonth = 31}
            },
            new DayPairTestItem
            {
                DayTestItem1 = new DayTestItem {YearNum = 10, MonthOfYear = 2, DayOfMonth = 16},
                DayTestItem2 = new DayTestItem {YearNum = 10, MonthOfYear = 2, DayOfMonth = 15}
            },
            new DayPairTestItem
            {
                DayTestItem1 = new DayTestItem {YearNum = 9950, MonthOfYear = 6, DayOfMonth = 1},
                DayTestItem2 = new DayTestItem {YearNum = 9950, MonthOfYear = 5, DayOfMonth = 31}
            }
        };

        #endregion Time Period Arithmetic

        #region Formatting and Parsing
        // TODO move all of this into t4 generated code?
        [TestCaseSource(nameof(ToStringTestItems))]
        public void ToString_EqualsExpectedResult(DayTestItem<string> dayTestItem)
        {
            var day = dayTestItem.Create();
            var formatted = day.ToString();
            Assert.AreEqual(dayTestItem.ExpectedResult, formatted);
        }

        private static readonly IEnumerable<DayTestItem<string>> ToStringTestItems = new[]
        {
            new DayTestItem<string>
            {
                YearNum = 2018,
                MonthOfYear = 1,
                DayOfMonth = 5,
                ExpectedResult = "2018-01-05"
            },
            new DayTestItem<string>
            {
                YearNum = 5,
                MonthOfYear = 11,
                DayOfMonth = 20,
                ExpectedResult = "0005-11-20"
            },
            new DayTestItem<string>
            {
                YearNum = 550,
                MonthOfYear = 12,
                DayOfMonth = 3,
                ExpectedResult = "0550-12-03"
            }
        };

        [TestCaseSource(nameof(ToStringTestItems))]
        public void Parse_EqualsExpectedResult(DayTestItem<string> dayTestItem)
        {
            var dayFromText = Day.Parse(dayTestItem.ExpectedResult); // TODO bad use of Expected result?
            var expectedDay = dayTestItem.Create();
            Assert.AreEqual(expectedDay, dayFromText);
        }

        [TestCaseSource(nameof(InvalidDayTextTestItems))]
        public void Parse_WithInvalidText_ThrowsArgumentException(string text)
        {
            Assert.Throws(Is.TypeOf<FormatException>().And
                .Message.EqualTo("String was not recognized as a valid Day."), () => Day.Parse(text));
        }

        private static readonly IEnumerable<string> InvalidDayTextTestItems = new []
        {
            "2018/01/10",
            "2018-02-29",
            "20180105"
        };

        #endregion Formatting and Parsing

        #region Test Case and Value Sources

        private static readonly IEnumerable<DayTestItem> DayTestItems = new[]
        {
            new DayTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1},
            new DayTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 15},
            new DayTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 31},
            // TODO put more items here
        };

        private static readonly IEnumerable<DayPairTestItem> Day1EarlierThanDay2 = new[]
        {
            new DayPairTestItem
            {
                DayTestItem1 = new DayTestItem {YearNum = 10, MonthOfYear = 1, DayOfMonth = 1},
                DayTestItem2 = new DayTestItem{YearNum = 10, MonthOfYear = 1, DayOfMonth = 2}
            }
            // TODO put more items here
        };

        // TODO put this into template?
        private static IEnumerable<DayPairTestItem> Day1LaterThanDay2 =>
                            Day1EarlierThanDay2.Select(dayPairTestItem => new DayPairTestItem
                            {
                                DayTestItem2 = dayPairTestItem.DayTestItem1,
                                DayTestItem1 = dayPairTestItem.DayTestItem2
                            });

        // TODO put this into template?
        private static IEnumerable<DayPairTestItem> NonEqualDayPairTestItems =>
                            Day1EarlierThanDay2.Union(Day1LaterThanDay2);

        // TODO put this into template?
        private static IEnumerable<DayPairTestItem> EqualDayPairTestItems =>
                                DayTestItems.Select(monthTestItem => new DayPairTestItem
                                {
                                    DayTestItem1 = monthTestItem,
                                    DayTestItem2 = monthTestItem
                                });
        
        // TODO put this into template?
        private static IEnumerable<DayPairTestItem> Day1LaterThanOrEqualToDay2 =>
                                Day1LaterThanDay2.Union(EqualDayPairTestItems);

        // TODO put this into template?
        private static IEnumerable<DayPairTestItem> Day1EarlierThanOrEqualToDay2 =>
            Day1EarlierThanDay2.Union(EqualDayPairTestItems);

        #endregion Test Case and Value Sources
        
        #region Test Helper Classes

        /// <summary>
        /// Contains data necessary to construct a Day instance for use in parameterised unit tests.
        /// </summary>
        public class DayTestItem : ITestItem<Day>
        {
            public int YearNum { get; set; }
            public int MonthOfYear { get; set; }
            public int DayOfMonth { get; set; }

            public Day Create() => new Day(YearNum, MonthOfYear, DayOfMonth);

            public override string ToString()
            {
                return $"{nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Day instance plus the expected result of an
        /// operation using Day which returns a single instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class DayTestItem<TResult> : DayTestItem
        {
            public TResult ExpectedResult { get; set; }

            public override string ToString()
            {
                return $"Month({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth})"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Day instance plus the expected result and parameter value
        /// of an operation using Day which returns a single instance and uses a single parameter.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class DayTestItem<TParameter, TResult> : DayTestItem<TResult>
        {
            public TParameter ParameterValue { get; set; }
            public string ParameterName { get; set; }

            public override string ToString()
            {
                return $"Day({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}, {nameof(DayOfMonth)}: {DayOfMonth})"
                       + $", {ParameterName}: {ParameterValue}"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a pair of Day instances for use in parameterised unit tests.
        /// </summary>
        public class DayPairTestItem : ITestItemPair<Day>
        {
            public DayTestItem DayTestItem1 { get; set; }
            public DayTestItem DayTestItem2 { get; set; }

            public (Day timePeriod1, Day timePeriod2) CreatePair() =>
                (timePeriod1: DayTestItem1.Create(), timePeriod2: DayTestItem2.Create());

            public override string ToString()
            {
                return $"Day1: ({DayTestItem1}), Day2: ({DayTestItem2})";
            }
        }

        #endregion Test Helper Classes

    }
}
