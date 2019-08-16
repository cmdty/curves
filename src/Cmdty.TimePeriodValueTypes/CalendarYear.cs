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
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Cmdty.TimePeriodValueTypes
{
    /// <summary>
    /// Represents a single calendar year, i.e. the period which starts at the beginning of the 1st of January, and ends
    /// at the end of the 31st of December.
    /// </summary>
    [Serializable]
    public struct CalendarYear : ITimePeriod<CalendarYear>
    {
        #region Fields
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private ushort _value;  // Not readonly to avoid defensive copying (see https://blog.nodatime.org/2014/07/micro-optimization-surprising.html)
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Constructs an instance of <see cref="CalendarYear"/> from an integer year number.
        /// </summary>
        /// <param name="year">The number of the year being constructed. Must be in the inclusive range 1 to 9998.</param>
        /// <exception cref="ArgumentOutOfRangeException">The year parameter is outside of the range of permissible years.</exception>
        public CalendarYear(int year)
        {
            Preconditions.CheckArgumentOutOfRange(nameof(year), year, Constants.MinCalendarYearNum, Constants.MaxCalendarYearNum);
            _value = (ushort)(year - 1);
        }
        #endregion Constructors

        #region Instance Properties
        /// <summary>
        /// Gets the integer number representation of the <see cref="CalendarYear"/>.
        /// </summary>
        public int Year => _value + 1;

        /// <summary>
        /// Gets the start date and time of the calendar year as a DateTime instance, i.e. 
        /// 00:00 on the 1st of January.
        /// </summary>
        public DateTime Start => new DateTime(Year, 1, 1);

        /// <summary>
        /// Gets the exclusive end date and time of the calendar year as a DateTime instance. As this is the exclusive
        /// end it will be equal to the start date time of the following calendar year.
        /// </summary>
        public DateTime End => new DateTime(Year + 1, 1, 1);
        #endregion Instance Properties

        #region Static Properties
        /// <summary>
        /// Gets the minimum (earliest) value that <see cref="CalendarYear"/> can be constructed as, this being the calendar year 1 AD.
        /// </summary>
        public static CalendarYear MinCalendarYear => new CalendarYear();

        /// <summary>
        /// Gets the maximum (latest) value that <see cref="CalendarYear"/> can be constructed as, this being the calendar year 9998 AD.
        /// </summary>
        public static CalendarYear MaxCalendarYear => new CalendarYear(Constants.MaxCalendarYearNum);
        #endregion Static Properties

        #region Instance Methods

        /// <summary>
        /// Returns the first instance of time period type <typeparamref name="T"/> which sits within the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the time period being returns.</typeparam>
        /// <returns>The first instance of <typeparamref name="T"/> which sit within the <see cref="CalendarYear"/>
        /// represented by the current instance.</returns>
        public T First<T>() where T : ITimePeriod<T>
        {
            return TimePeriodHelper.First<CalendarYear, T>(this);
        }

        /// <summary>
        /// Returns the last instance of time period type <typeparamref name="T"/> which sits within the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the time period being returns.</typeparam>
        /// <returns>The last instance of <typeparamref name="T"/> which sit within the <see cref="CalendarYear"/>
        /// represented by the current instance.</returns>
        public T Last<T>() where T : ITimePeriod<T>
        {
            return TimePeriodHelper.Last<CalendarYear, T>(this);
        }

        /// <summary>
        /// Expands the current instance into a collection of instances of a smaller time period type, which fit
        /// within the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the time period being expanded to.</typeparam>
        /// <returns>A collection of all instances of <typeparamref name="T"/> which fit within the <see cref="CalendarYear"/>
        /// represented by the current instance.</returns>
        public IEnumerable<T> Expand<T>() where T : ITimePeriod<T>
        {
            return TimePeriodHelper.Expand<CalendarYear, T>(this);
        }

        /// <summary>
        /// Compares this instance with another <see cref="CalendarYear"/> and indicates whether this instance is
        /// earlier than, equal to, or later than the other instance.
        /// </summary>
        /// <param name="other">The calendar year being compared to.</param>
        /// <returns>A positive number if this instance is later than <paramref name="other"/>, zero if this instance is
        /// equal to <paramref name="other"/>, and a negative number if this instance is earlier than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(CalendarYear other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return _value.CompareTo(other._value);
        }

        /// <summary>
        /// Compares this instance with another boxed <see cref="CalendarYear"/> and indicates whether this instance is
        /// earlier than, equal to, or later than the other instance.
        /// </summary>
        /// <remarks>Explicit interface implementation of the CompareTo method of the non-generic IComparable interface.
        /// The calling of this method should generally be avoided in favour of the CompareTo method with parameter of
        /// type <see cref="CalendarYear"/> as this is both type safe, and does not involve boxing of the instance being compared to.</remarks>
        /// <param name="obj">The object being compared to.</param>
        /// <returns>Integer returned (if <paramref name="obj"/> is non-null and of type <see cref="CalendarYear"/>)
        /// is a positive number if this instance is later than <paramref name="obj"/>, zero if this instance is
        /// equal to <paramref name="obj"/>, and a negative number if this instance is earlier than <paramref name="obj"/>.
        /// If <paramref name="obj"/> is null then this instance is considered later than <paramref name="obj"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null and not of type <see cref="CalendarYear"/>.
        /// </exception>
        [Pure]
        int IComparable.CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (!(obj is CalendarYear calendarYear))
                throw new ArgumentException("Object must be of type AnotherTimePeriod.CalendarYear.", nameof(obj));
            return CompareTo(calendarYear);
        }

        /// <summary>
        /// Evaluates equality with another <see cref="CalendarYear"/> instance.
        /// </summary>
        /// <param name="other">The calendar year being compared to.</param>
        /// <returns>True if <paramref name="other"/> represents the same year as the current instance,
        /// false otherwise.</returns>
        [Pure]
        public bool Equals(CalendarYear other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return _value.Equals(other._value);
        }

        /// <summary>
        /// Calculates the <see cref="CalendarYear"/> a specified number of years after or before the current instance.
        /// </summary>
        /// <param name="numPeriods">The number of years to offset the current instance by.</param>
        /// <returns>The <see cref="CalendarYear"/> equal to this instance offset by <paramref name="numPeriods"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="CalendarYear"/> being <paramref name="numPeriods"/> years after the current instance if <paramref name="numPeriods"/>
        /// is positive, and the absolute value of <paramref name="numPeriods"/> years before the current instance if
        /// <paramref name="numPeriods"/> is negative.</remarks>
        [Pure]
        public CalendarYear Offset(int numPeriods)
        {
            return new CalendarYear(_value + 1 + numPeriods);
        }

        /// <summary>
        /// Calculates the number of calendar years difference between this instance and another <see cref="CalendarYear"/>.
        /// </summary>
        /// <param name="other">The calendar year the difference is calculated from.</param>
        /// <returns>The number of calendar years difference between this instance and <paramref name="other"/>.</returns>
        /// <remarks>This will be a positive number if <paramref name="other"/> is earlier than, and negative if
        /// <paramref name="other"/> is after, the current instance.</remarks>
        [Pure]
        public int OffsetFrom(CalendarYear other)
        {
            return _value - other._value;
        }

        /// <inheritdoc/>
        [Pure]
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc/>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Preconditions.CheckParameterNotNull(reader, nameof(reader));
            string yearText = reader.ReadElementContentAsString();
            var yearNum = int.Parse(yearText);
            this = new CalendarYear(yearNum);
        }

        /// <inheritdoc/>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckParameterNotNull(writer, nameof(writer));
            writer.WriteString(Year.ToString("D", CultureInfo.InvariantCulture)); // Same as ToString, but ToString contract does not assume culture invariance 
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        [Pure]
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _value.GetHashCode();
        }

        /// <summary>
        /// Evaluates equality with another object.
        /// </summary>
        /// <param name="obj">The object being compared to.</param>
        /// <returns>True if <paramref name="obj"/> is of type <see cref="CalendarYear"/> and represents the same
        /// year as the current instance, false otherwise.</returns>
        [Pure]
        public override bool Equals(object obj)
        {
            if (obj is CalendarYear month)
                return Equals(month);
            return false;
        }

        /// <summary>
        /// Converts the current instance to a string representation.
        /// </summary>
        /// <returns>A string representation of the current instance.</returns>
        [Pure]
        public override string ToString()
        {
            return Year.ToString("D", CultureInfo.InvariantCulture); // Using InvariantCulture doesn't seem to be necessary as all cultures result in the same string, but using InvariantCulture is safer and more future-proof
        }
        #endregion Instance Methods

        #region Static Methods

        /// <summary>
        /// Creates an instance of <see cref="CalendarYear"/> from a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> being converted to a calendar year.</param>
        /// <returns>A <see cref="CalendarYear"/> instance representing the calendar year in which
        /// <paramref name="dateTime"/> resides.</returns>
        /// <remarks>The Kind property of <paramref name="dateTime"/> will be ignored and no time zone conversions
        /// will be performed. Simply the Year property of <paramref name="dateTime"/> is using to construct a
        /// <see cref="CalendarYear"/> instance.</remarks>
        public static CalendarYear FromDateTime(DateTime dateTime) => new CalendarYear(dateTime.Year);

        /// <summary>
        /// Adds a specified number of years to a <see cref="CalendarYear"/> instance.
        /// </summary>
        /// <param name="calYear">The calendar year to add to.</param>
        /// <param name="numPeriods">The number of calendar years added.</param>
        /// <returns>The calendar year <paramref name="numPeriods"/> number of years after <paramref name="calYear"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="CalendarYear"/> being <paramref name="numPeriods"/> years after <paramref name="calYear"/> if
        /// <paramref name="numPeriods"/> is positive, and the absolute value of <paramref name="numPeriods"/> years
        /// before <paramref name="calYear"/> if <paramref name="numPeriods"/> is negative.</remarks>
        [Pure]
        public static CalendarYear Add(CalendarYear calYear, int numPeriods) => calYear.Offset(numPeriods);

        /// <summary>
        /// Subtracts a specified number of years from a <see cref="CalendarYear"/> instance.
        /// </summary>
        /// <param name="calYear">The calendar year to subtract from.</param>
        /// <param name="numPeriods">The number of calendar years subtracted.</param>
        /// <returns>The calendar year <paramref name="numPeriods"/> number of years before <paramref name="calYear"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="CalendarYear"/> being <paramref name="numPeriods"/> years before <paramref name="calYear"/> if <paramref name="numPeriods"/>
        /// is positive, and the absolute value of <paramref name="numPeriods"/> years after <paramref name="calYear"/> if
        /// <paramref name="numPeriods"/> is negative.</remarks>
        [Pure]
        public static CalendarYear Subtract(CalendarYear calYear, int numPeriods) => calYear.Offset(-numPeriods);

        /// <summary>
        /// Subtracts one <see cref="CalendarYear"/> instance from another, returning the number of years difference.
        /// </summary>
        /// <param name="left">The calendar year being subtracted from.</param>
        /// <param name="right">The calendar year being subtracted.</param>
        /// <returns>The number of calendar years that <paramref name="left"/> is after <paramref name="right"/>.</returns>
        /// <remarks>This will be a positive number if <paramref name="left"/> is later than <paramref name="right"/>,
        /// and negative if <paramref name="left"/> is earlier than <paramref name="right"/>.</remarks>
        [Pure]
        public static int Subtract(CalendarYear left, CalendarYear right) => left.OffsetFrom(right);

        #endregion Static Methods

        #region Operators
        /// <summary>
        /// Evaluates the equality of two <see cref="CalendarYear"/> instances.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if the two <see cref="CalendarYear"/> instances represent the same year,
        /// false otherwise.</returns>
        public static bool operator ==(CalendarYear left, CalendarYear right) => left.Equals(right);

        /// <summary>
        /// Evaluates the inequality of two <see cref="CalendarYear"/> instances.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if the two <see cref="CalendarYear"/> instances represent different years,
        /// false otherwise.</returns>
        public static bool operator !=(CalendarYear left, CalendarYear right) => !left.Equals(right);

        /// <summary>
        /// Evaluates whether one <see cref="CalendarYear"/> instance is later than another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is later than <paramref name="right"/>, false otherwise.</returns>
        public static bool operator >(CalendarYear left, CalendarYear right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Evaluates whether one <see cref="CalendarYear"/> instance is earlier than another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is earlier than <paramref name="right"/>, false otherwise.</returns>
        public static bool operator <(CalendarYear left, CalendarYear right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Evaluates whether one <see cref="CalendarYear"/> instance is later than or equal to than another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is later than or equal to <paramref name="right"/>, false otherwise.</returns>
        public static bool operator >=(CalendarYear left, CalendarYear right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Evaluates whether one <see cref="CalendarYear"/> instance is earlier than or equal to another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is earlier than or equal to <paramref name="right"/>, false otherwise.</returns>
        public static bool operator <=(CalendarYear left, CalendarYear right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Adds a specified number of years to a <see cref="CalendarYear"/> instance.
        /// </summary>
        /// <param name="calYear">The calendar year to add to.</param>
        /// <param name="numPeriods">The number of calendar years added.</param>
        /// <returns>The calendar year <paramref name="numPeriods"/> number of years after <paramref name="calYear"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="CalendarYear"/> being <paramref name="numPeriods"/> years after <paramref name="calYear"/> if
        /// <paramref name="numPeriods"/> is positive, and the absolute value of <paramref name="numPeriods"/> years
        /// before <paramref name="calYear"/> if <paramref name="numPeriods"/> is negative.</remarks>
        public static CalendarYear operator +(CalendarYear calYear, int numPeriods) => Add(calYear, numPeriods);

        /// <summary>
        /// Subtracts a specified number of years from a <see cref="CalendarYear"/> instance.
        /// </summary>
        /// <param name="calYear">The calendar year to subtract from.</param>
        /// <param name="numPeriods">The number of calendar years subtracted.</param>
        /// <returns>The calendar year <paramref name="numPeriods"/> number of years before <paramref name="calYear"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="CalendarYear"/> being <paramref name="numPeriods"/> years before <paramref name="calYear"/> if <paramref name="numPeriods"/>
        /// is positive, and the absolute value of <paramref name="numPeriods"/> years after <paramref name="calYear"/> if
        /// <paramref name="numPeriods"/> is negative.</remarks>
        public static CalendarYear operator -(CalendarYear calYear, int numPeriods) => Subtract(calYear, numPeriods);

        /// <summary>
        /// Subtracts one <see cref="CalendarYear"/> instance from another, returning the number of years difference.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>The number of calendar years that <paramref name="left"/> is after <paramref name="right"/>.</returns>
        /// <remarks>This will be a positive number if <paramref name="left"/> is later than <paramref name="right"/>,
        /// and negative if <paramref name="left"/> is earlier than <paramref name="right"/>.</remarks>
        public static int operator -(CalendarYear left, CalendarYear right) => Subtract(left, right);

        /// <summary>
        /// Increments a <see cref="CalendarYear"/> instance by one year.
        /// </summary>
        /// <param name="calYear">The calendar year being incremented.</param>
        /// <returns>The calendar year immediately following <paramref name="calYear"/>.</returns>
        public static CalendarYear operator ++(CalendarYear calYear) => calYear.Offset(1);

        /// <summary>
        /// Decrements a <see cref="CalendarYear"/> instance by one year.
        /// </summary>
        /// <param name="calYear">The calendar year being decremented.</param>
        /// <returns>The calendar year immediately preceding <paramref name="calYear"/>.</returns>
        public static CalendarYear operator --(CalendarYear calYear) => calYear.Offset(-1);

        #endregion

    }
}
