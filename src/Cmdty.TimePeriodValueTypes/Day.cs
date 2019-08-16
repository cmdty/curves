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
using System.Globalization;
using JetBrains.Annotations;

namespace Cmdty.TimePeriodValueTypes
{
    /// <summary>
    /// Represents a single day in the Gregorian calendar.
    /// </summary>
    [Serializable]
    public partial struct Day : ITimePeriod<Day>
    {

        #region Constructors

        /// <summary>
        /// Constructs an instance of <see cref="Day"/> from integer year, month and day numbers.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <param name="month">The number of the month of the year for the month being constructed, with 1 representing
        /// January, 2 February etc.</param>
        /// <param name="day">The day number of the month for the day being constructed. Must be in the inclusive range 1 to the
        /// number of days in the month.</param>
        /// <exception cref="ArgumentOutOfRangeException">Either the year, month or day parameters is outside of the range of permissible
        /// years or months respectively.</exception>
        public Day(int year, int month, int day)
        {
            // TODO if don't get rid of use of DateTime here then probably can get rid of these precondition checks as the DateTime constructor will perform the same checks
            Preconditions.CheckArgumentOutOfRange(nameof(year), year, Constants.MinCalendarYearNum, Constants.MaxCalendarYearNum);
            Preconditions.CheckArgumentOutOfRange(nameof(month), month, 1, 12);
            TimePeriodHelper.CheckDayOfMonth(nameof(day), year, month, day);

            // TODO need to create a method which calculates different in days which doesn't used DateTime
            var dateTime = new DateTime(year, month, day);
            _value = dateTime.Subtract(FirstDateTime).Days;
        }

        private static readonly DateTime FirstDateTime = new DateTime(1, 1, 1);

        /// <summary>
        /// Constructs an instance of <see cref="Day"/> by directly setting the private integer _value field.
        /// </summary>
        /// <param name="value">Integer value to assign to private _value field.</param>
        private Day(int value)
        {
            // TODO check that value is in permissible range
            _value = value;
        }

        #endregion Constructors

        #region Instance Properties

        public DateTime Start => FirstDateTime.AddDays(_value);

        public DateTime End => FirstDateTime.AddDays(_value + 1);

        public int Year => Start.Year;

        public int MonthOfYear => Start.Month;

        public int DayOfMonth => Start.Day;

        public DayOfWeek DayOfWeek => Start.DayOfWeek; // TODO unit test

        #endregion Instance Properties

        #region Static Properties

        /// <summary>
        /// Gets the minimum (earliest) value that <see cref="Day"/> can be constructed as, this being the day 1st of January in 1 AD.
        /// </summary>
        public static Day MinDay => new Day();

        /// <summary>
        /// Gets the maximum (latest) value that <see cref="Day"/> can be constructed as, this being the day 31st of December 9998 AD.
        /// </summary>
        public static Day MaxDay => new Day(Constants.MaxCalendarYearNum, 12, 31);

        #endregion Static Properties

        #region Static Methods

        /// <summary>
        /// Creates an instance of <see cref="Day"/> from a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> being converted to a day.</param>
        /// <returns>A <see cref="Day"/> instance representing the day in which <paramref name="dateTime"/> resides.</returns>
        /// <remarks>The Kind property of <paramref name="dateTime"/> will be ignored and no time zone conversions
        /// will be performed. Simply the Year, Month and Day properties of <paramref name="dateTime"/> are used to
        /// construct a <see cref="Day"/> instance.</remarks>
        public static Day FromDateTime(DateTime dateTime) => new Day(dateTime.Year, dateTime.Month, dateTime.Day);

        /// <summary>
        /// Creates an instance of <see cref="Day"/> from a string representation.
        /// </summary>
        /// <param name="text">A string representation of a Day in the ISO 8601 date format, i.e. "yyyy-MM-dd".</param>
        /// <returns>A <see cref="Day"/> instance for the same Day that the parameter <paramref name="text"/> represents.</returns>
        public static Day Parse(string text)
        {
            // TODO avoid using DateTime?
            if (!DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, 
                                        out DateTime dateTime))
            {
                throw new FormatException("String was not recognized as a valid Day.");
            }
            return FromDateTime(dateTime);
        }

        #endregion Static Methods

        #region Instance Methods

        /// <summary>
        /// Converts the current instance to a string representation in the ISO 8601 format.
        /// </summary>
        /// <returns>A string representation of the current instance in the ISO 8601 format.</returns>
        [Pure]
        public override string ToString()
        {
            (int year, int month, int day) = this;
            return string.Format(CultureInfo.InvariantCulture, "{0:D4}-{1:D2}-{2:D2}", year, month, day); // Using InvariantCulture doesn't seem to be necessary as all cultures result in the same string, but using InvariantCulture is safer and more future-proof
        }

        /// <summary>
        /// Deconstructs the current instance into its components
        /// </summary>
        /// <param name="year">The year component</param>
        /// <param name="month">The month of year component</param>
        /// <param name="day">The day of month component</param>
        [Pure]
        public void Deconstruct(out int year, out int month, out int day)
        {
            DateTime start = Start;
            year = start.Year;
            month = start.Month;
            day = start.Day;
        }

        #endregion Instance Methods

    }
}
