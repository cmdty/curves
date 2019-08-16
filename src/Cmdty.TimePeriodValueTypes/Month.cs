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
    /// Represents a single Gregorian calendar month.
    /// </summary>
    [Serializable]
    public partial struct Month : ITimePeriod<Month>
    {
        #region Constants
        private const int MinValue = 0;
        private const int MaxValue = Constants.MaxCalendarYearNum * 12 - 1;
        #endregion Constants
        
        #region Constructors

        /// <summary>
        /// Constructs an instance of <see cref="Month"/> from integer year and month numbers.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <param name="month">The number of the month of the year for the month being constructed, with 1 representing
        /// January, 2 February etc.</param>
        /// <exception cref="ArgumentOutOfRangeException">Either the year or month parameters is outside of the range of permissible
        /// years or months respectively.</exception>
        public Month(int year, int month)
        {
            Preconditions.CheckArgumentOutOfRange(nameof(year), year, Constants.MinCalendarYearNum, Constants.MaxCalendarYearNum);
            Preconditions.CheckArgumentOutOfRange(nameof(month), month, 1, 12);
            _value = (year - 1)*12 + month - 1;
        }

        /// <summary>
        /// Constructs an instance of <see cref="Month"/> by directly setting the private integer _value field.
        /// </summary>
        /// <param name="value">Integer value to assign to private _value field.</param>
        private Month(int value)
        {
            Preconditions.CheckArgumentOutOfRange(nameof(value), value, MinValue, MaxValue);
            _value = value;
        }

        #endregion Constructors

        #region Instance Properties
        /// <summary>
        /// Gets the integer number representation of the calendar year in which the month resides.
        /// </summary>
        public int Year => _value/12 + 1;

        /// <summary>
        /// Gets the integer month number of the month, with 1 representing January, up to 12 for December.
        /// </summary>
        public int MonthOfYear => _value % 12 + 1;

        /// <summary>
        /// Gets the start date and time of the month as a DateTime instance, i.e. 
        /// 00:00 on the 1st day of the month.
        /// </summary>
        public DateTime Start => new DateTime(Year, MonthOfYear, 1);

        /// <summary>
        /// Gets the exclusive end date and time of the month as a DateTime instance. This will be
        /// equal to the start date time of the following month.
        /// </summary>
        public DateTime End
        {
            get
            {
                var thisMonthOfYear = MonthOfYear;
                if (thisMonthOfYear == 12)
                {
                    return new DateTime(Year + 1, 1, 1);
                }
                return new DateTime(Year, thisMonthOfYear + 1, 1);
            }
        }

        #endregion Instance Properties

        #region Static Properties
        /// <summary>
        /// Gets the minimum (earliest) value that <see cref="Month"/> can be constructed as, this being the month January 1 AD.
        /// </summary>
        public static Month MinMonth => new Month();

        /// <summary>
        /// Gets the maximum (latest) value that <see cref="Month"/> can be constructed as, this being the month December 9998 AD.
        /// </summary>
        public static Month MaxMonth => new Month(Constants.MaxCalendarYearNum, 12);
        #endregion Static Properties
        
        #region Static Methods

        /// <summary>
        /// Creates an instance of <see cref="Month"/> from a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> being converted to a month.</param>
        /// <returns>A <see cref="Month"/> instance representing the month in which <paramref name="dateTime"/> resides.</returns>
        /// <remarks>The Kind property of <paramref name="dateTime"/> will be ignored and no time zone conversions
        /// will be performed. Simply the Year and Month properties of <paramref name="dateTime"/> are used to
        /// construct a <see cref="Month"/> instance.</remarks>
        public static Month FromDateTime(DateTime dateTime) => new Month(dateTime.Year, dateTime.Month);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a January in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing January for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateJanuary(int year) => new Month(year, 1);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a February in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing February for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateFebruary(int year) => new Month(year, 2);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a March in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing March for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateMarch(int year) => new Month(year, 3);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a April in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing April for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateApril(int year) => new Month(year, 4);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a May in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing May for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateMay(int year) => new Month(year, 5);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a June in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing June for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateJune(int year) => new Month(year, 6);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a July in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing July for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateJuly(int year) => new Month(year, 7);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a August in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing August for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateAugust(int year) => new Month(year, 8);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a September in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing September for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateSeptember(int year) => new Month(year, 9);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a October in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing October for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateOctober(int year) => new Month(year, 10);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a November in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing November for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateNovember(int year) => new Month(year, 11);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> representing a December in the Gregorian calendar.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <returns>A <see cref="Month"/> instance representing December for the year number specified by
        /// <paramref name="year"/>.</returns>
        public static Month CreateDecember(int year) => new Month(year, 12);

        /// <summary>
        /// Creates an instance of <see cref="Month"/> from a string representation.
        /// </summary>
        /// <param name="text">A string representation of a Month in the format "yyyy-MM".</param>
        /// <returns>A <see cref="Month"/> instance for the same Month that the parameter <paramref name="text"/> represents.</returns>
        public static Month Parse(string text)
        {
            // TODO avoid using DateTime?
            if (!DateTime.TryParseExact(text, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None,
                                        out DateTime dateTime))
            {
                throw new FormatException("String was not recognized as a valid Month.");
            }
            return FromDateTime(dateTime);
        }

        #endregion Static Methods

        #region Instance Methods

        /// <summary>
        /// Converts the current instance to a string representation.
        /// </summary>
        /// <returns>A string representation of the current instance.</returns>
        [Pure]
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:D4}-{1:D2}", Year, MonthOfYear); // Using InvariantCulture doesn't seem to be necessary as all cultures result in the same string, but using InvariantCulture is safer and more future-proof
        }

        /// <summary>
        /// Deconstructs the current instance into its components
        /// </summary>
        /// <param name="year">The year component</param>
        /// <param name="month">The month of year component</param>
        [Pure]
        public void Deconstruct(out int year, out int month)
        {
            year = Year;
            month = MonthOfYear;
        }
        
        #endregion Instance Methods

    }
}
