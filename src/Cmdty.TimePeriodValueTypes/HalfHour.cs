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
    // TODO rename to reflect fact that it is UTC hour, maybe "UtcHalfHour", or "LocalHalfHour" (like NodaTime)
    [Serializable]
    public partial struct HalfHour : ITimePeriod<HalfHour>, IFormattable
    {

        #region Constructors

        public HalfHour(int year, int month, int day, int hour, int minute)
        {
            // TODO if don't get rid of use of DateTime here then probably can get rid of these precondition checks as the DateTime constructor will perform the same checks
            Preconditions.CheckArgumentOutOfRange(nameof(year), year, Constants.MinCalendarYearNum, Constants.MaxCalendarYearNum);
            Preconditions.CheckArgumentOutOfRange(nameof(month), month, 1, 12);
            TimePeriodHelper.CheckDayOfMonth(nameof(day), year, month, day);
            Preconditions.CheckArgumentOutOfRange(nameof(hour), hour, 0, 23);

            if (minute != 0 && minute != 30)
                throw new ArgumentException("minute argument value must equal either 0 or 30", nameof(minute));

            // TODO need to create a method which calculates different in days which doesn't used DateTime
            var dateTime = new DateTime(year, month, day, hour, minute, 0);
            _value = (int)dateTime.Subtract(FirstDateTime).TotalMinutes / 30;
        }

        private static readonly DateTime FirstDateTime = new DateTime(1, 1, 1);

        private HalfHour(int value)
        {
            // TODO check that value is in permissible range
            _value = value;
        }

        #endregion Constructors

        #region Instance Properties
        public DateTime Start => FirstDateTime.AddMinutes(_value * 30);

        public DateTime End => FirstDateTime.AddMinutes((_value + 1) * 30);

        public int Year => Start.Year;

        public int MonthOfYear => Start.Month;

        public int DayOfMonth => Start.Day;

        public int HourOfDay => Start.Hour;

        public int MinuteOfDay => Start.Minute;

        #endregion Instance Properties

        #region Static Methods

        public static HalfHour FromDateTime(DateTime dateTime) => new HalfHour(dateTime.Year, dateTime.Month, 
                                            dateTime.Day, dateTime.Hour, dateTime.Minute);

        // TODO deal with TimeZone/offset portions of text? Remove the effect of the offset?
        public static HalfHour Parse(string text)
        {
            DateTime dateTime = DateTime.Parse(text);
            return FromDateTime(dateTime);
        }

        // TODO take other parsing static methods from Hour
        #endregion Static Methods

        #region Static Properties

        /// <summary>
        /// Gets the minimum (earliest) value that <see cref="HalfHour"/> can be constructed as, this being the first half-hour of hour zero, of the date 1st of January in 1 AD.
        /// </summary>
        public static HalfHour MinHalfHour => new HalfHour();

        /// <summary>
        /// Gets the maximum (latest) value that <see cref="HalfHour"/> can be constructed as, this being second half-hour of hour 23, of the day 31st of December 9998 AD.
        /// </summary>
        public static HalfHour MaxHalfHour => new HalfHour(Constants.MaxCalendarYearNum, 12, 31, 23, 30);

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
            (int year, int month, int day, int hour, int minute) = this;
            return string.Format(CultureInfo.InvariantCulture, "{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}", year, month, day, hour, minute); // Using InvariantCulture doesn't seem to be necessary as all cultures result in the same string, but using InvariantCulture is safer and more future-proof
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
        /// <param name="minute">The minute of day component</param>
        [Pure]
        public void Deconstruct(out int year, out int month, out int day, out int hour, out int minute)
        {
            DateTime start = Start;
            year = start.Year;
            month = start.Month;
            day = start.Day;
            hour = start.Hour;
            minute = start.Minute;
        }

        #endregion Instance Methods

    }
}
