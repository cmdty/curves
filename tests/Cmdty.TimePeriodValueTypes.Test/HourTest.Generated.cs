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
    public partial class HourTest
    {

        #region Comparison and Equality

        [TestCaseSource(nameof(HourTestItems))]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Hour> testItem)
        {
            var hour1 = testItem.Create();
            var hour2 = testItem.Create();
            var equals = hour1.Equals(hour2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Hour> testItem)
        {
            object hour1 = testItem.Create();
            object hour2 = testItem.Create();
            var equals = hour1.Equals(hour2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(NonEqualHourPairTestItems))]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            var equals = hour1.Equals(hour2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(NonEqualHourPairTestItems))]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Hour> pairTestItem)
        {
            (object hour1, object hour2) = pairTestItem.CreatePair();
            var equals = hour1.Equals(hour2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void ObjectEquals_WithParameterNotOfTypeHour_ReturnsFalse(ITestItem<Hour> testItem)
        {
            var hour = testItem.Create();
            object obj = new object();
            var equals = hour.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(ITestItem<Hour> testItem)
        {
            var hour1 = testItem.Create();
            var hour2 = testItem.Create();
            Assert.AreEqual(hour1.GetHashCode(), hour2.GetHashCode());
        }

        [TestCaseSource(nameof(NonEqualHourPairTestItems))]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.AreNotEqual(hour1.GetHashCode(), hour2.GetHashCode());
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Hour> testItem)
        {
            var hour1 = testItem.Create();
            var hour2 = testItem.Create();
            Assert.IsTrue(hour1 == hour2);
        }

        [TestCaseSource(nameof(NonEqualHourPairTestItems))]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsFalse(hour1 == hour2);
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(ITestItem<Hour> testItem)
        {
            var hour1 = testItem.Create();
            var hour2 = testItem.Create();
            Assert.IsFalse(hour1 != hour2);
        }

        [TestCaseSource(nameof(NonEqualHourPairTestItems))]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsTrue(hour1 != hour2);
        }

        [TestCaseSource(nameof(Hour1LaterThanHour2))]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsTrue(hour1 > hour2);
        }

        [TestCaseSource(nameof(Hour1EarlierThanHour2))]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsFalse(hour1 > hour2);
        }

        [TestCaseSource(nameof(Hour1EarlierThanHour2))]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsTrue(hour1 < hour2);
        }

        [TestCaseSource(nameof(Hour1LaterThanOrEqualToHour2))]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsFalse(hour1 < hour2);
        }

        [TestCaseSource(nameof(Hour1LaterThanOrEqualToHour2))]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsTrue(hour1 >= hour2);
        }

        [TestCaseSource(nameof(Hour1EarlierThanHour2))]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsFalse(hour1 >= hour2);
        }

        [TestCaseSource(nameof(Hour1EarlierThanOrEqualToHour2))]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsTrue(hour1 <= hour2);
        }

        [TestCaseSource(nameof(Hour1LaterThanHour2))]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            Assert.IsFalse(hour1 <= hour2);
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(ITestItem<Hour> testItem)
        {
            var hour1 = testItem.Create();
            var hour2 = testItem.Create();
            var comp = hour1.CompareTo(hour2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Hour1LaterThanHour2))]
        public void CompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            var comp = hour1.CompareTo(hour2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Hour1EarlierThanHour2))]
        public void CompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Hour> pairTestItem)
        {
            var (hour1, hour2) = pairTestItem.CreatePair();
            var comp = hour1.CompareTo(hour2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void IComparableCompareTo_WithInstanceIdenticalToParameter_ReturnsZero(ITestItem<Hour> testItem)
        {
            var hour1 = testItem.Create();
            var hour2 = testItem.Create();
            var comp = hour1.CompareTo(hour2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Hour1LaterThanHour2))]
        public void IComparableCompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Hour> pairTestItem)
        {
            (IComparable hour1, object hour2) = pairTestItem.CreatePair();
            var comp = hour1.CompareTo(hour2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Hour1EarlierThanHour2))]
        public void IComparableCompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Hour> pairTestItem)
        {
            (IComparable hour1, object hour2) = pairTestItem.CreatePair();
            var comp = hour1.CompareTo(hour2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void IComparableCompareTo_WithParameterNotOfHourType_ThrowsArgumentException(ITestItem<Hour> testItem)
        {
            IComparable hour = testItem.Create();
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => hour.CompareTo(obj));
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(ITestItem<Hour> testItem)
        {
            IComparable hour = testItem.Create();
            var comp = hour.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region IXmlSerializable

        [TestCaseSource(nameof(HourTestItems))]
        public void IXmlSerializable_Roundtrip_Completes(ITestItem<Hour> testItem)
        {
            var hour = testItem.Create();
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(hour);
        }

        [TestCaseSource(nameof(HourTestItems))]
        public void GetSchema_ReturnsNull(ITestItem<Hour> testItem)
        {
            IXmlSerializable hour = testItem.Create();
            Assert.IsNull(hour.GetSchema());
        }

        #endregion IXmlSerializable

		#region Binary Serializable
		
		[TestCaseSource(nameof(HourTestItems))]
        public void BinarySerialization_Roundtrip_Completes(ITestItem<Hour> testItem)
        {
            var hour = testItem.Create();
            TestHelper.AssertTimePeriodBinarySerializationRoundTripSuccess(hour);
        }
		
		#endregion Binary Serializable

    }
}

