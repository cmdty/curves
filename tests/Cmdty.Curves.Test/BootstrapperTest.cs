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
using Cmdty.TimePeriodValueTypes;
using NUnit.Framework;

namespace Cmdty.Curves.Test
{
    [TestFixture]
    public sealed class BootstrapperTest 
    {
        private const double Tolerance = 1E-10;

        [TestCaseSource(nameof(MonthContractTestCases))]
        public void Bootstrap_WithMonthTypeParameter_CurveConsistentWithInputs(List<Contract<Month>> contracts, 
                                                                            List<Shaping<Month>> shapings)
        {
            var (curve, _) = new Bootstrapper<Month>()
                    .AddContracts(contracts)
                    .AddShapings(shapings)
                    .Bootstrap();
            TestHelper.AssertCurveConsistentWithInputs(contracts, period => period.End.Subtract(period.Start).Days, curve, Tolerance);
        }

        [TestCaseSource(nameof(MonthContractTestCases))]
        public void Bootstrap_WithMonthTypeParameter_BootstrappedContractsConsistentWithCurve(List<Contract<Month>> contracts,
                                                                                        List<Shaping<Month>> shapings)
        {
            var (curve, bootstrappedContracts) = new Bootstrapper<Month>()
                .AddContracts(contracts)
                .AddShapings(shapings)
                .Bootstrap();
            AssertBootstrappedContractsConsistentWithCurve(curve, bootstrappedContracts, 
                period => period.End.Subtract(period.Start).Days, 1E-10);
        }
        // TODO both of the above tests with a different time period granularity

        private static readonly IEnumerable<object> MonthContractTestCases = new object[]
        {
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1),
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72)
                },
                new List<Shaping<Month>>()
            },
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1)
                },
                new List<Shaping<Month>>()
            },
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72)
                },
                new List<Shaping<Month>>()
            },
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1),
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72),
                    // Gap
                    Contract.Create(Month.CreateOctober(2019), Month.CreateDecember(2019), 15.07),
                    Contract.CreateForSinglePeriod(Month.CreateOctober(2019), 48.88)
                },
                new List<Shaping<Month>>()
            },
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateApril(2019), 12.1),
                    // Gap
                    Contract.CreateForSinglePeriod(Month.CreateJune(2019), 14.72),
                    // Gap
                    Contract.Create(Month.CreateOctober(2019), Month.CreateDecember(2019), 15.07),
                    // Gap
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2020), 48.88)
                },
                new List<Shaping<Month>>()
            },
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateJune(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1),
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72)
                },
                new List<Shaping<Month>>()
                {
                    Shaping<Month>.Ratio.Between(Month.CreateApril(2019)).And(Month.CreateMay(2019)).Is(1.1),
                    Shaping<Month>.Spread.Between(Month.CreateMay(2019)).And(Month.CreateJune(2019)).Is(0.2)
                }
            }
        };

        [Test]
        public void Bootstrap_WithRedundantContracts_ThrowsArgumentException()
        {
            IBootstrapperAddOptionalParameters<Month> bootstrapper = new Bootstrapper<Month>()
                .AddContract(Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55))
                .AddContract(Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1))
                .AddContract(Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72))
                .AddContract(Contract.CreateForSinglePeriod(Month.CreateMarch(2019), 18.01));

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Redundant contracts and shapings are present"), 
                        () => bootstrapper.Bootstrap());
        }

        [Test]
        public void Bootstrap_WithContractsAndSpreadJointlyRedundant_ThrowsArgumentException()
        {
            var feb20 = Month.CreateFebruary(2020);
            var mar20 = Month.CreateMarch(2020);

            IBootstrapperAddOptionalParameters<Month> bootstrapper = new Bootstrapper<Month>()
                                        .AddContract(Month.CreateJanuary(2020), 22.95)
                                        .AddContract(feb20, 21.05)
                                        .AddContract(Quarter.CreateQuarter1(2020), 19.05)
                                        .AddShaping(Shaping<Month>.Spread.Between(feb20).And(mar20).Is(0.21));

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Redundant contracts and shapings are present"),
                () => bootstrapper.Bootstrap());
        }

        [Test]
        public void Bootstrap_WithRedundantContractAndAllowRedundancy_DoesNotThrow()
        {
            IBootstrapperAddOptionalParameters<Month> bootstrapper = new Bootstrapper<Month>()
                .AddContract(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55)
                .AddContract(Month.CreateJanuary(2019), 12.1)
                .AddContract(Month.CreateFebruary(2019), 14.72)
                .AddContract(Month.CreateMarch(2019), 18.01)
                .AllowRedundancy();

            Assert.DoesNotThrow(() => bootstrapper.Bootstrap());
        }

        [Test]
        public void Bootstrap_WithZeroWeightedContract_ThrowsArgumentException()
        {
            var builder = new Bootstrapper<Day>()
                .AddContract(new Day(2019, 1, 7), new Day(2019, 1, 13), 58.45)
                .AddContract(new Day(2019, 1, 12), new Day(2019, 1, 13), 60.87)
                .WithAverageWeighting(Weighting.BusinessDayCount<Day>(new List<Day>()));

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo(
                    "sum of weighting evaluated to non-positive number for the following contract: Start: 2019-01-12, End: 2019-01-13, Price: 60.87"),
                () => builder.Bootstrap());
        }

        [Test]
        public void Bootstrap_AverageWeightingNotAdded_ResultsConsistentWithPeriodLengthWeight()
        {
            const double janPrice = 85.69;
            const double febPrice = 88.000085;
            const double q1Price = 90.1005968;
            
            DoubleCurve<Month> curve = new Bootstrapper<Month>()
                .AddContract(Month.CreateJanuary(2019), janPrice)
                .AddContract(Month.CreateFebruary(2019), febPrice)
                .AddContract(Quarter.CreateQuarter1(2019), q1Price)
                .Bootstrap()
                .Curve;

            double backCalculatedQ1Price = (curve[Month.CreateJanuary(2019)] * 31.0 +
                                           curve[Month.CreateFebruary(2019)] * 28.0 +
                                           curve[Month.CreateMarch(2019)] * 31.0)/(31.0 + 28.0 + 31.0);
            Assert.AreEqual(q1Price, backCalculatedQ1Price, Tolerance);
        }

        // TODO test using Weighting.BusinessDayCount
        // TODO test without weighting e.g. equal weighting



        private void AssertBootstrappedContractsConsistentWithCurve<T>(DoubleCurve<T> curve,
                        IReadOnlyList<Contract<T>> bootstrappedContracts, Func<T, double> weighting, double tolerance)
            where T : ITimePeriod<T>
        {
            foreach (var bootstrappedContract in bootstrappedContracts)
            {
                foreach (var curvePeriod in bootstrappedContract.Start.EnumerateTo(bootstrappedContract.End))
                {
                    Assert.AreEqual(bootstrappedContract.Price, curve[curvePeriod], tolerance);
                }
            }
        }
        
        [TestCaseSource(nameof(BootstrappedContractsStartAndEndMonthInputs))]
        public void Bootstrap_WithMonthTypeParameter_BootstrappedContractsStartAndEndAsExpected(
                List<Contract<Month>> inputContracts, List<Shaping<Month>> shapings, List<(Month Start, Month End)> expectedStartAndEnd)
        {
            // TODO think how and whether to use AllowRedundancy in this test
            AssertBootstrappedContractsStartAndEndAsExpected(inputContracts, shapings, expectedStartAndEnd);
        }

        private static readonly IEnumerable<object> BootstrappedContractsStartAndEndMonthInputs = new object[]
        {
            // 1 quarter and two months results in 3 months
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1),
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72)
                },
                new List<Shaping<Month>>(),
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateFebruary(2019)),
                    (Start: Month.CreateMarch(2019), End: Month.CreateMarch(2019))
                }
            },
            // 1 quarter and 1 month results in 1 month and a 2-month strip
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1)
                },
                new List<Shaping<Month>>(),
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateMarch(2019))
                }
            },
            // 1 quarter and 1 month in middle results in 3 months
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72)
                },
                new List<Shaping<Month>>(),
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateFebruary(2019)),
                    (Start: Month.CreateMarch(2019), End: Month.CreateMarch(2019))
                }
            },

            // 2 separate quarters with overlapping months and gap between
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateJanuary(2019), 12.1),
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2019), 14.72),
                    // Gap
                    Contract.Create(Month.CreateOctober(2019), Month.CreateDecember(2019), 15.07),
                    Contract.CreateForSinglePeriod(Month.CreateOctober(2019), 48.88)
                },
                new List<Shaping<Month>>(),
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateFebruary(2019)),
                    (Start: Month.CreateMarch(2019), End: Month.CreateMarch(2019)),
                    // Gap
                    (Start: Month.CreateOctober(2019), End: Month.CreateOctober(2019)),
                    (Start: Month.CreateNovember(2019), End: Month.CreateDecember(2019))
                }
            },
            // Some contracts with no overlaps and gaps between them
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                    Contract.CreateForSinglePeriod(Month.CreateApril(2019), 12.1),
                    // Gap
                    Contract.CreateForSinglePeriod(Month.CreateJune(2019), 14.72),
                    // Gap
                    Contract.Create(Month.CreateOctober(2019), Month.CreateDecember(2019), 15.07),
                    // Gap
                    Contract.CreateForSinglePeriod(Month.CreateFebruary(2020), 48.88)
                },
                new List<Shaping<Month>>(),
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateMarch(2019)),
                    (Start: Month.CreateApril(2019), End: Month.CreateApril(2019)),
                    (Start: Month.CreateJune(2019), End: Month.CreateJune(2019)),
                    // Gap
                    (Start: Month.CreateOctober(2019), End: Month.CreateDecember(2019)),
                    (Start: Month.CreateFebruary(2020), End: Month.CreateFebruary(2020))
                }
            }
        };

        [TestCaseSource(nameof(BootstrappedContractsStartAndEndMonthInputsWithShaping))]
        public void Bootstrap_WithMonthTypeParameterAndShaping_BootstrappedContractsStartAndEndAsExpected(
            List<Contract<Month>> inputContracts, List<Shaping<Month>> shapings, List<(Month Start, Month End)> expectedStartAndEnd)
        {
            // TODO think how and whether to use AllowRedundancy in this test
            AssertBootstrappedContractsStartAndEndAsExpected(inputContracts, shapings, expectedStartAndEnd);
        }

        private static readonly IEnumerable<object> BootstrappedContractsStartAndEndMonthInputsWithShaping = new object[]
        {
            // 1 quarter and ratio between 1st month and quarter results in 1 month and a 2-month strip
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract<Month>.Create(Quarter.CreateQuarter1(2019), 13.55),
                },
                new List<Shaping<Month>>()
                {
                    Shaping<Month>.Ratio.Between(Month.CreateJanuary(2019)).And(Quarter.CreateQuarter1(2019)).Is(1.2)
                },
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateMarch(2019))
                }
            },
            // 1 quarter and spread between 1st month and quarter results in 1 month and a 2-month strip
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract<Month>.Create(Quarter.CreateQuarter1(2019), 13.55),
                },
                new List<Shaping<Month>>()
                {
                    Shaping<Month>.Spread.Between(Month.CreateJanuary(2019)).And(Quarter.CreateQuarter1(2019)).Is(1.2)
                },
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateMarch(2019))
                }
            },

            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract<Month>.Create(Quarter.CreateQuarter1(2019), 13.55),
                },
                new List<Shaping<Month>>()
                {
                    Shaping<Month>.Spread.Between(Month.CreateApril(2019)).And(Quarter.CreateQuarter1(2019)).Is(1.2)
                },
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateMarch(2019)),
                    (Start: Month.CreateApril(2019), End: Month.CreateApril(2019))
                }
            },
                        // 1 quarter and 1 ratio shaping in results in 3 months
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract<Month>.Create(Quarter.CreateQuarter1(2019), 13.55),
                },
                new List<Shaping<Month>>
                {
                    Shaping<Month>.Ratio.Between(Month.CreateJanuary(2019)).And(Month.CreateMarch(2019)).Is(1.09),
                },
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateFebruary(2019)),
                    (Start: Month.CreateMarch(2019), End: Month.CreateMarch(2019))
                }
            },
            // 1 quarter and 1 spread shaping in results in 3 months
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract<Month>.Create(Quarter.CreateQuarter1(2019), 13.55),
                },
                new List<Shaping<Month>>
                {
                    Shaping<Month>.Spread.Between(Month.CreateJanuary(2019)).And(Month.CreateMarch(2019)).Is(0.02),
                },
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateJanuary(2019)),
                    (Start: Month.CreateFebruary(2019), End: Month.CreateFebruary(2019)),
                    (Start: Month.CreateMarch(2019), End: Month.CreateMarch(2019))
                }
            },
            // Ratio filling gap
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract<Month>.Create(Quarter.CreateQuarter1(2019), 13.55),
                    Contract<Month>.Create(Quarter.CreateQuarter3(2019), 12.01)
                },
                new List<Shaping<Month>>
                {
                    Shaping<Month>.Ratio.Between(Quarter.CreateQuarter1(2019)).And(Quarter.CreateQuarter2(2019)).Is(0.02),
                },
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateMarch(2019)),
                    (Start: Month.CreateApril(2019), End: Month.CreateJune(2019)),
                    (Start: Month.CreateJuly(2019), End: Month.CreateSeptember(2019))
                }
            },
            // Spread filling gap
            new object[]
            {
                new List<Contract<Month>>
                {
                    Contract<Month>.Create(Quarter.CreateQuarter1(2019), 13.55),
                    Contract<Month>.Create(Quarter.CreateQuarter3(2019), 12.01)
                },
                new List<Shaping<Month>>
                {
                    Shaping<Month>.Spread.Between(Quarter.CreateQuarter1(2019)).And(Quarter.CreateQuarter2(2019)).Is(0.02),
                },
                new List<(Month Start, Month End)>
                {
                    (Start: Month.CreateJanuary(2019), End: Month.CreateMarch(2019)),
                    (Start: Month.CreateApril(2019), End: Month.CreateJune(2019)),
                    (Start: Month.CreateJuly(2019), End: Month.CreateSeptember(2019))
                }
            }
        };

        private void AssertBootstrappedContractsStartAndEndAsExpected<T>(
                List<Contract<T>> inputContracts, List<Shaping<T>> shapings, List<(T Start, T End)> expectedStartAndEnd) where T : ITimePeriod<T>
        {
            IBootstrapper<T> builder = new Bootstrapper<T>();

            IBootstrapperAddOptionalParameters<T> canAddShaping;

            if (inputContracts.Count == 1)
            {
                canAddShaping = builder.AddContract(inputContracts[0]);
            }
            else
            {
                canAddShaping = builder.AddContracts(inputContracts);
            }

            if (shapings.Count > 0)
            {
                canAddShaping.AddShapings(shapings);
            }

            var results = canAddShaping.Bootstrap();

            Assert.AreEqual(expectedStartAndEnd.Count, results.BootstrappedContracts.Count);

            for (int i = 0; i < expectedStartAndEnd.Count; i++)
            {
                Assert.AreEqual(expectedStartAndEnd[i].Start, results.BootstrappedContracts[i].Start);
                Assert.AreEqual(expectedStartAndEnd[i].End, results.BootstrappedContracts[i].End);
            }
        }

        [Test]
        public void Bootstrap_SingleShapingRatio_AsExpected()
        {
            const double inputPriceRatio = 1.2;
            var (curve, _) = new Bootstrapper<Month>()
                .AddContract(Quarter.CreateQuarter1(2018), 25.5)
                .AddShaping(Shaping<Month>.Ratio.Between(Month.CreateJanuary(2018))
                    .And(Month.CreateFebruary(2018)).Is(inputPriceRatio))
                .Bootstrap();

            var janPrice = curve[Month.CreateJanuary(2018)];
            var febPrice = curve[Month.CreateFebruary(2018)];

            var outputPriceRatio = janPrice / febPrice;
            Assert.AreEqual(inputPriceRatio, outputPriceRatio, Tolerance);
        }

        [Test]
        public void Bootstrap_SingleShapingSpread_AsExpected()
        {
            const double inputPriceSpread = -0.88;
            var (curve, _) = new Bootstrapper<Month>()
                .AddContract(Quarter.CreateQuarter1(2018), 25.5)
                .AddShaping(Shaping<Month>.Spread.Between(Month.CreateJanuary(2018))
                    .And(Month.CreateFebruary(2018)).Is(inputPriceSpread))
                .Bootstrap();

            var janPrice = curve[Month.CreateJanuary(2018)];
            var febPrice = curve[Month.CreateFebruary(2018)];

            var outputPriceSpread = janPrice - febPrice;
            Assert.AreEqual(inputPriceSpread, outputPriceSpread, Tolerance);
        }
        
    }
}
