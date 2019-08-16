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
using Cmdty.TimePeriodValueTypes;
using NUnit.Framework;

namespace Cmdty.Curves.Test
{
    [TestFixture]
    public sealed class DoubleCurveTest
    {
        [Test]
        public void BuilderBuild_AsExpected()
        {
            var curve = new DoubleCurve<Month>.Builder(month => 1.0)
                                    {
                                        {Month.CreateJanuary(2019), 1.0},
                                        {Month.CreateFebruary(2019), 2.0},
                                        {Month.CreateMarch(2019), 3.0}
                                    }.Build();

            Assert.AreEqual(3, curve.Count);
            Assert.AreEqual(1.0, curve[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, curve[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, curve[Month.CreateMarch(2019)]);
            Assert.AreEqual(2.0, curve.Price(Quarter.CreateQuarter1(2019)));
        }

        [Test]
        public void BuilderBuild_ConstructedWithCapacityParameter_AsExpected()
        {
            var curve = new DoubleCurve<Month>.Builder(2, month => 1.0)
                                    {
                                        {Month.CreateJanuary(2019), 1.0},
                                        {Month.CreateFebruary(2019), 2.0},
                                        {Month.CreateMarch(2019), 3.0}
                                    }.Build();

            Assert.AreEqual(3, curve.Count);
            Assert.AreEqual(1.0, curve[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, curve[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, curve[Month.CreateMarch(2019)]);
            Assert.AreEqual(2.0, curve.Price(Quarter.CreateQuarter1(2019)));
        }

        [Test]
        public void BuilderWithData_AddsPointsToBuiltSeries()
        {
            var curve = new DoubleCurve<Month>.Builder(month => 1.0)
                .WithData(Month.CreateJanuary(2019), 1.0)
                .WithData(Month.CreateFebruary(2019), 2.0)
                .WithData(Month.CreateMarch(2019), 3.0)
                .Build();

            Assert.AreEqual(3, curve.Count);
            Assert.AreEqual(1.0, curve[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, curve[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, curve[Month.CreateMarch(2019)]);
            Assert.AreEqual(2.0, curve.Price(Quarter.CreateQuarter1(2019)));
        }

        [Test]
        public void BuilderWithData_ConstructedWithCapacityParameter_AddsPointsToBuiltSeries()
        {
            var curve = new DoubleCurve<Month>.Builder(1, month => 1.0)
                .WithData(Month.CreateJanuary(2019), 1.0)
                .WithData(Month.CreateFebruary(2019), 2.0)
                .WithData(Month.CreateMarch(2019), 3.0)
                .Build();

            Assert.AreEqual(3, curve.Count);
            Assert.AreEqual(1.0, curve[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, curve[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, curve[Month.CreateMarch(2019)]);
            Assert.AreEqual(2.0, curve.Price(Quarter.CreateQuarter1(2019)));
        }

        [Test]
        public void PriceStartAndEndParameters_MonthlyCurveBusinessDayWeight_EqualsWeightedAverage()
        {
            var curve = new DoubleCurve<Month>(Month.CreateJanuary(2020), new []{56.54, 54.15, 51.14}, 
                                Weighting.BusinessDayCount<Month>(new Day[0]));

            double febMarPrice = curve.Price(Month.CreateFebruary(2020), Month.CreateMarch(2020));

            const double expectedFebMarPrice = (54.15 * 20.0 + 51.14 * 22.0) / 42.0;
            Assert.AreEqual(expectedFebMarPrice, febMarPrice);
        }

        [Test]
        public void Price_MonthlyCurveBusinessDayWeight_EqualsWeightedAverage()
        {
            var curve = new DoubleCurve<Month>(Month.CreateJanuary(2020), new[] { 56.54, 54.15, 51.14 },
                Weighting.BusinessDayCount<Month>(new Day[0]));

            double q1Price = curve.Price(Quarter.CreateQuarter1(2020));

            const double expectedQ1Price = (56.54 * 23.0 +  54.15 * 20.0 + 51.14 * 22.0) / 65.0;
            Assert.AreEqual(expectedQ1Price, q1Price);
        }

        [Test]
        public void Price_EmptyCurve_ThrowsInvalidOperationException()
        {
            DoubleCurve<Month> emptyCurve = DoubleCurve<Month>.Empty;
            Assert.Throws<InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var price = emptyCurve.Price(Month.CreateJanuary(2020));
            });

        }

        [Test]
        public void Empty_ReturnsEmptyCurve()
        {
            DoubleCurve<Month> emptyCurve = DoubleCurve<Month>.Empty;

            Assert.AreEqual(0, emptyCurve.Count);
            Assert.AreEqual(0, emptyCurve.Indices.Count);
            Assert.AreEqual(0, emptyCurve.Data.Count);
            Assert.IsTrue(emptyCurve.IsEmpty);
        }

        // TODO more unit tests
        // Reading off higher granularity, e.g. Day from Monthly curve. Should this throw exception?
        // Exceptions for period and start/end being outside of the bounds of the curve

    }
}
