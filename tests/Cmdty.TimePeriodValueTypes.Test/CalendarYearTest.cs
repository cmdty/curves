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
using System.Xml.Serialization;
using NUnit.Framework;

namespace Cmdty.TimePeriodValueTypes.Test
{
    public sealed class CalendarYearTest
    {

        #region Constructors
        [TestCase(0)]
        [TestCase(-10)]
        [TestCase(-950)]
        public void Constructor_WithYearParameterBeforeOrEqualToZero_ThrowsArgumentOutOfRangeException(int yearNum)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => new CalendarYear(yearNum));
        }

        [TestCase(9999)]
        [TestCase(10564)]
        [TestCase(45669)]
        public void Constructor_WithYearParameterAfterOrEqualTo9999_ThrowsArgumentOutOfRangeException(int yearNum)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => new CalendarYear(yearNum));
        }
        
        [Test]
        public void DefaultConstructor_ReturnsInstanceEqualToYear1()
        {
            var calYear = new CalendarYear();
            var year1 = new CalendarYear(1);
            Assert.AreEqual(year1, calYear);
        }

        #endregion Constructors

        #region Static Factory Methods

        [Test]
        public void FromDateTime_ReturnsCalendarYearForYearOfParameterYear(
                                    [ValueSource(nameof(DateTimesForFromDateTimeTest))] DateTime dateTime)
        {
            var calYear = CalendarYear.FromDateTime(dateTime);
            var expectedCalYear = new CalendarYear(dateTime.Year);
            Assert.AreEqual(expectedCalYear, calYear);
        }

        private static readonly DateTime[] DateTimesForFromDateTimeTest =
        {
            new DateTime(50, 1, 1),
            new DateTime(2018, 12, 15),
            new DateTime(8569, 12, 31)
        };

        #endregion Static Factory Methods

        #region Instance Properties

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void Start_ShouldEqualDateTimeForStartOfYear(int yearNum)
        {
            var calendarYear = new CalendarYear(yearNum);
            var expectedStart = new DateTime(yearNum, 1, 1);
            Assert.AreEqual(expectedStart, calendarYear.Start);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void End_ShouldEqualDateTimeForStartOfNextYear(int yearNum)
        {
            var calendarYear = new CalendarYear(yearNum);
            var expectedEnd = new DateTime(yearNum + 1, 1, 1);
            Assert.AreEqual(expectedEnd, calendarYear.End);
        }

        [Test]
        public void Start_ForMinCalendarYear_EqualsFirstOfJanYear1()
        {
            // Checking that Start doesn't do anything unwanted at the boundaries
            var minCalYear = CalendarYear.MinCalendarYear;
            var firstOfJanYear1 = new DateTime(1, 1, 1);
            Assert.AreEqual(firstOfJanYear1, minCalYear.Start);
        }
        
        [Test]
        public void End_ForMaxCalendarYear_EqualsFirstOfJanYear9999()
        {
            // Checking that End doesn't do anything unwanted at the boundaries
            var maxCalYear = CalendarYear.MaxCalendarYear;
            var firstOfJanYear9999 = new DateTime(9999, 1, 1);
            Assert.AreEqual(firstOfJanYear9999, maxCalYear.End);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void YearProperty_EqualsYearUsedToConstruct(int yearNum)
        {
            var calendarYear = new CalendarYear(yearNum);
            Assert.AreEqual(yearNum, calendarYear.Year);
        }

        #endregion Instance Properties

        #region Static Properties

        [Test]
        public void MinCalendarYear_EqualsYear1()
        {
            var minCalYear = CalendarYear.MinCalendarYear;
            var year1 = new CalendarYear(1);
            Assert.AreEqual(year1, minCalYear);
        }

        [Test]
        public void MaxCalendarYear_EqualsYear9998()
        {
            var maxCalYear = CalendarYear.MaxCalendarYear;
            var year9998 = new CalendarYear(9998);
            Assert.AreEqual(year9998, maxCalYear);
        }

        #endregion Static Properties

        #region Comparison and Equality

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(int yearNum)
        {
            var calYear1 = new CalendarYear(yearNum);
            var calYear2 = new CalendarYear(yearNum);
            var equals = calYear1.Equals(calYear2);
            Assert.IsTrue(equals);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(int yearNum)
        {
            object calYear1 = new CalendarYear(yearNum);
            object calYear2 = new CalendarYear(yearNum);
            var equals = calYear1.Equals(calYear2);
            Assert.IsTrue(equals);
        }

        [TestCase(2010, 2011)]
        [TestCase(5, 9965)]
        [TestCase(150, 132)]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(int yearNum1, int yearNum2)
        {
            var calYear1 = new CalendarYear(yearNum1);
            var calYear2 = new CalendarYear(yearNum2);
            var equals = calYear1.Equals(calYear2);
            Assert.IsFalse(equals);
        }

        [TestCase(2010, 2011)]
        [TestCase(5, 9965)]
        [TestCase(150, 132)]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(int yearNum1, int yearNum2)
        {
            object calYear1 = new CalendarYear(yearNum1);
            object calYear2 = new CalendarYear(yearNum2);
            var equals = calYear1.Equals(calYear2);
            Assert.IsFalse(equals);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void ObjectEquals_WithParameterNotOfTypeCalendarYear_ReturnsFalse(int yearNum)
        {
            object calYear = new CalendarYear(yearNum);
            object obj = new object();
            var equals = calYear.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(int yearNum)
        {
            var calYear1 = new CalendarYear(yearNum);
            var calYear2 = new CalendarYear(yearNum);
            Assert.AreEqual(calYear1.GetHashCode(), calYear2.GetHashCode());
        }

        [TestCase(2010, 2011)]
        [TestCase(5, 9965)]
        [TestCase(150, 132)]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(int yearNum1, int yearNum2)
        {
            var calYear1 = new CalendarYear(yearNum1);
            var calYear2 = new CalendarYear(yearNum2);
            Assert.AreNotEqual(calYear1.GetHashCode(), calYear2.GetHashCode());
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(int yearNum)
        {
            var calYear1 = new CalendarYear(yearNum);
            var calYear2 = new CalendarYear(yearNum);
            Assert.IsTrue(calYear1 == calYear2);
        }

        [TestCase(2002, 1995)]
        [TestCase(508, 2000)]
        [TestCase(2, 1066)]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsFalse(calYear1 == calYear2);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(int yearNum)
        {
            var calYear1 = new CalendarYear(yearNum);
            var calYear2 = new CalendarYear(yearNum);
            Assert.IsFalse(calYear1 != calYear2);
        }

        [TestCase(1999, 2000)]
        [TestCase(2002, 1995)]
        [TestCase(50, 100)]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsTrue(calYear1 != calYear2);
        }

        [TestCase(2000, 1999)]
        [TestCase(2001, 2000)]
        [TestCase(50, 10)]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsTrue(calYear1 > calYear2);
        }

        [TestCase(2010, 2010)]
        [TestCase(100, 2018)]
        [TestCase(2002, 2018)]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsFalse(calYear1 > calYear2);
        }

        [TestCase(2002, 2018)]
        [TestCase(1999, 2000)]
        [TestCase(50, 60)]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsTrue(calYear1 < calYear2);
        }

        [TestCase(2010, 2010)]
        [TestCase(2020, 2018)]
        [TestCase(5, 2)]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsFalse(calYear1 < calYear2);
        }
        
        [TestCase(2010, 2010)]
        [TestCase(9850, 50)]
        [TestCase(2001, 2000)]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsTrue(calYear1 >= calYear2);
        }

        [TestCase(2002, 2018)]
        [TestCase(2000, 2002)]
        [TestCase(100, 2002)]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsFalse(calYear1 >= calYear2);
        }

        [TestCase(2010, 2010)]
        [TestCase(2018, 2018)]
        [TestCase(2002, 2018)]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsTrue(calYear1 <= calYear2);
        }

        [TestCase(2001, 2000)]
        [TestCase(2003, 2001)]
        [TestCase(5, 1)]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(int leftYearNum, int rightYearNum)
        {
            var calYear1 = new CalendarYear(leftYearNum);
            var calYear2 = new CalendarYear(rightYearNum);
            Assert.IsFalse(calYear1 <= calYear2);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(int yearNum)
        {
            var calYear1 = new CalendarYear(yearNum);
            var calYear2 = new CalendarYear(yearNum);
            var comp = calYear1.CompareTo(calYear2);
            Assert.AreEqual(0, comp);
        }

        [TestCase(1992, 1989)]
        [TestCase(2, 1)]
        [TestCase(512, 10)]
        public void CompareTo_WithParameterEarlierThanInstance_ReturnsPositiveNumber(int yearNumInstance, int yearNumParameter)
        {
            var calYear1 = new CalendarYear(yearNumInstance);
            var calYear2 = new CalendarYear(yearNumParameter);
            var comp = calYear1.CompareTo(calYear2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCase(2111, 3000)]
        [TestCase(10, 11)]
        [TestCase(9997, 9998)]
        public void CompareTo_WithParameterLaterThanInstance_ReturnsNegativeNumber(int yearNumInstance, int yearNumParameter)
        {
            var calYear1 = new CalendarYear(yearNumInstance);
            var calYear2 = new CalendarYear(yearNumParameter);
            var comp = calYear1.CompareTo(calYear2);
            Assert.That(comp, Is.LessThan(0));
        }
        
        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void IComparableCompareTo_WithParameterIdenticalToInstance_ReturnsZero(int yearNum)
        {
            IComparable calYear1 = new CalendarYear(yearNum);
            object calYear2 = new CalendarYear(yearNum);
            var comp = calYear1.CompareTo(calYear2);
            Assert.AreEqual(0, comp);
        }

        [TestCase(1992, 1989)]
        [TestCase(2, 1)]
        [TestCase(512, 10)]
        public void IComparableCompareTo_WithParameterEarlierThanInstance_ReturnsPositiveNumber(int yearNumInstance, int yearNumParameter)
        {
            IComparable calYear1 = new CalendarYear(yearNumInstance);
            object calYear2 = new CalendarYear(yearNumParameter);
            var comp = calYear1.CompareTo(calYear2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCase(2111, 3000)]
        [TestCase(10, 11)]
        [TestCase(9997, 9998)]
        public void IComparableCompareTo_WithParameterLaterThanInstance_ReturnsNegativeNumber(int yearNumInstance, int yearNumParameter)
        {
            IComparable calYear1 = new CalendarYear(yearNumInstance);
            object calYear2 = new CalendarYear(yearNumParameter);
            var comp = calYear1.CompareTo(calYear2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void IComparableCompareTo_WithParameterNotOfCalendarYearType_ThrowsArgumentException(int yearNum)
        {
            IComparable calYear = new CalendarYear(yearNum);
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => calYear.CompareTo(obj));
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(int yearNum)
        {
            IComparable calYear = new CalendarYear(yearNum);
            var comp = calYear.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region Time Period Arithmetic
        
        [TestCase(2018, 0)]
        [TestCase(2018, 109)]
        [TestCase(2018, -50)]
        public void Offset_ReturnsYearPlusOffset(int startYearNum, int offset)
        {
            var startYear = new CalendarYear(startYearNum);
            var offsetYear = startYear.Offset(offset);
            int expectedYearNumAfterOffset = startYearNum + offset;
            var expectedOffsetYear = new CalendarYear(expectedYearNumAfterOffset);
            Assert.AreEqual(expectedOffsetYear, offsetYear);
        }
        
        [TestCase(2010, 2010)]
        [TestCase(2016, 2010)]
        [TestCase(2018, 2009)]
        [TestCase(2008, 2009)]
        [TestCase(1979, 2009)]
        public void OffsetFrom_ReturnsDifferenceInYears(int yearNum1, int yearNum2)
        {
            var calYear1 = new CalendarYear(yearNum1);
            var calYear2 = new CalendarYear(yearNum2);
            var diff = calYear1.OffsetFrom(calYear2);
            var expectedDiff = yearNum1 - yearNum2;
            Assert.AreEqual(expectedDiff, diff);
        }

        [TestCase(2010, 4)]
        [TestCase(2018, 100)]
        [TestCase(1999, -2)]
        public void AdditionOperator_ReturnsYearPlusNumYearsAdded(int yearNum, int numTimePeriods)
        {
            var calYear = new CalendarYear(yearNum);
            var calYearAfterAddition = calYear + numTimePeriods;
            int expectedYearNumAfterAddition = yearNum + numTimePeriods;
            var expectedYearAfterAddition = new CalendarYear(expectedYearNumAfterAddition);
            Assert.AreEqual(expectedYearAfterAddition, calYearAfterAddition);
        }

        [TestCase(2010, 2010)]
        [TestCase(2016, 2010)]
        [TestCase(2018, 2009)]
        [TestCase(2008, 2009)]
        [TestCase(1979, 2009)]
        public void MinusOperatorCalYearParameter_ReturnsDifferenceInYearNumbers(int yearNum1, int yearNum2)
        {
            var calYear1 = new CalendarYear(yearNum1);
            var calYear2 = new CalendarYear(yearNum2);
            var yearDifference = calYear1 - calYear2;
            var expectedYearDifference = yearNum1 - yearNum2;
            Assert.AreEqual(expectedYearDifference, yearDifference);
        }

        [TestCase(2010, 4)]
        [TestCase(2018, 100)]
        [TestCase(1999, -2)]
        public void MinusOperatorIntParameter_ReturnsYearMinusYearsSubtracted(int yearNum, int numTimePeriods)
        {
            var calYear = new CalendarYear(yearNum);
            var calYearAfterSubtraction = calYear - numTimePeriods;
            var expectedYearAfterSubtraction = new CalendarYear(yearNum - numTimePeriods);
            Assert.AreEqual(expectedYearAfterSubtraction, calYearAfterSubtraction);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void PlusPlusOperator_EqualsNextCalendarYear(int yearNum)
        {
            var calYear = new CalendarYear(yearNum);
            var nextCalYear = new CalendarYear(yearNum + 1);
            calYear++;
            Assert.AreEqual(nextCalYear, calYear);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void MinusMinusOperator_EqualsPreviousCalendarYear(int yearNum)
        {
            var calYear = new CalendarYear(yearNum);
            var previousCalYear = new CalendarYear(yearNum - 1);
            calYear--;
            Assert.AreEqual(previousCalYear, calYear);
        }

        #endregion Time Period Arithmetic

        #region Formatting and Parsing

        [TestCase(2018, ExpectedResult = "2018")]
        [TestCase(1979, ExpectedResult = "1979")]
        [TestCase(2050, ExpectedResult = "2050")]
        [TestCase(562, ExpectedResult = "562")]
        public string ToString_EqualsYearNumString(int yearNum)
        {
            var calYear = new CalendarYear(yearNum);
            return calYear.ToString();
        }

        #endregion Formatting and Parsing

        #region IXmlSerializable

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void IXmlSerializable_Roundtrip_Completes(int yearNum)
        {
            var calYear = new CalendarYear(yearNum);
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(calYear);
        }

        [TestCase(10)]
        [TestCase(1979)]
        [TestCase(2018)]
        [TestCase(9852)]
        public void GetSchema_ReturnsNull(int yearNum)
        {
            IXmlSerializable calYear = new CalendarYear(yearNum);
            Assert.IsNull(calYear.GetSchema());
        }

        #endregion IXmlSerializable

    }
}
