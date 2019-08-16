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
    public partial class QuarterTest
    {

        #region Comparison and Equality

        [TestCaseSource(nameof(QuarterTestItems))]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Quarter> testItem)
        {
            var quarter1 = testItem.Create();
            var quarter2 = testItem.Create();
            var equals = quarter1.Equals(quarter2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Quarter> testItem)
        {
            object quarter1 = testItem.Create();
            object quarter2 = testItem.Create();
            var equals = quarter1.Equals(quarter2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(NonEqualQuarterPairTestItems))]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            var equals = quarter1.Equals(quarter2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(NonEqualQuarterPairTestItems))]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Quarter> pairTestItem)
        {
            (object quarter1, object quarter2) = pairTestItem.CreatePair();
            var equals = quarter1.Equals(quarter2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void ObjectEquals_WithParameterNotOfTypeQuarter_ReturnsFalse(ITestItem<Quarter> testItem)
        {
            var quarter = testItem.Create();
            object obj = new object();
            var equals = quarter.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(ITestItem<Quarter> testItem)
        {
            var quarter1 = testItem.Create();
            var quarter2 = testItem.Create();
            Assert.AreEqual(quarter1.GetHashCode(), quarter2.GetHashCode());
        }

        [TestCaseSource(nameof(NonEqualQuarterPairTestItems))]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.AreNotEqual(quarter1.GetHashCode(), quarter2.GetHashCode());
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Quarter> testItem)
        {
            var quarter1 = testItem.Create();
            var quarter2 = testItem.Create();
            Assert.IsTrue(quarter1 == quarter2);
        }

        [TestCaseSource(nameof(NonEqualQuarterPairTestItems))]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarter1 == quarter2);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(ITestItem<Quarter> testItem)
        {
            var quarter1 = testItem.Create();
            var quarter2 = testItem.Create();
            Assert.IsFalse(quarter1 != quarter2);
        }

        [TestCaseSource(nameof(NonEqualQuarterPairTestItems))]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarter1 != quarter2);
        }

        [TestCaseSource(nameof(Quarter1LaterThanQuarter2))]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarter1 > quarter2);
        }

        [TestCaseSource(nameof(Quarter1EarlierThanQuarter2))]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarter1 > quarter2);
        }

        [TestCaseSource(nameof(Quarter1EarlierThanQuarter2))]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarter1 < quarter2);
        }

        [TestCaseSource(nameof(Quarter1LaterThanOrEqualToQuarter2))]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarter1 < quarter2);
        }

        [TestCaseSource(nameof(Quarter1LaterThanOrEqualToQuarter2))]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarter1 >= quarter2);
        }

        [TestCaseSource(nameof(Quarter1EarlierThanQuarter2))]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarter1 >= quarter2);
        }

        [TestCaseSource(nameof(Quarter1EarlierThanOrEqualToQuarter2))]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarter1 <= quarter2);
        }

        [TestCaseSource(nameof(Quarter1LaterThanQuarter2))]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarter1 <= quarter2);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(ITestItem<Quarter> testItem)
        {
            var quarter1 = testItem.Create();
            var quarter2 = testItem.Create();
            var comp = quarter1.CompareTo(quarter2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Quarter1LaterThanQuarter2))]
        public void CompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            var comp = quarter1.CompareTo(quarter2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Quarter1EarlierThanQuarter2))]
        public void CompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Quarter> pairTestItem)
        {
            var (quarter1, quarter2) = pairTestItem.CreatePair();
            var comp = quarter1.CompareTo(quarter2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void IComparableCompareTo_WithInstanceIdenticalToParameter_ReturnsZero(ITestItem<Quarter> testItem)
        {
            var quarter1 = testItem.Create();
            var quarter2 = testItem.Create();
            var comp = quarter1.CompareTo(quarter2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Quarter1LaterThanQuarter2))]
        public void IComparableCompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Quarter> pairTestItem)
        {
            (IComparable quarter1, object quarter2) = pairTestItem.CreatePair();
            var comp = quarter1.CompareTo(quarter2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Quarter1EarlierThanQuarter2))]
        public void IComparableCompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Quarter> pairTestItem)
        {
            (IComparable quarter1, object quarter2) = pairTestItem.CreatePair();
            var comp = quarter1.CompareTo(quarter2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void IComparableCompareTo_WithParameterNotOfQuarterType_ThrowsArgumentException(ITestItem<Quarter> testItem)
        {
            IComparable quarter = testItem.Create();
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => quarter.CompareTo(obj));
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(ITestItem<Quarter> testItem)
        {
            IComparable quarter = testItem.Create();
            var comp = quarter.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region IXmlSerializable

        [TestCaseSource(nameof(QuarterTestItems))]
        public void IXmlSerializable_Roundtrip_Completes(ITestItem<Quarter> testItem)
        {
            var quarter = testItem.Create();
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(quarter);
        }

        [TestCaseSource(nameof(QuarterTestItems))]
        public void GetSchema_ReturnsNull(ITestItem<Quarter> testItem)
        {
            IXmlSerializable quarter = testItem.Create();
            Assert.IsNull(quarter.GetSchema());
        }

        #endregion IXmlSerializable

		#region Binary Serializable
		
		[TestCaseSource(nameof(QuarterTestItems))]
        public void BinarySerialization_Roundtrip_Completes(ITestItem<Quarter> testItem)
        {
            var quarter = testItem.Create();
            TestHelper.AssertTimePeriodBinarySerializationRoundTripSuccess(quarter);
        }
		
		#endregion Binary Serializable

    }
}

