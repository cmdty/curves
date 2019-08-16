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
using System.Collections.Generic;
using NUnit.Framework;

namespace Cmdty.TimePeriodValueTypes.Test
{
    public sealed class TimePeriodFactoryTest
    {
        // TODO tests for other TimePeriod types

        [TestCaseSource(nameof(ToStringTestItems))]
        public void FromDateTime_MonthTypeParameter_EqualsExpected(DateTime dateTime, Month expectedMonth)
        {
            var month = TimePeriodFactory.FromDateTime<Month>(dateTime);
            Assert.AreEqual(expectedMonth, month);
        }

        private static readonly IEnumerable<object[]> ToStringTestItems = new[]
        {
            new object[]
            {
                new DateTime(2019, 1, 1),
                Month.CreateJanuary(2019)
            },
            new object[]
            {
                new DateTime(2019, 1, 15),
                Month.CreateJanuary(2019)
            },
            new object[]
            {
                new DateTime(2019, 1, 31),
                Month.CreateJanuary(2019)
            },
            new object[]
            {
                new DateTime(2019, 12, 1),
                Month.CreateDecember(2019)
            },
            new object[]
            {
                new DateTime(2019, 12, 15),
                Month.CreateDecember(2019)
            },
            new object[]
            {
                new DateTime(2019, 12, 31),
                Month.CreateDecember(2019)
            }
        };
    }
}
