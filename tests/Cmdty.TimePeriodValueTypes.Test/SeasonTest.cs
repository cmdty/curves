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
    public sealed partial class SeasonTest
    {

        [TestCaseSource(nameof(SeasonTestItems))]
        public void Deconstruct_ReturnsComponentsUsedToConstruct(SeasonTestItem testItem)
        {
            var season = testItem.Create();
            (int year, SeasonOfYear seasonOfYear) = season;
            Assert.AreEqual(testItem.YearNum, year);
            Assert.AreEqual(testItem.SeasonOfYear, seasonOfYear);
        }

        #region Test Case and Value Sources

        private static readonly IEnumerable<SeasonTestItem> SeasonTestItems = new[]
        {
            new SeasonTestItem {YearNum = 10, SeasonOfYear = SeasonOfYear.Summer},
            new SeasonTestItem {YearNum = 10, SeasonOfYear = SeasonOfYear.Winter},
            new SeasonTestItem {YearNum = 1979, SeasonOfYear = SeasonOfYear.Summer},
            new SeasonTestItem {YearNum = 1979, SeasonOfYear = SeasonOfYear.Winter},
            new SeasonTestItem {YearNum = 2018, SeasonOfYear = SeasonOfYear.Summer},
            new SeasonTestItem {YearNum = 2018, SeasonOfYear = SeasonOfYear.Winter},
            new SeasonTestItem {YearNum = 9852, SeasonOfYear = SeasonOfYear.Summer},
            new SeasonTestItem {YearNum = 9852, SeasonOfYear = SeasonOfYear.Winter}
        };

        private static IEnumerable<SeasonPairTestItem> EqualSeasonPairTestItems =>
                                                SeasonTestItems.Select(seasonTestItem => new SeasonPairTestItem
                                                {
                                                    SeasonTestItem1 = seasonTestItem,
                                                    SeasonTestItem2 = seasonTestItem
                                                });

        private static IEnumerable<SeasonPairTestItem> NonEqualSeasonPairTestItems =>
                                                Season1EarlierThanSeason2.Union(Season1LaterThanSeason2);

        private static IEnumerable<SeasonPairTestItem> Season1EarlierThanSeason2 => new[]
        {
            new SeasonPairTestItem
            {
                SeasonTestItem1 = new SeasonTestItem {YearNum = 10, SeasonOfYear = SeasonOfYear.Summer},
                SeasonTestItem2 = new SeasonTestItem {YearNum = 10, SeasonOfYear = SeasonOfYear.Winter}
            },
            new SeasonPairTestItem
            {
                SeasonTestItem1 = new SeasonTestItem {YearNum = 1979, SeasonOfYear = SeasonOfYear.Winter},
                SeasonTestItem2 = new SeasonTestItem {YearNum = 2018, SeasonOfYear = SeasonOfYear.Summer}
            },
            new SeasonPairTestItem
            {
                SeasonTestItem1 = new SeasonTestItem {YearNum = 1979, SeasonOfYear = SeasonOfYear.Summer},
                SeasonTestItem2 = new SeasonTestItem {YearNum = 2018, SeasonOfYear = SeasonOfYear.Winter}
            }
        };

        private static IEnumerable<SeasonPairTestItem> Season1LaterThanSeason2 =>
                                Season1EarlierThanSeason2.Select(pairTestItem => new SeasonPairTestItem
                                {
                                    SeasonTestItem1 = pairTestItem.SeasonTestItem2,
                                    SeasonTestItem2 = pairTestItem.SeasonTestItem1
                                });

        private static IEnumerable<SeasonPairTestItem> Season1LaterThanOrEqualToSeason2 =>
                                    Season1LaterThanSeason2.Union(EqualSeasonPairTestItems);

        private static IEnumerable<SeasonPairTestItem> Season1EarlierThanOrEqualToSeason2 =>
                                    Season1EarlierThanSeason2.Union(EqualSeasonPairTestItems);

        #endregion Test Case and Value Sources

        #region Test Helper Classes

        /// <summary>
        /// Contains data necessary to construct a Season instance for use in parameterised unit tests.
        /// </summary>
        public class SeasonTestItem : ITestItem<Season>
        {
            public int YearNum { get; set; }
            public SeasonOfYear SeasonOfYear { get; set; }

            public Season Create() => new Season(YearNum, SeasonOfYear);

            public override string ToString()
            {
                return $"{nameof(YearNum)}: {YearNum}, {nameof(SeasonOfYear)}: {SeasonOfYear}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Season instance plus the expected result of an
        /// operation using Season which returns a single instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class SeasonTestItem<TResult> : SeasonTestItem
        {
            public TResult ExpectedResult { get; set; }

            public override string ToString()
            {
                return $"Season({nameof(YearNum)}: {YearNum}, {nameof(SeasonOfYear)}: {SeasonOfYear})"
                                        + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a Season instance plus the expected result and parameter value
        /// of an operation using Season which returns a single instance and uses a single parameter.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        public class SeasonTestItem<TParameter, TResult> : SeasonTestItem<TResult>
        {
            public TParameter ParameterValue { get; set; }
            public string ParameterName { get; set; }

            public override string ToString()
            {
                return $"Season({nameof(YearNum)}: {YearNum}, {nameof(SeasonOfYear)}: {SeasonOfYear})"
                       + $", {ParameterName}: {ParameterValue}"
                       + $", ExpectedResult: {ExpectedResult}";
            }
        }

        /// <summary>
        /// Contains data necessary to construct a pair of Season instances for use in parameterised unit tests.
        /// </summary>
        public class SeasonPairTestItem : ITestItemPair<Season>
        {
            public SeasonTestItem SeasonTestItem1 { get; set; }
            public SeasonTestItem SeasonTestItem2 { get; set; }

            public (Season timePeriod1, Season timePeriod2) CreatePair() =>
                (timePeriod1: SeasonTestItem1.Create(), timePeriod2: SeasonTestItem2.Create());

            public override string ToString()
            {
                return $"Season1: ({SeasonTestItem1}), Season2: ({SeasonTestItem2})";
            }
        }

        #endregion Test Helper Classes


    }
}
