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
    public partial class MonthTest
    {

        #region Comparison and Equality

        [TestCaseSource(nameof(MonthTestItems))]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Month> testItem)
        {
            var month1 = testItem.Create();
            var month2 = testItem.Create();
            var equals = month1.Equals(month2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Month> testItem)
        {
            object month1 = testItem.Create();
            object month2 = testItem.Create();
            var equals = month1.Equals(month2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(NonEqualMonthPairTestItems))]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            var equals = month1.Equals(month2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(NonEqualMonthPairTestItems))]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Month> pairTestItem)
        {
            (object month1, object month2) = pairTestItem.CreatePair();
            var equals = month1.Equals(month2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void ObjectEquals_WithParameterNotOfTypeMonth_ReturnsFalse(ITestItem<Month> testItem)
        {
            var month = testItem.Create();
            object obj = new object();
            var equals = month.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(ITestItem<Month> testItem)
        {
            var month1 = testItem.Create();
            var month2 = testItem.Create();
            Assert.AreEqual(month1.GetHashCode(), month2.GetHashCode());
        }

        [TestCaseSource(nameof(NonEqualMonthPairTestItems))]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.AreNotEqual(month1.GetHashCode(), month2.GetHashCode());
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Month> testItem)
        {
            var month1 = testItem.Create();
            var month2 = testItem.Create();
            Assert.IsTrue(month1 == month2);
        }

        [TestCaseSource(nameof(NonEqualMonthPairTestItems))]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsFalse(month1 == month2);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(ITestItem<Month> testItem)
        {
            var month1 = testItem.Create();
            var month2 = testItem.Create();
            Assert.IsFalse(month1 != month2);
        }

        [TestCaseSource(nameof(NonEqualMonthPairTestItems))]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsTrue(month1 != month2);
        }

        [TestCaseSource(nameof(Month1LaterThanMonth2))]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsTrue(month1 > month2);
        }

        [TestCaseSource(nameof(Month1EarlierThanMonth2))]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsFalse(month1 > month2);
        }

        [TestCaseSource(nameof(Month1EarlierThanMonth2))]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsTrue(month1 < month2);
        }

        [TestCaseSource(nameof(Month1LaterThanOrEqualToMonth2))]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsFalse(month1 < month2);
        }

        [TestCaseSource(nameof(Month1LaterThanOrEqualToMonth2))]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsTrue(month1 >= month2);
        }

        [TestCaseSource(nameof(Month1EarlierThanMonth2))]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsFalse(month1 >= month2);
        }

        [TestCaseSource(nameof(Month1EarlierThanOrEqualToMonth2))]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsTrue(month1 <= month2);
        }

        [TestCaseSource(nameof(Month1LaterThanMonth2))]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            Assert.IsFalse(month1 <= month2);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(ITestItem<Month> testItem)
        {
            var month1 = testItem.Create();
            var month2 = testItem.Create();
            var comp = month1.CompareTo(month2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Month1LaterThanMonth2))]
        public void CompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            var comp = month1.CompareTo(month2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Month1EarlierThanMonth2))]
        public void CompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Month> pairTestItem)
        {
            var (month1, month2) = pairTestItem.CreatePair();
            var comp = month1.CompareTo(month2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void IComparableCompareTo_WithInstanceIdenticalToParameter_ReturnsZero(ITestItem<Month> testItem)
        {
            var month1 = testItem.Create();
            var month2 = testItem.Create();
            var comp = month1.CompareTo(month2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Month1LaterThanMonth2))]
        public void IComparableCompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Month> pairTestItem)
        {
            (IComparable month1, object month2) = pairTestItem.CreatePair();
            var comp = month1.CompareTo(month2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Month1EarlierThanMonth2))]
        public void IComparableCompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Month> pairTestItem)
        {
            (IComparable month1, object month2) = pairTestItem.CreatePair();
            var comp = month1.CompareTo(month2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void IComparableCompareTo_WithParameterNotOfMonthType_ThrowsArgumentException(ITestItem<Month> testItem)
        {
            IComparable month = testItem.Create();
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => month.CompareTo(obj));
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(ITestItem<Month> testItem)
        {
            IComparable month = testItem.Create();
            var comp = month.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region IXmlSerializable

        [TestCaseSource(nameof(MonthTestItems))]
        public void IXmlSerializable_Roundtrip_Completes(ITestItem<Month> testItem)
        {
            var month = testItem.Create();
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(month);
        }

        [TestCaseSource(nameof(MonthTestItems))]
        public void GetSchema_ReturnsNull(ITestItem<Month> testItem)
        {
            IXmlSerializable month = testItem.Create();
            Assert.IsNull(month.GetSchema());
        }

        #endregion IXmlSerializable

		#region Binary Serializable
		
		[TestCaseSource(nameof(MonthTestItems))]
        public void BinarySerialization_Roundtrip_Completes(ITestItem<Month> testItem)
        {
            var month = testItem.Create();
            TestHelper.AssertTimePeriodBinarySerializationRoundTripSuccess(month);
        }
		
		#endregion Binary Serializable

    }
}

