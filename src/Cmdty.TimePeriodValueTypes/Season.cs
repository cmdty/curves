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
    public partial struct Season : ITimePeriod<Season>
    {

        #region Constructors

        public Season(int year, SeasonOfYear seasonOfYear)
        {
            //Preconditions.CheckArgumentOutOfRange(nameof(year), year, Constants.MinCalendarYearNum, Constants.MaxCalendarYearNum);
            // TODO preconditions
            _value = (year - 1) * 2 + (int)seasonOfYear - 1;
        }

        private Season(int value)
        {
            _value = value;
        }

        #endregion Constructors

        #region Instance Properties
        /// <summary>
        /// Gets the integer number representation of the calendar year in which the season resides.
        /// </summary>
        public int Year => _value / 2 + 1;

        public SeasonOfYear SeasonOfYear => (_value & 1) == 0 ? SeasonOfYear.Summer : SeasonOfYear.Winter;
        
        public DateTime Start => new DateTime(Year, (_value & 1) == 0 ? 4 : 10, 1);

        public DateTime End
        {
            get
            {
                if ((_value & 1) == 0) // Summer
                {
                    return new DateTime(Year, 10, 1);
                }
                return new DateTime(Year + 1, 4, 1); // Winter
            }
        }
        
        #endregion Instance Properties

        #region Static Properties

        public static Season MinSeason => new Season(); // Should be Summer year 1
        
        #endregion Static Properties
        
        #region Static Methods

        public static Season FromDateTime(DateTime dateTime)
        {
            if (dateTime.Month >= 4 && dateTime.Month < 10)
            {
                return CreateSummer(dateTime.Year);
            }

            int winterYear = dateTime.Month < 4 ? dateTime.Year - 1 : dateTime.Year;
            return CreateWinter(winterYear);
        }

        public static Season CreateSummer(int year) => new Season(year, SeasonOfYear.Summer);

        public static Season CreateWinter(int year) => new Season(year, SeasonOfYear.Winter);
        
        
        public static Season Parse([NotNull] string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var match = Regex.Match(text, "^(?<SeasonOfYear>Sum|Win)-(?<YearNum>\\d{4})$", RegexOptions.Compiled | RegexOptions.CultureInvariant); // TODO look into flags

            if (!match.Success)
                throw new FormatException($"The string '{text}' was not recognized as a valid Quarter.");

            string seasonOfYearText = match.Groups["SeasonOfYear"].Value;
            var yearNumber = int.Parse(match.Groups["YearNum"].Value);

            var seasonOfYear = seasonOfYearText == "Sum" ? SeasonOfYear.Summer : SeasonOfYear.Winter;

            return new Season(yearNumber, seasonOfYear);
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
            var prefix = (_value & 1) == 0 ? "Sum" : "Win";
            return string.Format(CultureInfo.InvariantCulture, "{0}-{1:D4}", prefix, Year); // Using InvariantCulture doesn't seem to be necessary as all cultures result in the same string, but using InvariantCulture is safer and more future-proof
        }

        /// <summary>
        /// Deconstructs the current instance into its components
        /// </summary>
        /// <param name="year">The year component</param>
        /// <param name="seasonOfYear">The season (summer/winter) component</param>
        [Pure]
        public void Deconstruct(out int year, out SeasonOfYear seasonOfYear)
        {
            year = Year;
            seasonOfYear = SeasonOfYear;
        }

        #endregion Instance Methods

    }

    public enum SeasonOfYear
    {
        Summer = 1,
        Winter = 2
    }

}
