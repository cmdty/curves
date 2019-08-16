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
    // TODO rename to reflect fact that it is UTC hour, maybe "UtcHour", or "LocalHour" (like NodaTime)
    [Serializable]
    public partial struct Hour : ITimePeriod<Hour>, IFormattable
    {

        #region Constructors

        // TODO think about whether hour should for numbers 1-24 or 0-23
        public Hour(int year, int month, int day, int hour)
        {
            // TODO if don't get rid of use of DateTime here then probably can get rid of these precondition checks as the DateTime constructor will perform the same checks
            Preconditions.CheckArgumentOutOfRange(nameof(year), year, Constants.MinCalendarYearNum, Constants.MaxCalendarYearNum);
            Preconditions.CheckArgumentOutOfRange(nameof(month), month, 1, 12);
            TimePeriodHelper.CheckDayOfMonth(nameof(day), year, month, day);
            Preconditions.CheckArgumentOutOfRange(nameof(hour), hour, 0, 23);

            // TODO need to create a method which calculates different in days which doesn't used DateTime
            var dateTime = new DateTime(year, month, day, hour, 0, 0);
            _value = (int)dateTime.Subtract(FirstDateTime).TotalHours;
        }

        private static readonly DateTime FirstDateTime = new DateTime(1, 1, 1);

        private Hour(int value)
        {
            // TODO check that value is in permissible range
            _value = value;
        }

        #endregion Constructors

        #region Instance Properties

        public DateTime Start => FirstDateTime.AddHours(_value);

        public DateTime End => FirstDateTime.AddHours(_value + 1);

        public int Year => Start.Year;

        public int MonthOfYear => Start.Month;

        public int DayOfMonth => Start.Day;

        public int HourOfDay => Start.Hour;

        #endregion Instance Properties

        #region Static Methods

        /// <summary>
        /// Creates an instance of <see cref="Hour"/> from a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> being converted to an hour.</param>
        /// <returns>A <see cref="Hour"/> instance representing the hour in which <paramref name="dateTime"/> resides.</returns>
        /// <remarks>The Kind property of <paramref name="dateTime"/> will be ignored and no time zone conversions
        /// will be performed. Simply the Year, Month, Day and Hour properties of <paramref name="dateTime"/> are used to
        /// construct an <see cref="Hour"/> instance.</remarks>
        public static Hour FromDateTime(DateTime dateTime) => new Hour(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour);

        // TODO deal with TimeZone/offset portions of text? Remove the effect of the offset?

        /// <summary>
        /// Creates an instance of <see cref="Hour"/> from a string representation using the conventions
        /// of the current thread culture.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <returns>An <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within.</returns>
        public static Hour Parse(string text)
        {
            DateTime dateTime = DateTime.Parse(text);
            return FromDateTime(dateTime);
        }

        // TODO add XML comments to below methods

        /// <summary>
        /// Creates an instance of <see cref="Hour"/> from a string representation using culture-specific format information.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information. If null, the
        /// the current thread's culture is used.</param>
        /// <returns>An <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within.</returns>
        public static Hour Parse(string text, IFormatProvider provider)
        {
            DateTime dateTime = DateTime.Parse(text, provider);
            return FromDateTime(dateTime);
        }

        /// <summary>
        /// Creates an instance of <see cref="Hour"/> from a string representation using culture-specific format information
        /// and a formatting style.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information. If null, the
        /// the current thread's culture is used.</param>
        /// <param name="styles">A bitwise combination of the enumeration values that indicates the style elements
        /// that can be present in <paramref name="text"/> for the parse operation to succeed, and that defines how to
        /// interpret the parsed date in relation to the current time zone or the current date</param>
        /// <returns>An <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within.</returns>
        public static Hour Parse(string text, IFormatProvider provider, DateTimeStyles styles)
        {
            DateTime dateTime = DateTime.Parse(text, provider, styles);
            return FromDateTime(dateTime);
        }

        /// <summary>
        /// Creates an instance of <see cref="Hour"/> from a string representation using a specified format string and
        /// culture-specific format information.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="format">A standard or custom format string for the .NET System.DateTime type.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>An <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within.</returns>
        public static Hour ParseExact(string text, string format, IFormatProvider provider)
        {
            DateTime dateTime = DateTime.ParseExact(text, format, provider);
            return FromDateTime(dateTime);
        }

        /// <summary>
        /// Creates an instance of <see cref="Hour"/> from a string representation using a specified format string,
        /// culture-specific format information and a formatting style.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="format">A standard or custom format string for the .NET System.DateTime type.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="styles">A bitwise combination of the enumeration values that indicates the style elements
        /// that can be present in <paramref name="text"/> for the parse operation to succeed, and that defines how to
        /// interpret the parsed date in relation to the current time zone or the current date</param>
        /// <returns>An <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within.</returns>
        public static Hour ParseExact(string text, string format, IFormatProvider provider, DateTimeStyles styles)
        {
            DateTime dateTime = DateTime.ParseExact(text, format, provider, styles);
            return FromDateTime(dateTime);
        }

        /// <summary>
        /// Creates an instance of <see cref="Hour"/> from a string representation using a specified array of format
        /// strings, culture-specific format information and a formatting style.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="formats">An array of standard or custom format strings for the .NET System.DateTime type.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="styles">A bitwise combination of the enumeration values that indicates the style elements
        /// that can be present in <paramref name="text"/> for the parse operation to succeed, and that defines how to
        /// interpret the parsed date in relation to the current time zone or the current date</param>
        /// <returns>An <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within.</returns>
        public static Hour ParseExact(string text, string[] formats, IFormatProvider provider, DateTimeStyles styles)
        {
            DateTime dateTime = DateTime.ParseExact(text, formats, provider, styles);
            return FromDateTime(dateTime);
        }

        /// <summary>
        /// Tries to create an instance of <see cref="Hour"/> from a string representation and returns a
        /// boolean flag to indicate whether this operation succeeded. Uses the conventions of the current thread culture.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="result">If the parse operation was successful <paramref name="result"/> is set to
        /// an <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within. If the parse operation did not succeed <paramref name="result"/>
        /// is set to the default value for <see cref="Hour"/>.</param>
        /// <returns>True if it was possible to create an <see cref="Hour"/> from <paramref name="text"/>,
        /// false otherwise.</returns>
        public static bool TryParse(string text, out Hour result)
        {
            if (DateTime.TryParse(text, out DateTime dateTime))
            {
                result = FromDateTime(dateTime);
                return true;
            }
            result = new Hour();
            return false;
        }

        /// <summary>
        /// Tries to create an instance of <see cref="Hour"/> from a string representation and returns a
        /// boolean flag to indicate whether this operation succeeded. Uses culture-specific format
        /// information and a formatting style.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="styles">A bitwise combination of the enumeration values that indicates the style elements
        /// that can be present in <paramref name="text"/> for the parse operation to succeed, and that defines how to
        /// interpret the parsed date in relation to the current time zone or the current date</param>
        /// <param name="result">If the parse operation was successful <paramref name="result"/> is set to
        /// an <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within. If the parse operation did not succeed <paramref name="result"/>
        /// is set to the default value for <see cref="Hour"/>.</param>
        /// <returns>True if it was possible to create an <see cref="Hour"/> from <paramref name="text"/>,
        /// false otherwise.</returns>
        public static bool TryParse(string text, IFormatProvider provider, DateTimeStyles styles, out Hour result)
        {
            if (DateTime.TryParse(text, provider, styles, out DateTime dateTime))
            {
                result = FromDateTime(dateTime);
                return true;
            }
            result = new Hour();
            return false;
        }

        /// <summary>
        /// Tries to create an instance of <see cref="Hour"/> from a string representation, following a specific
        /// format, and returns a boolean flag to indicate whether this operation succeeded. Uses culture-specific format
        /// information and a formatting style.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="format">A standard or custom format string for the .NET System.DateTime type.
        /// <paramref name="text"/> must be formatted in line with <paramref name="format"/> in order for
        /// the parse to succeed.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="styles">A bitwise combination of the enumeration values that indicates the style elements
        /// that can be present in <paramref name="text"/> for the parse operation to succeed, and that defines how to
        /// interpret the parsed date in relation to the current time zone or the current date</param>
        /// <param name="result">If the parse operation was successful <paramref name="result"/> is set to
        /// an <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within. If the parse operation did not succeed <paramref name="result"/>
        /// is set to the default value for <see cref="Hour"/>.</param>
        /// <returns>True if it was possible to create an <see cref="Hour"/> from <paramref name="text"/>,
        /// false otherwise.</returns>
        public static bool TryParseExact(string text, string format, IFormatProvider provider, DateTimeStyles styles, 
                                        out Hour result)
        {
            if (DateTime.TryParseExact(text, format, provider, styles, out DateTime dateTime))
            {
                result = FromDateTime(dateTime);
                return true;
            }
            result = new Hour();
            return false;
        }

        /// <summary>
        /// Tries to create an instance of <see cref="Hour"/> from a string representation, following one of an array
        /// of specific formats, and returns a boolean flag to indicate whether this operation succeeded.
        /// Uses culture-specific format information and a formatting style.
        /// </summary>
        /// <param name="text">A string representation of a date time.</param>
        /// <param name="formats">An array of standard or custom format strings for the .NET System.DateTime type.
        /// <paramref name="text"/> must be formatted in line with one of the formats strings in
        /// <paramref name="formats"/> in order for the parse to succeed.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="styles">A bitwise combination of the enumeration values that indicates the style elements
        /// that can be present in <paramref name="text"/> for the parse operation to succeed, and that defines how to
        /// interpret the parsed date in relation to the current time zone or the current date</param>
        /// <param name="result">If the parse operation was successful <paramref name="result"/> is set to
        /// an <see cref="Hour"/> instance for the same hour that the date time, represented by the parameter
        /// <paramref name="text"/>, resides within. If the parse operation did not succeed <paramref name="result"/>
        /// is set to the default value for <see cref="Hour"/>.</param>
        /// <returns>True if it was possible to create an <see cref="Hour"/> from <paramref name="text"/>,
        /// false otherwise.</returns>
        public static bool TryParseExact(string text, string[] formats, IFormatProvider provider, DateTimeStyles styles,
            out Hour result)
        {
            if (DateTime.TryParseExact(text, formats, provider, styles, out DateTime dateTime))
            {
                result = FromDateTime(dateTime);
                return true;
            }
            result = new Hour();
            return false;
        }

        // TODO should above methods check that parsed DateTime is an exact hour, i.e. doesn't contain any minutes?

        #endregion Static Methods

        #region Static Properties

        /// <summary>
        /// Gets the minimum (earliest) value that <see cref="Hour"/> can be constructed as, this being hour zero of the date 1st of January in 1 AD.
        /// </summary>
        public static Hour MinHour => new Hour();

        /// <summary>
        /// Gets the maximum (latest) value that <see cref="Hour"/> can be constructed as, this being hour 23 of the day 31st of December 9998 AD.
        /// </summary>
        public static Hour MaxHour => new Hour(Constants.MaxCalendarYearNum, 12, 31, 23);

        #endregion Static Properties

        #region Instance Methods

        /// <summary>
        /// Converts the current instance to a string representation in the ISO 8601 format.
        /// </summary>
        /// <returns>A string representation of the current instance in the ISO 8601 format.</returns>
        [Pure]
        public override string ToString()
        {
            // TODO think about using a different format? Use a "T" between the date and hour parts?
            (int year, int month, int day, int hour) = this;
            return string.Format(CultureInfo.InvariantCulture, "{0:D4}-{1:D2}-{2:D2} {3:D2}:00", year, month, day, hour); // Using InvariantCulture doesn't seem to be necessary as all cultures result in the same string, but using InvariantCulture is safer and more future-proof
        }

        /// <summary>
        /// Converts the current instance to a string representation using a format string and culture-specific format information.
        /// </summary>
        /// <param name="format">A standard or custom format string for the .NET System.DateTime type.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information. If null, the
        /// the current thread's culture is used.</param>
        /// <returns>A string representation of the current instance, formatted using <paramref name="format"/> and
        /// <paramref name="formatProvider"/>.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Start.ToString(format, formatProvider);
        }

        /// <summary>
        /// Converts the current instance to a string representation using a format string and the formatting conventions
        /// of the current culture.
        /// </summary>
        /// <param name="format">A standard or custom format string for the .NET System.DateTime type.</param>
        /// <returns>A string representation of the current instance, formatted using <paramref name="format"/> and
        /// the current thread's formatting conventions.</returns>
        public string ToString(string format)
        {
            return Start.ToString(format);
        }

        /// <summary>
        /// Deconstructs the current instance into its components
        /// </summary>
        /// <param name="year">The year component</param>
        /// <param name="month">The month of year component</param>
        /// <param name="day">The day of month component</param>
        /// <param name="hour">The hour of day component</param>
        [Pure]
        public void Deconstruct(out int year, out int month, out int day, out int hour)
        {
            DateTime start = Start;
            year = start.Year;
            month = start.Month;
            day = start.Day;
            hour = start.Hour;
        }

        #endregion Instance Methods

    }
}
