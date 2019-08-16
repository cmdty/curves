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
using Cmdty.TimePeriodValueTypes;
using NUnit.Framework;

namespace Cmdty.Curves.Test
{
    [TestFixture]
    public sealed class MaxSmoothnessSplineCurveBuilderTest
    {
        private const double Tolerance = 1E-10;
        
        [Test]
        public void Build_InputContractsAllEqualAndNoSeasonalAdjustments_ResultsInFlatCurve()
        {
            const double flatPrice = 84.54;
            var curve = new MaxSmoothnessSplineCurveBuilder<Day>()
                .AddContract(Month.CreateJanuary(2019), flatPrice)
                .AddContract(Month.CreateFebruary(2019), flatPrice)
                .AddContract(Month.CreateMarch(2019), flatPrice)
                .AddContract(Quarter.CreateQuarter2(2019), flatPrice)
                .AddContract(Quarter.CreateQuarter3(2019), flatPrice)
                .AddContract(Season.CreateWinter(2019), flatPrice)
                .BuildCurve();

            foreach (double price in curve.Data)
            {
                Assert.AreEqual(flatPrice, price, 1E-12);
            }

        }

        [Test]
        public void Build_InputContractsInLinearTrend_ResultsLinear()
        {
            // Parameters of linear trend
            const double intercept = 45.7;
            const double dailySlope = 0.8;

            double dailyPrice = intercept;
            Day contractDay = new Day(2019, 5, 11);
            var contracts = new List<Contract<Hour>>();

            for (int i = 0; i < 14; i++)
            {
                contracts.Add(Contract<Hour>.Create(contractDay, dailyPrice));
                contractDay++;
                dailyPrice += dailySlope;
            }

            var curve = new MaxSmoothnessSplineCurveBuilder<Hour>()
                .AddContracts(contracts)
                .BuildCurve();

            double hourlySlope = dailySlope / 24;

            foreach (var pricePair in curve.Data.Zip(curve.Data.Skip(1), 
                        (priceHour1, priceHour2) => new {priceHour1, priceHour2}))
            {
                double hourlyChange = pricePair.priceHour2 - pricePair.priceHour1;
                Assert.AreEqual(hourlySlope, hourlyChange, 1E-12);
            }

        }

        [TestCaseSource(nameof(MonthContractTestCases))]
        public void Build_WithMonthTypeParameter_CurveConsistentWithInputs(List<Contract<Month>> contracts)
        {
            var results = new MaxSmoothnessSplineCurveBuilder<Month>()
                .AddContracts(contracts)
                .WithMultiplySeasonalAdjustment(MonthlySeasonalAdjust)
                // TODO add additive seasonal adjustment
                .WithFrontFirstDerivative(0.8) // TODO pass in as parameter?
                .WithBackFirstDerivative(-1.5) // TODO pass in as parameter?
                .BuildCurve();

            TestHelper.AssertCurveConsistentWithInputs(contracts, period => period.End.Subtract(period.Start).Days, results, Tolerance);
        }

        private double MonthlySeasonalAdjust(Month month)
        {
            switch (month.MonthOfYear)
            {
                case 1:
                    return 1.35;
                case 2:
                    return 1.25;
                case 3:
                    return 1.15;
                case 4:
                    return 1.05;
                case 5:
                    return 0.8;
                case 6:
                    return 0.7;
                case 7:
                    return 0.72;
                case 8:
                    return 0.81;
                case 9:
                    return 0.95;
                case 10:
                    return 1.02;
                case 11:
                    return 1.11;
                case 12:
                    return 1.22;
            }

            throw new ArgumentException("month not valid", nameof(month));
        }

        // TODO how to test that the seasonal adjustment has been correctly applied?
        // TODO test time func and back/front second derivatives

        private static readonly IEnumerable<List<Contract<Month>>> MonthContractTestCases = new[]
        {
            new List<Contract<Month>>
            {
                Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                Contract.CreateForSinglePeriod(Month.CreateApril(2019), 12.1),
                Contract.CreateForSinglePeriod(Month.CreateMay(2019), 14.72),
                Contract.Create(Month.CreateJune(2019), Month.CreateDecember(2019), 15.07),
            },
            // Only 2 contracts
            new List<Contract<Month>>
            {
                Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                Contract.CreateForSinglePeriod(Month.CreateApril(2019), 12.1),
            },
            // Gaps
            new List<Contract<Month>>
            {
                Contract.Create(Month.CreateJanuary(2019), Month.CreateMarch(2019), 13.55),
                // Gap in April
                Contract.CreateForSinglePeriod(Month.CreateMay(2019), 14.72),
                Contract.Create(Month.CreateJune(2019), Month.CreateDecember(2019), 15.07),
                // Gap Jan-Feb
                Contract.CreateForSinglePeriod(Month.CreateMarch(2020), 16.02),
                // Gap Apr-Oct
                Contract.Create(Month.CreateNovember(2020), Month.CreateDecember(2020), 20.17),
            }
        };

        [Test]
        public void Build_WithOverlappingContracts_ThrowArgumentException()
        {
            var builder = new MaxSmoothnessSplineCurveBuilder<Month>()
                .AddContract(Month.CreateJanuary(2019), Month.CreateMarch(2019), 19.8)
                .AddContract(Month.CreateMarch(2019), 15.2)
                .AddContract(Month.CreateApril(2019), 10.02);

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo(
                    "contracts are overlapping"),
                () => builder.BuildCurve());
        }
        
    }
}
