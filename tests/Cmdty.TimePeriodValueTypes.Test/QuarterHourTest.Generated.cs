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
    public partial class QuarterHourTest
    {

        #region Comparison and Equality

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<QuarterHour> testItem)
        {
            var quarterHour1 = testItem.Create();
            var quarterHour2 = testItem.Create();
            var equals = quarterHour1.Equals(quarterHour2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<QuarterHour> testItem)
        {
            object quarterHour1 = testItem.Create();
            object quarterHour2 = testItem.Create();
            var equals = quarterHour1.Equals(quarterHour2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(NonEqualQuarterHourPairTestItems))]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            var equals = quarterHour1.Equals(quarterHour2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(NonEqualQuarterHourPairTestItems))]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<QuarterHour> pairTestItem)
        {
            (object quarterHour1, object quarterHour2) = pairTestItem.CreatePair();
            var equals = quarterHour1.Equals(quarterHour2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void ObjectEquals_WithParameterNotOfTypeQuarterHour_ReturnsFalse(ITestItem<QuarterHour> testItem)
        {
            var quarterHour = testItem.Create();
            object obj = new object();
            var equals = quarterHour.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(ITestItem<QuarterHour> testItem)
        {
            var quarterHour1 = testItem.Create();
            var quarterHour2 = testItem.Create();
            Assert.AreEqual(quarterHour1.GetHashCode(), quarterHour2.GetHashCode());
        }

        [TestCaseSource(nameof(NonEqualQuarterHourPairTestItems))]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.AreNotEqual(quarterHour1.GetHashCode(), quarterHour2.GetHashCode());
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<QuarterHour> testItem)
        {
            var quarterHour1 = testItem.Create();
            var quarterHour2 = testItem.Create();
            Assert.IsTrue(quarterHour1 == quarterHour2);
        }

        [TestCaseSource(nameof(NonEqualQuarterHourPairTestItems))]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarterHour1 == quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(ITestItem<QuarterHour> testItem)
        {
            var quarterHour1 = testItem.Create();
            var quarterHour2 = testItem.Create();
            Assert.IsFalse(quarterHour1 != quarterHour2);
        }

        [TestCaseSource(nameof(NonEqualQuarterHourPairTestItems))]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarterHour1 != quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1LaterThanQuarterHour2))]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarterHour1 > quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1EarlierThanQuarterHour2))]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarterHour1 > quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1EarlierThanQuarterHour2))]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarterHour1 < quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1LaterThanOrEqualToQuarterHour2))]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarterHour1 < quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1LaterThanOrEqualToQuarterHour2))]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarterHour1 >= quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1EarlierThanQuarterHour2))]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarterHour1 >= quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1EarlierThanOrEqualToQuarterHour2))]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsTrue(quarterHour1 <= quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHour1LaterThanQuarterHour2))]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            Assert.IsFalse(quarterHour1 <= quarterHour2);
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(ITestItem<QuarterHour> testItem)
        {
            var quarterHour1 = testItem.Create();
            var quarterHour2 = testItem.Create();
            var comp = quarterHour1.CompareTo(quarterHour2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(QuarterHour1LaterThanQuarterHour2))]
        public void CompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            var comp = quarterHour1.CompareTo(quarterHour2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(QuarterHour1EarlierThanQuarterHour2))]
        public void CompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<QuarterHour> pairTestItem)
        {
            var (quarterHour1, quarterHour2) = pairTestItem.CreatePair();
            var comp = quarterHour1.CompareTo(quarterHour2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void IComparableCompareTo_WithInstanceIdenticalToParameter_ReturnsZero(ITestItem<QuarterHour> testItem)
        {
            var quarterHour1 = testItem.Create();
            var quarterHour2 = testItem.Create();
            var comp = quarterHour1.CompareTo(quarterHour2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(QuarterHour1LaterThanQuarterHour2))]
        public void IComparableCompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<QuarterHour> pairTestItem)
        {
            (IComparable quarterHour1, object quarterHour2) = pairTestItem.CreatePair();
            var comp = quarterHour1.CompareTo(quarterHour2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(QuarterHour1EarlierThanQuarterHour2))]
        public void IComparableCompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<QuarterHour> pairTestItem)
        {
            (IComparable quarterHour1, object quarterHour2) = pairTestItem.CreatePair();
            var comp = quarterHour1.CompareTo(quarterHour2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void IComparableCompareTo_WithParameterNotOfQuarterHourType_ThrowsArgumentException(ITestItem<QuarterHour> testItem)
        {
            IComparable quarterHour = testItem.Create();
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => quarterHour.CompareTo(obj));
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(ITestItem<QuarterHour> testItem)
        {
            IComparable quarterHour = testItem.Create();
            var comp = quarterHour.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region IXmlSerializable

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void IXmlSerializable_Roundtrip_Completes(ITestItem<QuarterHour> testItem)
        {
            var quarterHour = testItem.Create();
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(quarterHour);
        }

        [TestCaseSource(nameof(QuarterHourTestItems))]
        public void GetSchema_ReturnsNull(ITestItem<QuarterHour> testItem)
        {
            IXmlSerializable quarterHour = testItem.Create();
            Assert.IsNull(quarterHour.GetSchema());
        }

        #endregion IXmlSerializable

		#region Binary Serializable
		
		[TestCaseSource(nameof(QuarterHourTestItems))]
        public void BinarySerialization_Roundtrip_Completes(ITestItem<QuarterHour> testItem)
        {
            var quarterHour = testItem.Create();
            TestHelper.AssertTimePeriodBinarySerializationRoundTripSuccess(quarterHour);
        }
		
		#endregion Binary Serializable

    }
}

