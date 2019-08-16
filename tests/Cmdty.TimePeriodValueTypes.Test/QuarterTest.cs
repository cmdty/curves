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
    public sealed partial class QuarterTest
    {
        // TODO tidy this up
        [Test]
        public void First_AsExpected()
        {
            var quarter = new Quarter(2018, 1);
            var firstMonth = quarter.First<Month>();
            var expectedFirstMonth = Month.CreateJanuary(2018);
            Assert.AreEqual(expectedFirstMonth, firstMonth);
        }

        [Test]
        public void Last_AsExpected()
        {
            var quarter = new Quarter(2018, 1);
            var lastMonth = quarter.Last<Month>();
            var expectedLastMonth = Month.CreateMarch(2018);
            Assert.AreEqual(expectedLastMonth, lastMonth);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void Deconstruct_ReturnsComponentsUsedToConstruct(QuarterTestItem testItem)
        {
            var quarter = testItem.Create();
            (int year, int quarterOfYear) = quarter;
            Assert.AreEqual(testItem.YearNum, year);
            Assert.AreEqual(testItem.QuarterOfYear, quarterOfYear);
        }
        
        #region Constructors
        [TestCaseSource(nameof(YearOutOfRangeTestItems))]
        public void Constructor_WithYearParameterOutOfRange_ThrowsArgumentOutOfRangeException(QuarterTestItem quarterTestItem)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => quarterTestItem.Create());
        }

        private static readonly IEnumerable<QuarterTestItem> YearOutOfRangeTestItems = new[]
        {
            // Year too early
            new QuarterTestItem{YearNum = 0, QuarterOfYear = 1},
            new QuarterTestItem{YearNum = -10, QuarterOfYear = 2},
            new QuarterTestItem{YearNum = -950, QuarterOfYear = 4},
            // Year too late
            new QuarterTestItem{YearNum = 9999, QuarterOfYear = 1},
            new QuarterTestItem{YearNum = 10564, QuarterOfYear = 3},
            new QuarterTestItem{YearNum = 45669, QuarterOfYear = 4}
        };

        [TestCaseSource(nameof(QuarterOfYearOutOfRangeTestItems))]
        public void Constructor_WithQuarterOfYearParameterOutOfRange_ThrowsArgumentOutOfRangeException(QuarterTestItem quarterTestItem)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => quarterTestItem.Create());
        }

        private static readonly IEnumerable<QuarterTestItem> QuarterOfYearOutOfRangeTestItems = new[]
        {
            // Quarter of Year too low
            new QuarterTestItem{YearNum = 10, QuarterOfYear = -1},
            new QuarterTestItem{YearNum = 950, QuarterOfYear = -2},
            new QuarterTestItem{YearNum = 1979, QuarterOfYear = -40},
            // Quarter of Year too high
            new QuarterTestItem{YearNum = 10, QuarterOfYear = 5},
            new QuarterTestItem{YearNum = 950, QuarterOfYear = 6},
            new QuarterTestItem{YearNum = 1979, QuarterOfYear = 8}
        };

        [Test]
        public void DefaultConstructor_ReturnsInstanceEqualToQ10001()
        {
            var quarter = new Quarter();
            var q10001 = new Quarter(1, 1);
            Assert.AreEqual(q10001, quarter);
        }

        #endregion Constructors

        #region Static Factory Methods

        [TestCaseSource(nameof(FromDateTimeTestItems))]
        public void FromDateTime_AsExpected(DateTime dateTime, Quarter expectedQuarter)
        {
            var quarter = Quarter.FromDateTime(dateTime);
            Assert.AreEqual(expectedQuarter, quarter);
        }

        private static readonly IEnumerable<object> FromDateTimeTestItems = new []
        {
            new object[]
            {
                new DateTime(2018, 1, 1), 
                new Quarter(2018, 1)
            },
            new object[]
            {
                new DateTime(2018, 2, 10),
                new Quarter(2018, 1)
            },
            new object[]
            {
                new DateTime(2018, 3, 31),
                new Quarter(2018, 1)
            },
            new object[]
            {
                new DateTime(2018, 4, 1),
                new Quarter(2018, 2)
            },
            new object[]
            {
                new DateTime(2018, 5, 10),
                new Quarter(2018, 2)
            },
            new object[]
            {
                new DateTime(2018, 6, 30),
                new Quarter(2018, 2)
            },

            new object[]
            {
                new DateTime(2018, 7, 1),
                new Quarter(2018, 3)
            },
            new object[]
            {
                new DateTime(2018, 8, 10),
                new Quarter(2018, 3)
            },
            new object[]
            {
                new DateTime(2018, 9, 30),
                new Quarter(2018, 3)
            },
            new object[]
            {
                new DateTime(2018, 10, 1),
                new Quarter(2018, 4)
            },
            new object[]
            {
                new DateTime(2018, 11, 10),
                new Quarter(2018, 4)
            },
            new object[]
            {
                new DateTime(2018, 12, 31),
                new Quarter(2018, 4)
            }
        };

        [TestCaseSource(nameof(CreateQuarterTestItems))]
        public void CreateQuarter1_AsExpected(int year)
        {
            var quarter = Quarter.CreateQuarter1(year);
            var expected = new Quarter(year, 1);
            Assert.AreEqual(expected, quarter);
        }

        [TestCaseSource(nameof(CreateQuarterTestItems))]
        public void CreateQuarter2_AsExpected(int year)
        {
            var quarter = Quarter.CreateQuarter2(year);
            var expected = new Quarter(year, 2);
            Assert.AreEqual(expected, quarter);
        }

        [TestCaseSource(nameof(CreateQuarterTestItems))]
        public void CreateQuarter3_AsExpected(int year)
        {
            var quarter = Quarter.CreateQuarter3(year);
            var expected = new Quarter(year, 3);
            Assert.AreEqual(expected, quarter);
        }

        [TestCaseSource(nameof(CreateQuarterTestItems))]
        public void CreateQuarter4_AsExpected(int year)
        {
            var quarter = Quarter.CreateQuarter4(year);
            var expected = new Quarter(year, 4);
            Assert.AreEqual(expected, quarter);
        }

        private static readonly IEnumerable<int> CreateQuarterTestItems = new[]
        {
            1,
            10,
            1979,
            2018,
            9998
        };

        #endregion Static Factory Methods

        #region Instance Properties

        [TestCaseSource(nameof(StartTestItems))]
        public void Start_AsExpected(QuarterTestItem<DateTime> quarterTestItem)
        {
            var quarter = quarterTestItem.Create();
            Assert.AreEqual(quarterTestItem.ExpectedResult, quarter.Start);
        }

        private static readonly IEnumerable<QuarterTestItem<DateTime>> StartTestItems = new[]
        {
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 1,
                ExpectedResult = new DateTime(2018, 1, 1)
            },
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 2,
                ExpectedResult = new DateTime(2018, 4, 1)
            },
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 3,
                ExpectedResult = new DateTime(2018, 7, 1)
            },
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 4,
                ExpectedResult = new DateTime(2018, 10, 1)
            }
        };

        [TestCaseSource(nameof(EndTestItems))]
        public void End_AsExpected(QuarterTestItem<DateTime> quarterTestItem)
        {
            var quarter = quarterTestItem.Create();
            Assert.AreEqual(quarterTestItem.ExpectedResult, quarter.End);
        }
        
        private static readonly IEnumerable<QuarterTestItem<DateTime>> EndTestItems = new[]
        {
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 1,
                ExpectedResult = new DateTime(2018, 4, 1)
            },
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 2,
                ExpectedResult = new DateTime(2018, 7, 1)
            },
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 3,
                ExpectedResult = new DateTime(2018, 10, 1)
            },
            new QuarterTestItem<DateTime>
            {
                YearNum = 2018,
                QuarterOfYear = 4,
                ExpectedResult = new DateTime(2019, 1, 1)
            }
        };

        [Test]
        public void Start_ForMinQuarter_EqualsFirstOfJanYear1()
        {
            // Checking that Start doesn't do anything unwanted at the boundaries
            var minQuarter = Quarter.MinQuarter;
            var firstOfJan0001 = new DateTime(1, 1, 1);
            Assert.AreEqual(firstOfJan0001, minQuarter.Start);
        }

        [Test]
        public void End_ForMaxQuarter_EqualsFirstOfJanYear9999()
        {
            // Checking that End doesn't do anything unwanted at the boundaries
            var maxQuarter = Quarter.MaxQuarter;
            var firstOfJan9999 = new DateTime(9999, 1, 1);
            Assert.AreEqual(firstOfJan9999, maxQuarter.End);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void YearProperty_EqualsYearUsedToConstruct(QuarterTestItem quarterTestItem)
        {
            var quarter = quarterTestItem.Create();
            Assert.AreEqual(quarterTestItem.YearNum, quarter.Year);
        }

        #endregion Instance Properties

        #region Static Properties

        [Test]
        public void MinQuarter_EqualsQ1Year1()
        {
            var minQuarter = Quarter.MinQuarter;
            var q1Year1 = new Quarter(1, 1);
            Assert.AreEqual(q1Year1, minQuarter);
        }

        [Test]
        public void MaxQuarter_EqualsQ4Year9998()
        {
            var maxQuarter = Quarter.MaxQuarter;
            var q3Year9998 = new Quarter(9998, 4);
            Assert.AreEqual(q3Year9998, maxQuarter);
        }

        #endregion Static Properties

        #region Formatting and Parsing
        // TODO move all of this into t4 generated code?
        // TODO decide on consistent naming of tests; AsExpected or EqualsExpectedResult
        [TestCaseSource(nameof(ToStringTestItems))]
        public void ToString_EqualsExpectedResult(QuarterTestItem<string> quarterTestItem)
        {
            var quarter = quarterTestItem.Create();
            var formatted = quarter.ToString();
            Assert.AreEqual(quarterTestItem.ExpectedResult, formatted);
        }

        private static readonly IEnumerable<QuarterTestItem<string>> ToStringTestItems = new[]
        {
            new QuarterTestItem<string>
            {
                YearNum = 2018,
                QuarterOfYear = 1,
                ExpectedResult = "Q1-2018"
            },
            new QuarterTestItem<string>
            {
                YearNum = 50,
                QuarterOfYear = 2,
                ExpectedResult = "Q2-0050"
            },
            new QuarterTestItem<string>
            {
                YearNum = 199,
                QuarterOfYear = 3,
                ExpectedResult = "Q3-0199"
            },
            new QuarterTestItem<string>
            {
                YearNum = 9974,
                QuarterOfYear = 4,
                ExpectedResult = "Q4-9974"
            }
        };

        [TestCaseSource(nameof(ToStringTestItems))]
        public void Parse_EqualsExpectedResult(QuarterTestItem<string> quarterTestItem)
        {
            var quarterFromText = Quarter.Parse(quarterTestItem.ExpectedResult); // TODO bad use of Expected result?
            var expectedQuarter = quarterTestItem.Create();
            Assert.AreEqual(expectedQuarter, quarterFromText);
        }
        
        [TestCaseSource(nameof(InvalidQuarterTextTestItems))]
        public void Parse_WithInvalidText_ThrowsArgumentException(string text)
        {
            Assert.Throws(Is.TypeOf<FormatException>().And
                .Message.EqualTo($"The string '{text}' was not recognized as a valid Quarter."), () => Quarter.Parse(text));
        }

        private static readonly IEnumerable<string> InvalidQuarterTextTestItems = new[]
        {
            "1-2010",
            "q1-2010",
            "Q1-101",
            "Q12011",
            "Q1*2010",
            "Q1-20120"
        };
        
        #endregion Formatting and Parsing

        #region Test Case and Value Sources

        private static readonly IEnumerable<QuarterTestItem> QuarterTestItems = new[]
        {
            new QuarterTestItem {YearNum = 10, QuarterOfYear = 1},
            new QuarterTestItem {YearNum = 10, QuarterOfYear = 2},
            new QuarterTestItem {YearNum = 10, QuarterOfYear = 3},
            new QuarterTestItem {YearNum = 10, QuarterOfYear = 4},
            new QuarterTestItem {YearNum = 1979, QuarterOfYear = 1},
            new QuarterTestItem {YearNum = 1979, QuarterOfYear = 2},
            new QuarterTestItem {YearNum = 1979, QuarterOfYear = 3},
            new QuarterTestItem {YearNum = 1979, QuarterOfYear = 4},
            new QuarterTestItem {YearNum = 2018, QuarterOfYear = 1},
            new QuarterTestItem {YearNum = 2018, QuarterOfYear = 2},
            new QuarterTestItem {YearNum = 2018, QuarterOfYear = 3},
            new QuarterTestItem {YearNum = 2018, QuarterOfYear = 4},
            new QuarterTestItem {YearNum = 9852, QuarterOfYear = 1},
            new QuarterTestItem {YearNum = 9852, QuarterOfYear = 2},
            new QuarterTestItem {YearNum = 9852, QuarterOfYear = 3},
            new QuarterTestItem {YearNum = 9852, QuarterOfYear = 4}
        };

        private static IEnumerable<QuarterPairTestItem> EqualQuarterPairTestItems =>
                                                QuarterTestItems.Select(quarterTestItem => new QuarterPairTestItem
                                                {
                                                    QuarterTestItem1 = quarterTestItem,
                                                    QuarterTestItem2 = quarterTestItem
                                                });

        private static IEnumerable<QuarterPairTestItem> NonEqualQuarterPairTestItems =>
                                                Quarter1EarlierThanQuarter2.Union(Quarter1LaterThanQuarter2);

        private static IEnumerable<QuarterPairTestItem> Quarter1EarlierThanQuarter2 => new[]
        {
            new QuarterPairTestItem
            {
                QuarterTestItem1 = new QuarterTestItem {YearNum = 10, QuarterOfYear = 1},
                QuarterTestItem2 = new QuarterTestItem {YearNum = 10, QuarterOfYear = 2}
            },
            new QuarterPairTestItem
            {
                QuarterTestItem1 = new QuarterTestItem {YearNum = 10, QuarterOfYear = 1},
                QuarterTestItem2 = new QuarterTestItem {YearNum = 10, QuarterOfYear = 4}
            },
            new QuarterPairTestItem
            {
                QuarterTestItem1 = new QuarterTestItem {YearNum = 10, QuarterOfYear = 3},
                QuarterTestItem2 = new QuarterTestItem {YearNum = 10, QuarterOfYear = 4}
            },
            new QuarterPairTestItem
            {
                QuarterTestItem1 = new QuarterTestItem {YearNum = 1979, QuarterOfYear = 1},
                QuarterTestItem2 = new QuarterTestItem {YearNum = 2018, QuarterOfYear = 2}
            },
            new QuarterPairTestItem
            {
                QuarterTestItem1 = new QuarterTestItem {YearNum = 1979, QuarterOfYear = 1},
                QuarterTestItem2 = new QuarterTestItem {YearNum = 2018, QuarterOfYear = 4}
            },
            new QuarterPairTestItem
            {
                QuarterTestItem1 = new QuarterTestItem {YearNum = 1979, QuarterOfYear = 1},
                QuarterTestItem2 = new QuarterTestItem {YearNum = 2018, QuarterOfYear = 1}
            }
        };

        private static IEnumerable<QuarterPairTestItem> Quarter1LaterThanQuarter2 =>
                                Quarter1EarlierThanQuarter2.Select(pairTestItem => new QuarterPairTestItem
                                {
                                    QuarterTestItem1 = pairTestItem.QuarterTestItem2,
                                    QuarterTestItem2 = pairTestItem.QuarterTestItem1
                                });

        private static IEnumerable<QuarterPairTestItem> Quarter1LaterThanOrEqualToQuarter2 =>
                                    Quarter1LaterThanQuarter2.Union(EqualQuarterPairTestItems);

        private static IEnumerable<QuarterPairTestItem> Quarter1EarlierThanOrEqualToQuarter2 =>
                                    Quarter1EarlierThanQuarter2.Union(EqualQuarterPairTestItems);

        #endregion Test Case and Value Sources
        
        #region Test Helper Classes

        /// <summary>
        /// Contains data necessary to construct a Quarter instance for use in parameterised unit tests.
        /// </summary>
        public class QuarterTestItem : ITestItem<Quarter>
        {
            public int YearNum { get; set; }
            public int QuarterOfYear { get; set; }

            public Quarter Create() => new Quarter(YearNum, QuarterOfYear);

            public override string ToString()
            {
                return $"{nameof(YearNum)}: {YearNum}, {nameof(QuarterOfYear)}: {QuarterOfYear}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Quarter instance plus the expected result of an
        /// operation using Quarter which returns a single instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class QuarterTestItem<TResult> : QuarterTestItem
        {
            public TResult ExpectedResult { get; set; }

            public override string ToString()
            {
                return $"Quarter({nameof(YearNum)}: {YearNum}, {nameof(QuarterOfYear)}: {QuarterOfYear})"
                                        + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Quarter instance plus the expected result and parameter value
        /// of an operation using Quarter which returns a single instance and uses a single parameter.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class QuarterTestItem<TParameter, TResult> : QuarterTestItem<TResult>
        {
            public TParameter ParameterValue { get; set; }
            public string ParameterName { get; set; }

            public override string ToString()
            {
                return $"Quarter({nameof(YearNum)}: {YearNum}, {nameof(QuarterOfYear)}: {QuarterOfYear})"
                       + $", {ParameterName}: {ParameterValue}"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a pair of Quarter instances for use in parameterised unit tests.
        /// </summary>
        public class QuarterPairTestItem : ITestItemPair<Quarter>
        {
            public QuarterTestItem QuarterTestItem1 { get; set; }
            public QuarterTestItem QuarterTestItem2 { get; set; }

            public (Quarter timePeriod1, Quarter timePeriod2) CreatePair() =>
                (timePeriod1: QuarterTestItem1.Create(), timePeriod2: QuarterTestItem2.Create());

            public override string ToString()
            {
                return $"Quarter1: ({QuarterTestItem1}), Quarter2: ({QuarterTestItem2})";
            }
        }

        #endregion Test Helper Classes

    }
}
