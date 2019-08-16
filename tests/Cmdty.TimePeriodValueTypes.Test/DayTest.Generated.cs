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

// THIS FILE HAS BEEN AUTOMATICALLY GENERATED USING T4. DO NOT MODIFY AS ANY CHANGES WILL BE OVERWRITTEN.

using System;
using NUnit.Framework;
using System.Xml.Serialization;

namespace Cmdty.TimePeriodValueTypes.Test
{
    public partial class DayTest
    {

        #region Comparison and Equality

        [TestCaseSource(nameof(DayTestItems))]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Day> testItem)
        {
            var day1 = testItem.Create();
            var day2 = testItem.Create();
            var equals = day1.Equals(day2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Day> testItem)
        {
            object day1 = testItem.Create();
            object day2 = testItem.Create();
            var equals = day1.Equals(day2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(NonEqualDayPairTestItems))]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            var equals = day1.Equals(day2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(NonEqualDayPairTestItems))]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Day> pairTestItem)
        {
            (object day1, object day2) = pairTestItem.CreatePair();
            var equals = day1.Equals(day2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void ObjectEquals_WithParameterNotOfTypeDay_ReturnsFalse(ITestItem<Day> testItem)
        {
            var day = testItem.Create();
            object obj = new object();
            var equals = day.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(ITestItem<Day> testItem)
        {
            var day1 = testItem.Create();
            var day2 = testItem.Create();
            Assert.AreEqual(day1.GetHashCode(), day2.GetHashCode());
        }

        [TestCaseSource(nameof(NonEqualDayPairTestItems))]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.AreNotEqual(day1.GetHashCode(), day2.GetHashCode());
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Day> testItem)
        {
            var day1 = testItem.Create();
            var day2 = testItem.Create();
            Assert.IsTrue(day1 == day2);
        }

        [TestCaseSource(nameof(NonEqualDayPairTestItems))]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsFalse(day1 == day2);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(ITestItem<Day> testItem)
        {
            var day1 = testItem.Create();
            var day2 = testItem.Create();
            Assert.IsFalse(day1 != day2);
        }

        [TestCaseSource(nameof(NonEqualDayPairTestItems))]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsTrue(day1 != day2);
        }

        [TestCaseSource(nameof(Day1LaterThanDay2))]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsTrue(day1 > day2);
        }

        [TestCaseSource(nameof(Day1EarlierThanDay2))]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsFalse(day1 > day2);
        }

        [TestCaseSource(nameof(Day1EarlierThanDay2))]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsTrue(day1 < day2);
        }

        [TestCaseSource(nameof(Day1LaterThanOrEqualToDay2))]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsFalse(day1 < day2);
        }

        [TestCaseSource(nameof(Day1LaterThanOrEqualToDay2))]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsTrue(day1 >= day2);
        }

        [TestCaseSource(nameof(Day1EarlierThanDay2))]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsFalse(day1 >= day2);
        }

        [TestCaseSource(nameof(Day1EarlierThanOrEqualToDay2))]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsTrue(day1 <= day2);
        }

        [TestCaseSource(nameof(Day1LaterThanDay2))]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            Assert.IsFalse(day1 <= day2);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(ITestItem<Day> testItem)
        {
            var day1 = testItem.Create();
            var day2 = testItem.Create();
            var comp = day1.CompareTo(day2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Day1LaterThanDay2))]
        public void CompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            var comp = day1.CompareTo(day2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Day1EarlierThanDay2))]
        public void CompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Day> pairTestItem)
        {
            var (day1, day2) = pairTestItem.CreatePair();
            var comp = day1.CompareTo(day2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void IComparableCompareTo_WithInstanceIdenticalToParameter_ReturnsZero(ITestItem<Day> testItem)
        {
            var day1 = testItem.Create();
            var day2 = testItem.Create();
            var comp = day1.CompareTo(day2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Day1LaterThanDay2))]
        public void IComparableCompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Day> pairTestItem)
        {
            (IComparable day1, object day2) = pairTestItem.CreatePair();
            var comp = day1.CompareTo(day2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Day1EarlierThanDay2))]
        public void IComparableCompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Day> pairTestItem)
        {
            (IComparable day1, object day2) = pairTestItem.CreatePair();
            var comp = day1.CompareTo(day2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void IComparableCompareTo_WithParameterNotOfDayType_ThrowsArgumentException(ITestItem<Day> testItem)
        {
            IComparable day = testItem.Create();
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => day.CompareTo(obj));
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(ITestItem<Day> testItem)
        {
            IComparable day = testItem.Create();
            var comp = day.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region IXmlSerializable

        [TestCaseSource(nameof(DayTestItems))]
        public void IXmlSerializable_Roundtrip_Completes(ITestItem<Day> testItem)
        {
            var day = testItem.Create();
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(day);
        }

        [TestCaseSource(nameof(DayTestItems))]
        public void GetSchema_ReturnsNull(ITestItem<Day> testItem)
        {
            IXmlSerializable day = testItem.Create();
            Assert.IsNull(day.GetSchema());
        }

        #endregion IXmlSerializable

		#region Binary Serializable
		
		[TestCaseSource(nameof(DayTestItems))]
        public void BinarySerialization_Roundtrip_Completes(ITestItem<Day> testItem)
        {
            var day = testItem.Create();
            TestHelper.AssertTimePeriodBinarySerializationRoundTripSuccess(day);
        }
		
		#endregion Binary Serializable

    }
}

