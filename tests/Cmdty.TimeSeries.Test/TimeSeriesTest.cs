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
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cmdty.TimePeriodValueTypes;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Cmdty.TimeSeries.Test
{
    public class TimeSeriesTest
    {

        [Test]
        public void BuilderBuild_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>.Builder
                                    {
                                        {Month.CreateJanuary(2019), 1},
                                        {Month.CreateFebruary(2019), 2},
                                        {Month.CreateMarch(2019), 3}
                                    }.Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void BuilderBuild_ConstructedWithCapacityParameter_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>.Builder(2)
            {
                {Month.CreateJanuary(2019), 1},
                {Month.CreateFebruary(2019), 2},
                {Month.CreateMarch(2019), 3}
            }.Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void BuilderWithData_AddsPointsToBuiltSeries()
        {
            var timeSeries = new TimeSeries<Month, int>.Builder()
                .WithData(Month.CreateJanuary(2019), 1)
                .WithData(Month.CreateFebruary(2019), 2)
                .WithData(Month.CreateMarch(2019), 3)
                .Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void BuilderWithData_ConstructedWithCapacityParameter_AddsPointsToBuiltSeries()
        {
            var timeSeries = new TimeSeries<Month, int>.Builder(1)
                .WithData(Month.CreateJanuary(2019), 1)
                .WithData(Month.CreateFebruary(2019), 2)
                .WithData(Month.CreateMarch(2019), 3)
                .Build();

            Assert.AreEqual(3, timeSeries.Count);
            Assert.AreEqual(1, timeSeries[Month.CreateJanuary(2019)]);
            Assert.AreEqual(2, timeSeries[Month.CreateFebruary(2019)]);
            Assert.AreEqual(3, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void Count_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new [] { 9, 10, 11 });
            Assert.AreEqual(3, timeSeries.Count);
        }

        [Test]
        public void IntIndexer_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            Assert.AreEqual(10, timeSeries[1]);
        }

        [Test]
        public void TimePeriodIndexer_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            Assert.AreEqual(11, timeSeries[Month.CreateMarch(2019)]);
        }

        [Test]
        public void Start_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            Assert.AreEqual(Month.CreateJanuary(2019), timeSeries.Start);
        }

        [Test]
        public void End_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            Assert.AreEqual(Month.CreateMarch(2019), timeSeries.End);
        }

        [Test]
        public void Data_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            Assert.AreEqual(new[] { 9, 10, 11 }, timeSeries.Data);
        }

        [Test]
        public void Indices_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            var expectedIndices = new []{Month.CreateJanuary(2019), Month.CreateFebruary(2019), Month.CreateMarch(2019)};
            Assert.AreEqual(expectedIndices, timeSeries.Indices);
        }

        [Test]
        public void GetEnumerator_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });

            int i = 0;
            foreach ((Month indexMonth, int dataInt) in timeSeries)
            {
                if (i == 0)
                {
                    Assert.AreEqual(Month.CreateJanuary(2019), indexMonth);
                    Assert.AreEqual(9, dataInt);
                }
                else if (i == 1)
                {
                    Assert.AreEqual(Month.CreateFebruary(2019), indexMonth);
                    Assert.AreEqual(10, dataInt);
                }
                else if (i == 2)
                {
                    Assert.AreEqual(Month.CreateMarch(2019), indexMonth);
                    Assert.AreEqual(11, dataInt);
                }
                else
                {
                    throw new NUnitException("Loop shouldn't iterate more than three times");
                }
                i++;
            }
        }

        [Test]
        public void IntIndexer_OutsideOfValidRange_ThrowsException()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                // ReSharper disable once UnusedVariable
                int dataValue = timeSeries[4];
            });
        }

        [Test]
        public void TimePeriodIndexer_OutsideOfValidRange_ThrowsIndexOutOfException()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                // ReSharper disable once UnusedVariable
                int dataValue = timeSeries[Month.CreateApril(2019)];
            });
        }

        [Test]
        public void ToString_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() => timeSeries.ToString(-3));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void ToString_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Month, int>(Month.CreateJanuary(2019), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() => timeSeries.ToString(0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void ToStringIndexAndDataIFormattableParameters_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16) , new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() => 
                timeSeries.ToString("G", CultureInfo.CurrentCulture, "G", CultureInfo.CurrentCulture,-2));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void ToStringIndexAndDataIFormattableParameters_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.ToString("G", CultureInfo.CurrentCulture, "G", CultureInfo.CurrentCulture, 0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }
        
        [Test]
        public void ToStringSingleFormatProvider_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.ToString("G", "G", CultureInfo.CurrentCulture, -2));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void ToStringSingleFormatProvider_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.ToString("G", "G", CultureInfo.CurrentCulture, 0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void ToStringFormatStringParams_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.ToString("G", "G", -2));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void ToStringFormatStringParams_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.ToString("G", "G", 0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatIndexFormatProviderParam_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatIndex("G", CultureInfo.CurrentCulture, -2));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatIndexFormatProviderParam_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatIndex("G", CultureInfo.CurrentCulture, 0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatIndex_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatIndex("G", -2));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatIndex_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatIndex("G", 0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatDataFormatProviderParam_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatData("G", CultureInfo.CurrentCulture, -2));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatDataFormatProviderParam_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatData("G", CultureInfo.CurrentCulture, 0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatData_MaxRowsToPrintLessThanMinusOne_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatData("G", -2));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }

        [Test]
        public void FormatData_MaxRowsToPrintEqualToZero_ThrowsArgumentException()
        {
            var timeSeries = new TimeSeries<Hour, int>(new Hour(2019, 5, 30, 16), new[] { 9, 10, 11 });
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                timeSeries.FormatData("G", 0));
            Assert.AreEqual("maxNumRowsToPrint must be either equal to -1, or greater than 0\r\nParameter name: maxNumRowsToPrint", exception.Message);
        }




        [Test]
        public void ToString_CountBelowMaxRowsToPrint_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019), 
                                                        new[] { "1", "2", "3" });
            string timeSeriesToString = timeSeries.ToString();

            string expectedToString =
@"Count = 3
2019-01  1
2019-02  2
2019-03  3";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_CountBelowMaxRowsToPrint_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3" });
            string timeSeriesToString = timeSeries.ToString(5);

            string expectedToString =
@"Count = 3
2019-01  1
2019-02  2
2019-03  3";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_5ElementsMaxRowsToPrint3_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5" });
            string timeSeriesToString = timeSeries.ToString(3);

            string expectedToString =
@"Count = 5
2019-01  1
..........";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_5ElementsMaxRowsToPrint4_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5" });
            string timeSeriesToString = timeSeries.ToString(4);

            string expectedToString =
@"Count = 5
2019-01  1
..........
2019-05  5";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_5ElementsMaxRowsToPrint5_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5" });
            string timeSeriesToString = timeSeries.ToString(5);

            string expectedToString =
@"Count = 5
2019-01  1
2019-02  2
..........
2019-05  5";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }


        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint1_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(1);

            string expectedToString = "Count = 6";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }


        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint2_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(2);

            string expectedToString =
@"Count = 6
.........";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }
        
        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint3_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(3);

            string expectedToString =
@"Count = 6
2019-01  1
..........";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }


        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint4_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(4);

            string expectedToString =
@"Count = 6
2019-01  1
..........
2019-06  6";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint5_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(5);

            string expectedToString =
@"Count = 6
2019-01  1
2019-02  2
..........
2019-06  6";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint6_AsExpected()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(6);

            string expectedToString =
@"Count = 6
2019-01  1
2019-02  2
..........
2019-05  5
2019-06  6";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint7_PrintsAllRows()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(7);

            string expectedToString =
@"Count = 6
2019-01  1
2019-02  2
2019-03  3
2019-04  4
2019-05  5
2019-06  6";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrint8_PrintsAllRows()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(8);

            string expectedToString =
                @"Count = 6
2019-01  1
2019-02  2
2019-03  3
2019-04  4
2019-05  5
2019-06  6";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToStringMaxNumRowsToPrintParam_6ElementsMaxRowsToPrintMinusOne_PrintsAllRows()
        {
            var timeSeries = new TimeSeries<Month, string>(Month.CreateJanuary(2019),
                new[] { "1", "2", "3", "4", "5", "6" });
            string timeSeriesToString = timeSeries.ToString(-1);

            string expectedToString =
                @"Count = 6
2019-01  1
2019-02  2
2019-03  3
2019-04  4
2019-05  5
2019-06  6";

            Assert.AreEqual(expectedToString, timeSeriesToString);
        }

        [Test]
        public void ToString_EmptyTimeSeries_ReturnsCountText()
        {
            var emptyTimeSeries = TimeSeries<Month, string>.Empty;
            string timeSeriesToString = emptyTimeSeries.ToString();
            Assert.AreEqual("Count = 0", timeSeriesToString);
        }

        // TODO parameterise formatting tests?
        // TODO unit tests for other Formatting members

        [Test]
        [Ignore("Failing for .NET Core")] // TODO research: https://docs.microsoft.com/en-us/dotnet/standard/serialization/how-to-determine-if-netstandard-object-is-serializable
        public void BinarySerialization_RoundTrip_Success()
        {
            var timeSeries = new TimeSeries<Month, double>(Month.CreateJanuary(2019), new[] { 9.1, 10.2, 11.4 });
            AssertBinarySerializationRoundTripSuccess(timeSeries);
        }

        // TODO add serialization test under different scenarios
        internal static void AssertBinarySerializationRoundTripSuccess<TIndex, TData>(
            TimeSeries<TIndex, TData> timeSeriesOriginal)
            where TIndex : ITimePeriod<TIndex>
        {
            var binaryFormatter = new BinaryFormatter();
            using (Stream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, timeSeriesOriginal);
                memoryStream.Position = 0;
                TimeSeries<TIndex, TData> timeSeriesDeserialized = (TimeSeries<TIndex, TData>)binaryFormatter.Deserialize(memoryStream);
                CollectionAssert.AreEqual(timeSeriesOriginal.Data, timeSeriesDeserialized.Data);
                CollectionAssert.AreEqual(timeSeriesOriginal.Indices, timeSeriesDeserialized.Indices);
            }
        }

        [Test]
        public void Empty_ReturnsEmptyTimeSeries()
        {
            TimeSeries<Month, int> emptyTimeSeries = TimeSeries<Month, int>.Empty;
            
            Assert.AreEqual(0, emptyTimeSeries.Count);
            Assert.AreEqual(0, emptyTimeSeries.Indices.Count);
            Assert.AreEqual(0, emptyTimeSeries.Data.Count);
            Assert.IsTrue(emptyTimeSeries.IsEmpty);
        }

        [Test]
        public void Start_EmptyTimeSeries_ThrowsInvalidOperationException()
        {
            var emptyTimeSeries = new TimeSeries<Hour, double>();
            Assert.Throws<InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var start = emptyTimeSeries.Start;
            });
        }

        [Test]
        public void End_EmptyTimeSeries_ThrowsInvalidOperationException()
        {
            var emptyTimeSeries = new TimeSeries<Hour, double>();
            Assert.Throws<InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var end = emptyTimeSeries.End;
            });
        }

    }
}
