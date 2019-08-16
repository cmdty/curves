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
    public partial class SeasonTest
    {

        #region Comparison and Equality

        [TestCaseSource(nameof(SeasonTestItems))]
        public void Equals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Season> testItem)
        {
            var season1 = testItem.Create();
            var season2 = testItem.Create();
            var equals = season1.Equals(season2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void ObjectEquals_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Season> testItem)
        {
            object season1 = testItem.Create();
            object season2 = testItem.Create();
            var equals = season1.Equals(season2);
            Assert.IsTrue(equals);
        }

        [TestCaseSource(nameof(NonEqualSeasonPairTestItems))]
        public void Equals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            var equals = season1.Equals(season2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(NonEqualSeasonPairTestItems))]
        public void ObjectEquals_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Season> pairTestItem)
        {
            (object season1, object season2) = pairTestItem.CreatePair();
            var equals = season1.Equals(season2);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void ObjectEquals_WithParameterNotOfTypeSeason_ReturnsFalse(ITestItem<Season> testItem)
        {
            var season = testItem.Create();
            object obj = new object();
            var equals = season.Equals(obj);
            Assert.IsFalse(equals);
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void GetHashCode_OnTwoIdenticalInstances_ReturnSameValue(ITestItem<Season> testItem)
        {
            var season1 = testItem.Create();
            var season2 = testItem.Create();
            Assert.AreEqual(season1.GetHashCode(), season2.GetHashCode());
        }

        [TestCaseSource(nameof(NonEqualSeasonPairTestItems))]
        public void GetHashCode_OnTwoDifferentInstances_ReturnDifferentValue(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.AreNotEqual(season1.GetHashCode(), season2.GetHashCode());
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void EqualityOperator_WithTwoIdenticalInstances_ReturnsTrue(ITestItem<Season> testItem)
        {
            var season1 = testItem.Create();
            var season2 = testItem.Create();
            Assert.IsTrue(season1 == season2);
        }

        [TestCaseSource(nameof(NonEqualSeasonPairTestItems))]
        public void EqualityOperator_WithTwoDifferentInstances_ReturnsFalse(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsFalse(season1 == season2);
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void InequalityOperator_WithTwoIdenticalInstances_ReturnsFalse(ITestItem<Season> testItem)
        {
            var season1 = testItem.Create();
            var season2 = testItem.Create();
            Assert.IsFalse(season1 != season2);
        }

        [TestCaseSource(nameof(NonEqualSeasonPairTestItems))]
        public void InequalityOperator_WithTwoDifferentInstances_ReturnsTrue(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsTrue(season1 != season2);
        }

        [TestCaseSource(nameof(Season1LaterThanSeason2))]
        public void GreaterThanOperator_WithLeftLaterThanRight_ReturnsTrue(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsTrue(season1 > season2);
        }

        [TestCaseSource(nameof(Season1EarlierThanSeason2))]
        public void GreaterThanOperator_WithLeftEarlierThanOrEqualToRight_ReturnsFalse(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsFalse(season1 > season2);
        }

        [TestCaseSource(nameof(Season1EarlierThanSeason2))]
        public void LessThanOperator_WithLeftEarlierThanRight_ReturnsTrue(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsTrue(season1 < season2);
        }

        [TestCaseSource(nameof(Season1LaterThanOrEqualToSeason2))]
        public void LessThanOperator_WithLeftLaterThanOrEqualToRight_ReturnsFalse(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsFalse(season1 < season2);
        }

        [TestCaseSource(nameof(Season1LaterThanOrEqualToSeason2))]
        public void GreaterThanOrEqualToOperator_WithLeftLaterThanOrEqualToRight_ReturnsTrue(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsTrue(season1 >= season2);
        }

        [TestCaseSource(nameof(Season1EarlierThanSeason2))]
        public void GreaterThanOrEqualToOperator_WithLeftEarlierThanRight_ReturnsFalse(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsFalse(season1 >= season2);
        }

        [TestCaseSource(nameof(Season1EarlierThanOrEqualToSeason2))]
        public void LessThanOrEqualToOperator_WithLeftEarlierThanOrEqualToRight_ReturnsTrue(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsTrue(season1 <= season2);
        }

        [TestCaseSource(nameof(Season1LaterThanSeason2))]
        public void LessThanOrEqualToOperator_WithLeftLaterThanRight_ReturnsFalse(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            Assert.IsFalse(season1 <= season2);
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void CompareTo_WithParameterIdenticalToInstance_ReturnsZero(ITestItem<Season> testItem)
        {
            var season1 = testItem.Create();
            var season2 = testItem.Create();
            var comp = season1.CompareTo(season2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Season1LaterThanSeason2))]
        public void CompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            var comp = season1.CompareTo(season2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Season1EarlierThanSeason2))]
        public void CompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Season> pairTestItem)
        {
            var (season1, season2) = pairTestItem.CreatePair();
            var comp = season1.CompareTo(season2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void IComparableCompareTo_WithInstanceIdenticalToParameter_ReturnsZero(ITestItem<Season> testItem)
        {
            var season1 = testItem.Create();
            var season2 = testItem.Create();
            var comp = season1.CompareTo(season2);
            Assert.AreEqual(0, comp);
        }

        [TestCaseSource(nameof(Season1LaterThanSeason2))]
        public void IComparableCompareTo_WithInstanceLaterThanParameter_ReturnsPositiveNumber(ITestItemPair<Season> pairTestItem)
        {
            (IComparable season1, object season2) = pairTestItem.CreatePair();
            var comp = season1.CompareTo(season2);
            Assert.That(comp, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Season1EarlierThanSeason2))]
        public void IComparableCompareTo_WithInstanceEarlierThanParameter_ReturnsNegativeNumber(ITestItemPair<Season> pairTestItem)
        {
            (IComparable season1, object season2) = pairTestItem.CreatePair();
            var comp = season1.CompareTo(season2);
            Assert.That(comp, Is.LessThan(0));
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void IComparableCompareTo_WithParameterNotOfSeasonType_ThrowsArgumentException(ITestItem<Season> testItem)
        {
            IComparable season = testItem.Create();
            object obj = new object();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => season.CompareTo(obj));
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void IComparableCompareTo_WithParameterEqualToNull_Returns1(ITestItem<Season> testItem)
        {
            IComparable season = testItem.Create();
            var comp = season.CompareTo(null);
            Assert.AreEqual(1, comp);
        }

        #endregion Comparison and Equality

        #region IXmlSerializable

        [TestCaseSource(nameof(SeasonTestItems))]
        public void IXmlSerializable_Roundtrip_Completes(ITestItem<Season> testItem)
        {
            var season = testItem.Create();
            TestHelper.AssertTimePeriodXmlSerializationRoundTripSuccess(season);
        }

        [TestCaseSource(nameof(SeasonTestItems))]
        public void GetSchema_ReturnsNull(ITestItem<Season> testItem)
        {
            IXmlSerializable season = testItem.Create();
            Assert.IsNull(season.GetSchema());
        }

        #endregion IXmlSerializable

		#region Binary Serializable
		
		[TestCaseSource(nameof(SeasonTestItems))]
        public void BinarySerialization_Roundtrip_Completes(ITestItem<Season> testItem)
        {
            var season = testItem.Create();
            TestHelper.AssertTimePeriodBinarySerializationRoundTripSuccess(season);
        }
		
		#endregion Binary Serializable

    }
}

