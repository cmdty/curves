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
using System.Linq;
using Cmdty.TimePeriodValueTypes;
using JetBrains.Annotations;

namespace Cmdty.Curves
{
    public static class Weighting
    {

        public static Func<T, double> BusinessDayCount<T>([NotNull] IEnumerable<Day> holidays)
                where T : ITimePeriod<T>
        {
            if (holidays == null) throw new ArgumentNullException(nameof(holidays));
            return period =>
            {
                // TODO put in check for T being Day type and avoid some execution?
                var startDay = Day.FromDateTime(period.Start);
                var endDay = Day.FromDateTime(period.End).Offset(-1);
                return startDay.EnumerateBusinessDays(endDay, holidays).Count();
            };
        }

        public static double EqualWeighting<T>(T period) where T : ITimePeriod<T> => 1.0;

        // TODO hour count (includes clock changes)- could be hard
        // TODO peak hour count
        // TODO off-peak hour count

    }
}
