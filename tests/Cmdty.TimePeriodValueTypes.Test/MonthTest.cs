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
    public sealed partial class MonthTest
    {
        // TODO put this into region
        [TestCaseSource(nameof(MonthTestItems))]
        public void Deconstruct_ReturnsComponentsUsedToConstruct(MonthTestItem testItem)
        {
            var month = testItem.Create();
            (int year, int monthOfYear) = month;
            Assert.AreEqual(testItem.YearNum, year);
            Assert.AreEqual(testItem.MonthOfYear, monthOfYear);
        }
        
        #region Constructors
        [TestCaseSource(nameof(YearOutOfRangeTestItems))]
        public void Constructor_WithYearParameterOutOfRange_ThrowsArgumentOutOfRangeException(MonthTestItem month)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => month.Create());
        }

        private static readonly IEnumerable<MonthTestItem> YearOutOfRangeTestItems = new[]
        {
            // Year too early
            new MonthTestItem{YearNum = 0, MonthOfYear = 1},
            new MonthTestItem{YearNum = -10, MonthOfYear = 4},
            new MonthTestItem{YearNum = -950, MonthOfYear = 12}, 
            // Year too late
            new MonthTestItem{YearNum = 9999, MonthOfYear = 1},
            new MonthTestItem{YearNum = 10564, MonthOfYear = 6},
            new MonthTestItem{YearNum = 45669, MonthOfYear = 12},
        };

        [TestCaseSource(nameof(MonthOutOfRangeTestItems))]
        public void Constructor_WithMonthParameterOutOfRange_ThrowsArgumentOutOfRangeException(MonthTestItem month)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => month.Create());
        }

        private static readonly IEnumerable<MonthTestItem> MonthOutOfRangeTestItems = new[]
        {
            // Month number too low
            new MonthTestItem{YearNum = 10, MonthOfYear = 0},
            new MonthTestItem{YearNum = 950, MonthOfYear = -1},
            new MonthTestItem{YearNum = 1979, MonthOfYear = -10}, 
            // Month number too high
            new MonthTestItem{YearNum = 10, MonthOfYear = 13},
            new MonthTestItem{YearNum = 950, MonthOfYear = 15},
            new MonthTestItem{YearNum = 1979, MonthOfYear = 250},
        };

        [Test]
        public void DefaultConstructor_ReturnsInstanceEqualToJan0001()
        {
            var month = new Month();
            var jan0001 = new Month(1, 1);
            Assert.AreEqual(jan0001, month);
        }

        #endregion Constructors

        #region Static Factory Methods

        [Test]
        public void FromDateTime_ReturnsMonthForYearAndMonthOfParameterYearAndMonth(
                            [ValueSource(nameof(DateTimesForFromDateTimeTest))] DateTime dateTime)
        {
            var month = Month.FromDateTime(dateTime);
            var expectedMonth = new Month(dateTime.Year, dateTime.Month);
            Assert.AreEqual(expectedMonth, month);
        }

        private static readonly DateTime[] DateTimesForFromDateTimeTest =
        {
            new DateTime(50, 1, 1),
            new DateTime(2018, 12, 15),
            new DateTime(8569, 12, 31)
        };

        [Test]
        public void CreateJanuary_ReturnsJanuaryForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateJanuary(year);
            var expected = new Month(year, 1);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateFebruary_ReturnsFebruaryForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateFebruary(year);
            var expected = new Month(year, 2);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateMarch_ReturnsMarchForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateMarch(year);
            var expected = new Month(year, 3);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateApril_ReturnsAprilForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateApril(year);
            var expected = new Month(year, 4);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateMay_ReturnsMayForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateMay(year);
            var expected = new Month(year, 5);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateJune_ReturnsJuneForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateJune(year);
            var expected = new Month(year, 6);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateJuly_ReturnsJulyForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateJuly(year);
            var expected = new Month(year, 7);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateAugust_ReturnsAugustForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateAugust(year);
            var expected = new Month(year, 8);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateSeptember_ReturnsSeptemberForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateSeptember(year);
            var expected = new Month(year, 9);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateOctober_ReturnsOctoberForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateOctober(year);
            var expected = new Month(year, 10);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateNovember_ReturnsNovemberForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateNovember(year);
            var expected = new Month(year, 11);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateDecember_ReturnsDecemberForYearParameter(
                        [ValueSource(nameof(YearsForMonthFactoryTests))] int year)
        {
            var actual = Month.CreateDecember(year);
            var expected = new Month(year, 12);
            Assert.AreEqual(expected, actual);
        }
        
        private static readonly int[] YearsForMonthFactoryTests = 
        {
            10,
            1016,
            2018,
            5583
        };

        #endregion Static Factory Methods

        #region Instance Properties

        [TestCaseSource(nameof(MonthTestItems))]
        public void Start_ShouldEqualDateTimeForStartOfMonth(MonthTestItem monthTestItem)
        {
            var month = monthTestItem.Create();
            var expectedStart = new DateTime(monthTestItem.YearNum, monthTestItem.MonthOfYear, 1);
            Assert.AreEqual(expectedStart, month.Start);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void End_ShouldEqualDateTimeForStartOfNextMonth(MonthTestItem monthTestItem)
        {
            var month = monthTestItem.Create();
            var expectedEnd = new DateTime(monthTestItem.YearNum, monthTestItem.MonthOfYear, 1).AddMonths(1);
            Assert.AreEqual(expectedEnd, month.End);
        }

        [Test]
        public void Start_ForMinMonth_EqualsFirstOfJanYear1()
        {
            // Checking that Start doesn't do anything unwanted at the boundaries
            var minMonth = Month.MinMonth;
            var firstOfJan0001 = new DateTime(1, 1, 1);
            Assert.AreEqual(firstOfJan0001, minMonth.Start);
        }

        [Test]
        public void End_ForMaxMonth_EqualsFirstOfJanYear9999()
        {
            // Checking that End doesn't do anything unwanted at the boundaries
            var maxMonth = Month.MaxMonth;
            var firstOfJan9999 = new DateTime(9999, 1, 1);
            Assert.AreEqual(firstOfJan9999, maxMonth.End);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void YearProperty_EqualsYearUsedToConstruct(MonthTestItem monthTestItem)
        {
            var month = monthTestItem.Create();
            Assert.AreEqual(monthTestItem.YearNum, month.Year);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void MonthOfYearProperty_EqualsMonthUsedToConstruct(MonthTestItem monthTestItem)
        {
            var month = monthTestItem.Create();
            Assert.AreEqual(monthTestItem.MonthOfYear, month.MonthOfYear);
        }

        #endregion Instance Properties

        #region Static Properties

        [Test]
        public void MinMonth_EqualsJanuaryYear1()
        {
            var minMonth = Month.MinMonth;
            var jan0001 = new Month(1, 1);
            Assert.AreEqual(jan0001, minMonth);
        }

        [Test]
        public void MaxMonth_EqualsDecemberYear9998()
        {
            var maxMonth = Month.MaxMonth;
            var dec9998 = new Month(9998, 12);
            Assert.AreEqual(dec9998, maxMonth);
        }

        #endregion Static Properties
        
        #region Time Period Arithmetic

        [TestCaseSource(nameof(OffsetTestItems))]
        public void Offset_ReturnsExpectedResult(MonthTestItem<int, Month> monthTestItem)
        {
            var startMonth = monthTestItem.Create();
            var offsetMonth = startMonth.Offset(monthTestItem.ParameterValue);
            Assert.AreEqual(monthTestItem.ExpectedResult, offsetMonth);
        }

        private static readonly IEnumerable<MonthTestItem<int, Month>> OffsetTestItems = new[]
        {
            new MonthTestItem<int, Month>
            {
                MonthOfYear = 1,
                YearNum = 5,
                ParameterName = "Offset",
                ParameterValue = 1,
                ExpectedResult = new Month(5, 2)
            },
            new MonthTestItem<int, Month>
            {
                MonthOfYear = 6,
                YearNum = 1979,
                ParameterName = "Offset",
                ParameterValue = 10,
                ExpectedResult = new Month(1980, 4)
            },
            new MonthTestItem<int, Month>
            {
                MonthOfYear = 6,
                YearNum = 1979,
                ParameterName = "Offset",
                ParameterValue = -7,
                ExpectedResult = new Month(1978, 11)
            }
        };

        [TestCaseSource(nameof(OffsetFromTestItems))]
        public void OffsetFrom_ReturnsExpectedResult(MonthTestItem<Month, int> monthTestItem)
        {
            var month1 = monthTestItem.Create();
            var month2 = monthTestItem.ParameterValue;
            var offsetFrom = month1.OffsetFrom(month2);
            Assert.AreEqual(monthTestItem.ExpectedResult, offsetFrom);
        }

        private static readonly IEnumerable<MonthTestItem<Month, int>> OffsetFromTestItems = new[]
        {
            new MonthTestItem<Month, int>
            {
                MonthOfYear = 11,
                YearNum = 2018,
                ParameterName = "Other",
                ParameterValue = new Month(2018, 1),
                ExpectedResult = 10
            },
            new MonthTestItem<Month, int>
            {
                MonthOfYear = 1,
                YearNum = 2018,
                ParameterName = "Other",
                ParameterValue = new Month(2018, 1),
                ExpectedResult = 0
            },
            new MonthTestItem<Month, int>
            {
                MonthOfYear = 11,
                YearNum = 2018,
                ParameterName = "Other",
                ParameterValue = new Month(2019, 12),
                ExpectedResult = -13
            }
        };

        [TestCaseSource(nameof(OffsetTestItems))]
        public void AdditionOperator_ReturnsExpectedResult(MonthTestItem<int, Month> monthTestItem)
        {
            var month = monthTestItem.Create();
            var monthAfterAddition = month + monthTestItem.ParameterValue;
            Assert.AreEqual(monthTestItem.ExpectedResult, monthAfterAddition);
        }

        [TestCaseSource(nameof(OffsetFromTestItems))]
        public void MinusOperatorMonthParameter_ReturnsExpectedResult(MonthTestItem<Month, int> monthTestItem)
        {
            var month1 = monthTestItem.Create();
            var diff = month1 - monthTestItem.ParameterValue;
            Assert.AreEqual(monthTestItem.ExpectedResult, diff);
        }

        [TestCaseSource(nameof(MinusTestItems))]
        public void MinusOperatorIntParameter_ReturnsExpectedResult(MonthTestItem<int, Month> monthTestItem)
        {
            var month = monthTestItem.Create();
            var monthAfterSubtraction = month - monthTestItem.ParameterValue;
            Assert.AreEqual(monthTestItem.ExpectedResult, monthAfterSubtraction);
        }

        // TODO check addition and subtraction with too large numbers results in ArgumentOutOfRangeExceptions
        private static readonly IEnumerable<MonthTestItem<int, Month>> MinusTestItems = new[]
        {
            new MonthTestItem<int, Month>
            {
                MonthOfYear = 11,
                YearNum = 2018,
                ParameterName = "numPeriods",
                ParameterValue = 0,
                ExpectedResult = new Month(2018, 11)
            },
            new MonthTestItem<int, Month>
            {
                MonthOfYear = 11,
                YearNum = 2018,
                ParameterName = "numPeriods",
                ParameterValue = 13,
                ExpectedResult = new Month(2017, 10)
            },
            new MonthTestItem<int, Month>
            {
                MonthOfYear = 11,
                YearNum = 2018,
                ParameterName = "numPeriods",
                ParameterValue = -2,
                ExpectedResult = new Month(2019, 1)
            }
        };

        [TestCaseSource(nameof(PlusPlusTestItems))]
        public void PlusPlusOperator_EqualsNextMonth(MonthPairTestItem monthPairTestItem)
        {
            var (month, nextMonth) = monthPairTestItem.CreatePair();
            month++;
            Assert.AreEqual(nextMonth, month);
        }

        // TODO eliminate either PlusPlusTestItems or MinusMinusTestItems and just reverse the other?
        private static readonly IEnumerable<MonthPairTestItem> PlusPlusTestItems = new[]
        {
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 2018, MonthOfYear = 12},
                MonthTestItem2 = new MonthTestItem {YearNum = 2019, MonthOfYear = 1}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 10, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 10, MonthOfYear = 2}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 9950, MonthOfYear = 8},
                MonthTestItem2 = new MonthTestItem {YearNum = 9950, MonthOfYear = 9}
            }
        };

        [TestCaseSource(nameof(MinusMinusTestItems))]
        public void MinusMinusOperator_EqualsPreviousMonth(MonthPairTestItem monthPairTestItem)
        {
            var (month, previousMonth) = monthPairTestItem.CreatePair();
            month--;
            Assert.AreEqual(previousMonth, month);
        }

        private static readonly IEnumerable<MonthPairTestItem> MinusMinusTestItems = new[]
        {
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 2019, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 2018, MonthOfYear = 12}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 10, MonthOfYear = 2},
                MonthTestItem2 = new MonthTestItem {YearNum = 10, MonthOfYear = 1}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 9950, MonthOfYear = 9},
                MonthTestItem2 = new MonthTestItem {YearNum = 9950, MonthOfYear = 8}
            }
        };

        #endregion Time Period Arithmetic

        #region Formatting and Parsing

        [TestCaseSource(nameof(ToStringTestItems))]
        public void ToString_EqualsExpectedResult(
                            MonthTestItem<string> monthTestItem)
        {
            var month = monthTestItem.Create();
            var formatted = month.ToString();
            Assert.AreEqual(monthTestItem.ExpectedResult, formatted);
        }

        private static readonly IEnumerable<MonthTestItem<string>> ToStringTestItems = new[]
        {
            new MonthTestItem<string>
            {
                MonthOfYear = 1,
                YearNum = 9,
                ExpectedResult = "0009-01"
            },
            new MonthTestItem<string>
            {
                MonthOfYear = 3,
                YearNum = 50,
                ExpectedResult = "0050-03"
            },
            new MonthTestItem<string>
            {
                MonthOfYear = 10,
                YearNum = 9910,
                ExpectedResult = "9910-10"
            },
            new MonthTestItem<string>
            {
                MonthOfYear = 12,
                YearNum = 2018,
                ExpectedResult = "2018-12"
            },
            new MonthTestItem<string>
            {
                MonthOfYear = 6,
                YearNum = 1979,
                ExpectedResult = "1979-06"
            }
        };

        [TestCaseSource(nameof(ToStringTestItems))]
        public void Parse_EqualsExpectedResult(MonthTestItem<string> monthTestItem)
        {
            var monthFromText = Month.Parse(monthTestItem.ExpectedResult); // TODO bad use of Expected result?
            var expectedMonth = monthTestItem.Create();
            Assert.AreEqual(expectedMonth, monthFromText);
        }

        [TestCaseSource(nameof(InvalidMonthTextTestItems))]
        public void Parse_WithInvalidText_ThrowsArgumentException(string text)
        {
            Assert.Throws(Is.TypeOf<FormatException>().And
                .Message.EqualTo("String was not recognized as a valid Month."), () => Month.Parse(text));
        }

        private static readonly IEnumerable<string> InvalidMonthTextTestItems = new[]
        {
            "2018/01",
            "2018-00",
            "2018-13",
            "201801"
        };

        #endregion Formatting and Parsing

        #region Test Case and Value Sources

        private static readonly IEnumerable<MonthTestItem> MonthTestItems = new[]
        {
            new MonthTestItem {YearNum = 10, MonthOfYear = 1},
            new MonthTestItem {YearNum = 10, MonthOfYear = 2},
            new MonthTestItem {YearNum = 10, MonthOfYear = 3},
            new MonthTestItem {YearNum = 10, MonthOfYear = 4},
            new MonthTestItem {YearNum = 10, MonthOfYear = 5},
            new MonthTestItem {YearNum = 10, MonthOfYear = 6},
            new MonthTestItem {YearNum = 10, MonthOfYear = 7},
            new MonthTestItem {YearNum = 10, MonthOfYear = 8},
            new MonthTestItem {YearNum = 10, MonthOfYear = 9},
            new MonthTestItem {YearNum = 10, MonthOfYear = 10},
            new MonthTestItem {YearNum = 10, MonthOfYear = 11},
            new MonthTestItem {YearNum = 10, MonthOfYear = 12},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 1},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 2},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 3},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 4},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 5},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 6},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 7},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 8},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 9},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 10},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 11},
            new MonthTestItem {YearNum = 1979, MonthOfYear = 12},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 1},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 2},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 3},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 4},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 5},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 6},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 7},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 8},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 9},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 10},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 11},
            new MonthTestItem {YearNum = 2018, MonthOfYear = 12},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 1},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 2},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 3},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 4},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 5},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 6},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 7},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 8},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 9},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 10},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 11},
            new MonthTestItem {YearNum = 9852, MonthOfYear = 12}
        };

        private static IEnumerable<MonthPairTestItem> EqualMonthPairTestItems =>
                                                MonthTestItems.Select(monthTestItem => new MonthPairTestItem
                                                {
                                                    MonthTestItem1 = monthTestItem,
                                                    MonthTestItem2 = monthTestItem
                                                });

        private static IEnumerable<MonthPairTestItem> NonEqualMonthPairTestItems =>
                                                Month1EarlierThanMonth2.Union(Month1LaterThanMonth2);

        private static IEnumerable<MonthPairTestItem> Month1EarlierThanMonth2 => new[]
        {
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 10, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 10, MonthOfYear = 2}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 10, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 10, MonthOfYear = 6}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 10, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 10, MonthOfYear = 12}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 1979, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 2018, MonthOfYear = 2}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 1979, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 2018, MonthOfYear = 6}
            },
            new MonthPairTestItem
            {
                MonthTestItem1 = new MonthTestItem {YearNum = 1979, MonthOfYear = 1},
                MonthTestItem2 = new MonthTestItem {YearNum = 2018, MonthOfYear = 12}
            }
        };

        private static IEnumerable<MonthPairTestItem> Month1LaterThanMonth2 =>
                                Month1EarlierThanMonth2.Select(pairTestItem => new MonthPairTestItem
                                {
                                    MonthTestItem1 = pairTestItem.MonthTestItem2,
                                    MonthTestItem2 = pairTestItem.MonthTestItem1
                                });
        
        private static IEnumerable<MonthPairTestItem> Month1LaterThanOrEqualToMonth2 =>
                                    Month1LaterThanMonth2.Union(EqualMonthPairTestItems);

        private static IEnumerable<MonthPairTestItem> Month1EarlierThanOrEqualToMonth2 =>
                                    Month1EarlierThanMonth2.Union(EqualMonthPairTestItems);

        #endregion Test Case and Value Sources

        #region Test Helper Classes

        /// <summary>
        /// Contains data necessary to construct a Month instance for use in parameterised unit tests.
        /// </summary>
        public class MonthTestItem : ITestItem<Month>
        {
            public int YearNum { get; set; }
            public int MonthOfYear { get; set; }

            public Month Create() => new Month(YearNum, MonthOfYear);

            public override string ToString()
            {
                return $"{nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Month instance plus the expected result of an
        /// operation using Month which returns a single instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class MonthTestItem<TResult> : MonthTestItem
        {
            public TResult ExpectedResult { get; set; }

            public override string ToString()
            {
                return $"Month({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear})"
                                        + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Month instance plus the expected result and parameter value
        /// of an operation using Month which returns a single instance and uses a single parameter.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class MonthTestItem<TParameter, TResult> : MonthTestItem<TResult>
        {
            public TParameter ParameterValue { get; set; }
            public string ParameterName { get; set; }

            public override string ToString()
            {
                return $"Month({nameof(YearNum)}: {YearNum}, {nameof(MonthOfYear)}: {MonthOfYear})"
                       + $", {ParameterName}: {ParameterValue}"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a pair of Month instances for use in parameterised unit tests.
        /// </summary>
        public class MonthPairTestItem : ITestItemPair<Month>
        {
            public MonthTestItem MonthTestItem1 { get; set; }
            public MonthTestItem MonthTestItem2 { get; set; }

            public (Month timePeriod1, Month timePeriod2) CreatePair() =>
                (timePeriod1: MonthTestItem1.Create(), timePeriod2: MonthTestItem2.Create());

            public override string ToString()
            {
                return $"Month1: ({MonthTestItem1}), Month2: ({MonthTestItem2})";
            }
        }

        #endregion Test Helper Classes

    }
}
