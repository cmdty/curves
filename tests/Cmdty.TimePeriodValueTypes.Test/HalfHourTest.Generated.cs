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
    public partial class HalfHourTest
    {

        #region Comparison and Equality

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<HalfHour> testItem)
        {
            var halfHour1 = testItem.Create();
            var halfHour2 = testItem.Create();
            var equals = halfHour1.Equals(halfHour2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<HalfHour> testItem)
        {
            object halfHour1 = testItem.Create();
            object halfHour2 = testItem.Create();
            var equals = halfHour1.Equals(halfHour2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(NonEqualHalfHourPairTestItems))]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            var equals = halfHour1.Equals(halfHour2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(NonEqualHalfHourPairTestItems))]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<HalfHour> pairTestItem)
        {
            (object halfHour1, object halfHour2) = pairTestItem.CreatePair();
            var equals = halfHour1.Equals(halfHour2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void ObjectEquals_WithParameterNotOfTypeHalfHour_ReturnsFalse(ITestItem<HalfHour> testItem)
        {
            var halfHour = testItem.Create();
            object obj = new object();
            var equals = halfHour.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(ITestItem<HalfHour> testItem)
        {
            var halfHour1 = testItem.Create();
            var halfHour2 = testItem.Create();
            Assert.AreEqual(halfHour1.GetHashCode(), halfHour2.GetHashCode());
        }

        [TestCaseSource(nameof(NonEqualHalfHourPairTestItems))]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.AreNotEqual(halfHour1.GetHashCode(), halfHour2.GetHashCode());
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<HalfHour> testItem)
        {
            var halfHour1 = testItem.Create();
            var halfHour2 = testItem.Create();
            Assert.IsTrue(halfHour1 == halfHour2);
        }

        [TestCaseSource(nameof(NonEqualHalfHourPairTestItems))]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(halfHour1 == halfHour2);
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(ITestItem<HalfHour> testItem)
        {
            var halfHour1 = testItem.Create();
            var halfHour2 = testItem.Create();
            Assert.IsFalse(halfHour1 != halfHour2);
        }

        [TestCaseSource(nameof(NonEqualHalfHourPairTestItems))]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(halfHour1 != halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1LaterThanHalfHour2))]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(halfHour1 > halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1EarlierThanHalfHour2))]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(halfHour1 > halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1EarlierThanHalfHour2))]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(halfHour1 < halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1LaterThanOrEqualToHalfHour2))]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(halfHour1 < halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1LaterThanOrEqualToHalfHour2))]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(halfHour1 >= halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1EarlierThanHalfHour2))]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(halfHour1 >= halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1EarlierThanOrEqualToHalfHour2))]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(halfHour1 <= halfHour2);
        }

        [TestCaseSource(nameof(HalfHour1LaterThanHalfHour2))]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(halfHour1 <= halfHour2);
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(ITestItem<HalfHour> testItem)
        {
            var halfHour1 = testItem.Create();
            var halfHour2 = testItem.Create();
            var comp = halfHour1.CompareTo(halfHour2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(HalfHour1LaterThanHalfHour2))]
        public void CompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            var comp = halfHour1.CompareTo(halfHour2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(HalfHour1EarlierThanHalfHour2))]
        public void CompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<HalfHour> pairTestItem)
        {
            var (halfHour1, halfHour2) = pairTestItem.CreatePair();
            var comp = halfHour1.CompareTo(halfHour2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void IComparableCompareTo_WithInstanceIdenticalToParameter_ReturnsZero(ITestItem<HalfHour> testItem)
        {
            var halfHour1 = testItem.Create();
            var halfHour2 = testItem.Create();
            var comp = halfHour1.CompareTo(halfHour2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(HalfHour1LaterThanHalfHour2))]
        public void IComparableCompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<HalfHour> pairTestItem)
        {
            (IComparable halfHour1, object halfHour2) = pairTestItem.CreatePair();
            var comp = halfHour1.CompareTo(halfHour2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(HalfHour1EarlierThanHalfHour2))]
        public void IComparableCompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<HalfHour> pairTestItem)
        {
            (IComparable halfHour1, object halfHour2) = pairTestItem.CreatePair();
            var comp = halfHour1.CompareTo(halfHour2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void IComparableCompareTo_WithParameterNotOfHalfHourType_ThrowsArgumentException(ITestItem<HalfHour> testItem)
        {
            IComparable halfHour = testItem.Create();
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => halfHour.CompareTo(obj));
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(ITestItem<HalfHour> testItem)
        {
            IComparable halfHour = testItem.Create();
            var comp = halfHour.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region IXmlSerializable

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void IXmlSerializable_Roundtrip_Completes(ITestItem<HalfHour> testItem)
        {
            var halfHour = testItem.Create();
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(halfHour);
        }

        [TestCaseSource(nameof(HalfHourTestItems))]
        public void GetSchema_ReturnsNull(ITestItem<HalfHour> testItem)
        {
            IXmlSerializable halfHour = testItem.Create();
            Assert.IsNull(halfHour.GetSchema());
        }

        #endregion IXmlSerializable

		#region Binary Serializable
		
		[TestCaseSource(nameof(HalfHourTestItems))]
        public void BinarySerialization_Roundtrip_Completes(ITestItem<HalfHour> testItem)
        {
            var halfHour = testItem.Create();
            TestHelper.AssertTimePeriodBinarySerializationRoundTripSuccess(halfHour);
        }
		
		#endregion Binary Serializable

    }
}

