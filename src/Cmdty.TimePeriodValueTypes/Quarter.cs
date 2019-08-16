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
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Cmdty.TimePeriodValueTypes
{
    [Serializable]
    public partial struct Quarter : ITimePeriod<Quarter>
    {

        #region Constants
        private const int MinValue = 0;
        private const int MaxValue = Constants.MaxCalendarYearNum * 4 - 1;
        #endregion Constants

        #region Constructors

        public Quarter(int year, int quarter)
        {
            Preconditions.CheckArgumentOutOfRange(nameof(year), year, Constants.MinCalendarYearNum, Constants.MaxCalendarYearNum);
            Preconditions.CheckArgumentOutOfRange(nameof(quarter), quarter, 1, 4);
            _value = (year - 1) * 4 + quarter - 1;
        }

        private Quarter(int value)
        {
            _value = value;
        }

        #endregion Constructors

        #region Instance Properties
        /// <summary>
        /// Gets the integer number representation of the calendar year in which the quarter resides.
        /// </summary>
        public int Year => _value / 4 + 1;

        /// <summary>
        /// Gets the integer indicating which quarter of the year the current instance represents.
        /// </summary>
        public int QuarterOfYear => _value % 4 + 1;

        public DateTime Start => new DateTime(Year, QuarterOfYear * 3  - 2, 1);

        public DateTime End
        {
            get
            {
                var thisQuarterOfYear = QuarterOfYear;
                if (thisQuarterOfYear == 4)
                {
                    return new DateTime(Year + 1, 1, 1);
                }
                return new DateTime(Year, thisQuarterOfYear * 3 + 1, 1);
            }
        }

        #endregion Instance Properties

        #region Static Properties

        /// <summary>
        /// Gets the minimum (earliest) value that <see cref="Quarter"/> can be constructed as, this being the first quarter of the year 1 AD.
        /// </summary>
        public static Quarter MinQuarter => new Quarter();

        /// <summary>
        /// Gets the maximum (latest) value that <see cref="Quarter"/> can be constructed as, this being the 4th quarter of the year 9998 AD.
        /// </summary>
        public static Quarter MaxQuarter => new Quarter(Constants.MaxCalendarYearNum, 4);

        #endregion Static Properties

        #region Static Methods

        public static Quarter FromDateTime(DateTime dateTime) => new Quarter(dateTime.Year, (dateTime.Month - 1) / 3 + 1);

        public static Quarter CreateQuarter1(int year) => new Quarter(year, 1);

        public static Quarter CreateQuarter2(int year) => new Quarter(year, 2);

        public static Quarter CreateQuarter3(int year) => new Quarter(year, 3);

        public static Quarter CreateQuarter4(int year) => new Quarter(year, 4);

        public static Quarter Parse([NotNull] string text) // TODO look into performance
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var match = Regex.Match(text, "^Q(?<QuNum>[1-4])-(?<YearNum>\\d{4})$", RegexOptions.Compiled | RegexOptions.CultureInvariant); // TODO look into flags

            if (!match.Success)
                throw new FormatException($"The string '{text}' was not recognized as a valid Quarter.");

            var quarterNumber = int.Parse(match.Groups["QuNum"].Value);
            var yearNumber = int.Parse(match.Groups["YearNum"].Value);

            return new Quarter(yearNumber, quarterNumber);
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
            return string.Format(CultureInfo.InvariantCulture, "Q{0:D1}-{1:D4}", QuarterOfYear, Year); // Using InvariantCulture doesn't seem to be necessary as all cultures result in the same string, but using InvariantCulture is safer and more future-proof
        }

        /// <summary>
        /// Deconstructs the current instance into its components
        /// </summary>
        /// <param name="year">The year component</param>
        /// <param name="quarter">The quarter component</param>
        [Pure]
        public void Deconstruct(out int year, out int quarter)
        {
            year = Year;
            quarter = QuarterOfYear;
        }

        #endregion Instance Methods

    }
}
