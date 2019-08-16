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

namespace Cmdty.TimeSeries.Test
{
    public class DoubleTimeSeriesTest
    {

        [Test]
        public void BuilderBuild_AsExpected()
        {
            var timeSeries = new DoubleTimeSeries<Month>.Builder
                                    {
                                        {Month.CreateJanuary(2019), 1.0},
                                        {Month.CreateFebruary(2019), 2.0},
                                        {Month.CreateMarch(2019), 3.0}
                                    }.Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1.0, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void BuilderBuild_ConstructedWithCapacityParameter_AsExpected()
        {
            var timeSeries = new DoubleTimeSeries<Month>.Builder(2)
            {
                {Month.CreateJanuary(2019), 1.0},
                {Month.CreateFebruary(2019), 2.0},
                {Month.CreateMarch(2019), 3.0}
            }.Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1.0, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void BuilderWithData_AddsPointsToBuiltSeries()
        {
            var timeSeries = new DoubleTimeSeries<Month>.Builder()
                .WithData(Month.CreateJanuary(2019), 1.0)
                .WithData(Month.CreateFebruary(2019), 2.0)
                .WithData(Month.CreateMarch(2019), 3.0)
                .Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1.0, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void BuilderWithData_ConstructedWithCapacityParameter_AddsPointsToBuiltSeries()
        {
            var timeSeries = new DoubleTimeSeries<Month>.Builder(1)
                .WithData(Month.CreateJanuary(2019), 1.0)
                .WithData(Month.CreateFebruary(2019), 2.0)
                .WithData(Month.CreateMarch(2019), 3.0)
                .Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1.0, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2.0, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3.0, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void AddOpDoubleOnRight_AddsNumberToData()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new []{1.0, 2.0, 3.0});
            double doubleToAdd = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleTimeSeries + doubleToAdd;

            foreach ((Month month, double value) in doubleTimeSeriesAfterAdd)
            {
                Assert.AreEqual(doubleTimeSeries[month] + doubleToAdd, value);
            }
            Assert.AreEqual(doubleTimeSeries.Count, doubleTimeSeriesAfterAdd.Count);
        }

        [Test]
        public void AddOpDoubleOnRight_EmptyInstance_ReturnsEmptyInstance()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>();
            double doubleToAdd = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleTimeSeries + doubleToAdd;

            Assert.IsTrue(doubleTimeSeriesAfterAdd.IsEmpty);
        }

        [Test]
        public void AddOpDoubleOnLeft_AddsNumberToData()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            double doubleToAdd = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleToAdd + doubleTimeSeries;

            foreach ((Month month, double value) in doubleTimeSeriesAfterAdd)
            {
                Assert.AreEqual(doubleTimeSeries[month] + doubleToAdd, value);
            }
            Assert.AreEqual(doubleTimeSeries.Count, doubleTimeSeriesAfterAdd.Count);
        }

        [Test]
        public void AddOpDoubleOnLeft_EmptyInstance_ReturnsEmptyInstance()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>();
            double doubleToAdd = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleToAdd + doubleTimeSeries;

            Assert.IsTrue(doubleTimeSeriesAfterAdd.IsEmpty);
        }

        [Test]
        public void MultiplyOpDoubleOnRight_MultipliesDataByNumber()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            double doubleToMultiply = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleTimeSeries * doubleToMultiply;

            foreach ((Month month, double value) in doubleTimeSeriesAfterAdd)
            {
                Assert.AreEqual(doubleTimeSeries[month] * doubleToMultiply, value);
            }
            Assert.AreEqual(doubleTimeSeries.Count, doubleTimeSeriesAfterAdd.Count);
        }

        [Test]
        public void MultiplyOpDoubleOnRight_EmptyInstance_ReturnsEmptyInstance()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>();
            double doubleToMultiply = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterMultiply =  doubleTimeSeries * doubleToMultiply;

            Assert.IsTrue(doubleTimeSeriesAfterMultiply.IsEmpty);
        }

        [Test]
        public void MultiplyOpDoubleOnLeft_MultipliesDataByNumber()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            double doubleToMultiply = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleToMultiply * doubleTimeSeries;

            foreach ((Month month, double value) in doubleTimeSeriesAfterAdd)
            {
                Assert.AreEqual(doubleTimeSeries[month] * doubleToMultiply, value);
            }
            Assert.AreEqual(doubleTimeSeries.Count, doubleTimeSeriesAfterAdd.Count);
        }

        [Test]
        public void MultiplyOpDoubleOnLeft_EmptyInstance_ReturnsEmptyInstance()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>();
            double doubleToMultiply = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterMultiply = doubleToMultiply * doubleTimeSeries;

            Assert.IsTrue(doubleTimeSeriesAfterMultiply.IsEmpty);
        }

        [Test]
        public void SubtractOp_SubtractsNumberFromData()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            double doubleToSubtract = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleTimeSeries - doubleToSubtract;

            foreach ((Month month, double value) in doubleTimeSeriesAfterAdd)
            {
                Assert.AreEqual(doubleTimeSeries[month] - doubleToSubtract, value);
            }
            Assert.AreEqual(doubleTimeSeries.Count, doubleTimeSeriesAfterAdd.Count);
        }

        [Test]
        public void SubtractOp_EmptyInstance_ReturnsEmptyInstance()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>();
            double doubleToSubtract = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterSubtract = doubleTimeSeries - doubleToSubtract;

            Assert.IsTrue(doubleTimeSeriesAfterSubtract.IsEmpty);
        }

        [Test]
        public void DivideOp_DividesNumberFromData()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            double doubleToDivides = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterAdd = doubleTimeSeries / doubleToDivides;

            foreach ((Month month, double value) in doubleTimeSeriesAfterAdd)
            {
                Assert.AreEqual(doubleTimeSeries[month] / doubleToDivides, value);
            }
            Assert.AreEqual(doubleTimeSeries.Count, doubleTimeSeriesAfterAdd.Count);
        }

        [Test]
        public void DivideOp_EmptyInstance_ReturnsEmptyInstance()
        {
            var doubleTimeSeries = new DoubleTimeSeries<Month>();
            double doubleToDivide = 5.5;

            DoubleTimeSeries<Month> doubleTimeSeriesAfterDivide = doubleTimeSeries / doubleToDivide;

            Assert.IsTrue(doubleTimeSeriesAfterDivide.IsEmpty);
        }

        [Test]
        public void AddOpDoubleTimeSeries_AddsDoubles()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            DoubleTimeSeries<Month> doubleTimeSeriesAdded = doubleTimeSeries1 + doubleTimeSeries2;

            Assert.AreEqual(3, doubleTimeSeriesAdded.Count);
            Assert.AreEqual(1.0, doubleTimeSeriesAdded[Month.CreateJanuary(2020)]);
            Assert.AreEqual(3.8, doubleTimeSeriesAdded[Month.CreateFebruary(2020)]);
            Assert.AreEqual(5.1, doubleTimeSeriesAdded[Month.CreateMarch(2020)]);
        }

        [Test]
        public void AddOpDoubleTimeSeries_ParameterIndicesStartEarlier_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateDecember(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
            {
                // ReSharper disable once UnusedVariable
                var doubleTimeSeriesAdded = doubleTimeSeries1 + doubleTimeSeries2;
            });
        }

        [Test]
        public void AddOpDoubleTimeSeries_ParameterIndicesStartLater_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateMarch(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesAdded = doubleTimeSeries1 + doubleTimeSeries2;
                });
        }

        [Test]
        public void AddOpDoubleTimeSeries_LeftOperandIsEmpty_ThrowsInvalidOperationException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>();
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("TimeSeries is empty."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesAdded = doubleTimeSeries1 + doubleTimeSeries2;
                });
        }

        [Test]
        public void AddOpDoubleTimeSeries_RightOperandIsEmpty_ReturnsLeftOperand()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>();

            DoubleTimeSeries<Month> doubleTimeSeriesAdded = doubleTimeSeries1 + doubleTimeSeries2;

            Assert.AreEqual(doubleTimeSeries1, doubleTimeSeriesAdded);
        }

        [Test]
        public void SubtractOpDoubleTimeSeries_SubtractsDoubles()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            DoubleTimeSeries<Month> doubleTimeSeriesSubtracted = doubleTimeSeries1 - doubleTimeSeries2;

            Assert.AreEqual(3, doubleTimeSeriesSubtracted.Count);
            Assert.AreEqual(1.0, doubleTimeSeriesSubtracted[Month.CreateJanuary(2020)]);
            Assert.AreEqual(2.0 - 1.8, doubleTimeSeriesSubtracted[Month.CreateFebruary(2020)]);
            Assert.AreEqual(3.0 - 2.1, doubleTimeSeriesSubtracted[Month.CreateMarch(2020)]);
        }

        [Test]
        public void SubtractOpDoubleTimeSeries_ParameterIndicesStartEarlier_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateDecember(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesSubtracted = doubleTimeSeries1 - doubleTimeSeries2;
                });
        }

        [Test]
        public void SubtractOpDoubleTimeSeries_ParameterIndicesStartLater_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateMarch(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesSubtracted = doubleTimeSeries1 - doubleTimeSeries2;
                });
        }

        [Test]
        public void SubtractOpDoubleTimeSeries_LeftOperandIsEmpty_ThrowsInvalidOperationException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>();
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("TimeSeries is empty."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesSubtracted = doubleTimeSeries1 - doubleTimeSeries2;
                });
        }

        [Test]
        public void SubtractOpDoubleTimeSeries_RightOperandIsEmpty_ReturnsLeftOperand()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>();

            DoubleTimeSeries<Month> doubleTimeSeriesSubtracted = doubleTimeSeries1 - doubleTimeSeries2;

            Assert.AreEqual(doubleTimeSeries1, doubleTimeSeriesSubtracted);
        }

        [Test]
        public void MultiplyOpDoubleTimeSeries_MultipliesDoubles()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            DoubleTimeSeries<Month> doubleTimeSeriesMultiplied = doubleTimeSeries1 * doubleTimeSeries2;

            Assert.AreEqual(3, doubleTimeSeriesMultiplied.Count);
            Assert.AreEqual(1.0, doubleTimeSeriesMultiplied[Month.CreateJanuary(2020)]);
            Assert.AreEqual(2.0 * 1.8, doubleTimeSeriesMultiplied[Month.CreateFebruary(2020)]);
            Assert.AreEqual(3.0 * 2.1, doubleTimeSeriesMultiplied[Month.CreateMarch(2020)]);
        }

        [Test]
        public void MultiplyOpDoubleTimeSeries_ParameterIndicesStartEarlier_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateDecember(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesMultiplied = doubleTimeSeries1 * doubleTimeSeries2;
                });
        }

        [Test]
        public void MultiplyOpDoubleTimeSeries_ParameterIndicesStartLater_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateMarch(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesMultiplied = doubleTimeSeries1 * doubleTimeSeries2;
                });
        }

        [Test]
        public void MultiplyOpDoubleTimeSeries_LeftOperandIsEmpty_ThrowsInvalidOperationException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>();
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("TimeSeries is empty."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesMultiplied = doubleTimeSeries1 * doubleTimeSeries2;
                });
        }

        [Test]
        public void MultiplyOpDoubleTimeSeries_RightOperandIsEmpty_ReturnsLeftOperand()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>();

            DoubleTimeSeries<Month> doubleTimeSeriesMultiplied = doubleTimeSeries1 * doubleTimeSeries2;

            Assert.AreEqual(doubleTimeSeries1, doubleTimeSeriesMultiplied);
        }

        [Test]
        public void DivideOpDoubleTimeSeries_DividesDoubles()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            DoubleTimeSeries<Month> doubleTimeSeriesDivided = doubleTimeSeries1 / doubleTimeSeries2;

            Assert.AreEqual(3, doubleTimeSeriesDivided.Count);
            Assert.AreEqual(1.0, doubleTimeSeriesDivided[Month.CreateJanuary(2020)]);
            Assert.AreEqual(2.0 / 1.8, doubleTimeSeriesDivided[Month.CreateFebruary(2020)]);
            Assert.AreEqual(3.0 / 2.1, doubleTimeSeriesDivided[Month.CreateMarch(2020)]);
        }

        [Test]
        public void DivideOpDoubleTimeSeries_ParameterIndicesStartEarlier_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateDecember(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesDivided = doubleTimeSeries1 / doubleTimeSeries2;
                });
        }

        [Test]
        public void DivideOpDoubleTimeSeries_ParameterIndicesStartLater_ThrowsArgumentException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateMarch(2019), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("DoubleTimeSeries parameter instance has indices which are non-overlapping with the indices of the current instance."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesDivided = doubleTimeSeries1 / doubleTimeSeries2;
                });
        }

        [Test]
        public void DivideOpDoubleTimeSeries_LeftOperandIsEmpty_ThrowsInvalidOperationException()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>();
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>(Month.CreateFebruary(2020), new[] { 1.8, 2.1 });

            Assert.Throws(Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("TimeSeries is empty."),
                () =>
                {
                    // ReSharper disable once UnusedVariable
                    var doubleTimeSeriesDivided = doubleTimeSeries1 / doubleTimeSeries2;
                });
        }

        [Test]
        public void DivideOpDoubleTimeSeries_RightOperandIsEmpty_ReturnsLeftOperand()
        {
            var doubleTimeSeries1 = new DoubleTimeSeries<Month>(Month.CreateJanuary(2020), new[] { 1.0, 2.0, 3.0 });
            var doubleTimeSeries2 = new DoubleTimeSeries<Month>();

            DoubleTimeSeries<Month> doubleTimeSeriesDivided = doubleTimeSeries1 / doubleTimeSeries2;

            Assert.AreEqual(doubleTimeSeries1, doubleTimeSeriesDivided);
        }

        [Test]
        public void Empty_ReturnsEmptyTimeSeries()
        {
            DoubleTimeSeries<Month> emptyTimeSeries = DoubleTimeSeries<Month>.Empty;

            Assert.AreEqual(0, emptyTimeSeries.Count);
            Assert.AreEqual(0, emptyTimeSeries.Indices.Count);
            Assert.AreEqual(0, emptyTimeSeries.Data.Count);
            Assert.IsTrue(emptyTimeSeries.IsEmpty);
        }

    }
}
