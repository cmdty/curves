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
using JetBrains.Annotations;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
#if !NET40
using System.Runtime.CompilerServices;
#endif

namespace Cmdty.TimePeriodValueTypes
{
#if NET40
	public partial struct HalfHour
#else
	public readonly partial struct HalfHour
#endif
    {
	    #region Fields
#if NET40
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private int _value; // Not readonly to avoid defensive copying  (see https://blog.nodatime.org/2014/07/micro-optimization-surprising.html)
#else
        private readonly int _value;
#endif
		#endregion Fields

		#region Static Methods

        /// <summary>
        /// Adds a specified number of halfHours to a <see cref="HalfHour"/> instance.
        /// </summary>
        /// <param name="halfHour">The halfHour to add to.</param>
        /// <param name="numPeriods">The number of halfHours added.</param>
        /// <returns>The halfHour <paramref name="numPeriods"/> number of halfHours after <paramref name="halfHour"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="HalfHour"/> being <paramref name="numPeriods"/> halfHours after <paramref name="halfHour"/> if
        /// <paramref name="numPeriods"/> is positive, and the absolute value of <paramref name="numPeriods"/> halfHours
        /// before <paramref name="halfHour"/> if <paramref name="numPeriods"/> is negative.</remarks>
        [Pure]
        public static HalfHour Add(HalfHour halfHour, int numPeriods) => halfHour.Offset(numPeriods);

        /// <summary>
        /// Subtracts a specified number of halfHours from a <see cref="HalfHour"/> instance.
        /// </summary>
        /// <param name="halfHour">The halfHour to subtract from.</param>
        /// <param name="numPeriods">The number of halfHours subtracted.</param>
        /// <returns>The halfHour <paramref name="numPeriods"/> number of halfHours before <paramref name="halfHour"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="HalfHour"/> being <paramref name="numPeriods"/> halfHours before <paramref name="halfHour"/> if <paramref name="numPeriods"/>
        /// is positive, and the absolute value of <paramref name="numPeriods"/> halfHours after <paramref name="halfHour"/> if
        /// <paramref name="numPeriods"/> is negative.</remarks>
        [Pure]
        public static HalfHour Subtract(HalfHour halfHour, int numPeriods) => halfHour.Offset(-numPeriods);

        /// <summary>
        /// Subtracts one <see cref="HalfHour"/> instance from another, returning the number of halfHours difference.
        /// </summary>
        /// <param name="left">The halfHour being subtracted from.</param>
        /// <param name="right">The halfHour being subtracted.</param>
        /// <returns>The number of halfHours that <paramref name="left"/> is after <paramref name="right"/>.</returns>
        /// <remarks>This will be a positive number if <paramref name="left"/> is later than <paramref name="right"/>,
        /// and negative if <paramref name="left"/> is earlier than <paramref name="right"/>.</remarks>
        [Pure]
        public static int Subtract(HalfHour left, HalfHour right) => left.OffsetFrom(right);

        #endregion Static Methods

		#region Instance Methods
		
		/// <summary>
        /// Returns the first instance of time period type <typeparamref name="T"/> which sits within the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the time period being returns.</typeparam>
        /// <returns>The first instance of <typeparamref name="T"/> which sit within the <see cref="HalfHour"/>
        /// represented by the current instance.</returns>
		public T First<T>() where T : ITimePeriod<T>
        {
            return TimePeriodHelper.First<HalfHour, T>(this);
        }

		/// <summary>
        /// Returns the last instance of time period type <typeparamref name="T"/> which sits within the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the time period being returns.</typeparam>
        /// <returns>The last instance of <typeparamref name="T"/> which sit within the <see cref="HalfHour"/>
        /// represented by the current instance.</returns>
        public T Last<T>() where T : ITimePeriod<T>
        {
            return TimePeriodHelper.Last<HalfHour, T>(this);
        }

		/// <summary>
        /// Expands the current instance into a collection of instances of a smaller time period type, which fit
        /// within the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the time period being expanded to.</typeparam>
        /// <returns>A collection of all instances of <typeparamref name="T"/> which fit within the <see cref="HalfHour"/>
        /// represented by the current instance.</returns>
        public IEnumerable<T> Expand<T>() where T : ITimePeriod<T>
        {
            return TimePeriodHelper.Expand<HalfHour, T>(this);
        }

        /// <summary>
        /// Compares this instance with another <see cref="HalfHour"/> and indicates whether this instance is
        /// earlier than, equal to, or later than the other instance.
        /// </summary>
        /// <param name="other">The halfHour being compared to.</param>
        /// <returns>A positive number if this instance is later than <paramref name="other"/>, zero if this instance is
        /// equal to <paramref name="other"/>, and a negative number if this instance is earlier than <paramref name="other"/>.
        /// </returns>
        [Pure]
        public int CompareTo(HalfHour other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return _value.CompareTo(other._value);
        }

        /// <summary>
        /// Compares this instance with another boxed <see cref="HalfHour"/> and indicates whether this instance is
        /// earlier than, equal to, or later than the other instance.
        /// </summary>
        /// <remarks>Explicit interface implementation of the CompareTo method of the non-generic IComparable interface.
        /// The calling of this method should generally be avoided in favour of the CompareTo method with parameter of
        /// type <see cref="HalfHour"/> as this is both type safe, and does not involve boxing of the instance being compared to.</remarks>
        /// <param name="obj">The object being compared to.</param>
        /// <returns>Integer returned (if <paramref name="obj"/> is non-null and of type <see cref="HalfHour"/>)
        /// is a positive number if this instance is later than <paramref name="obj"/>, zero if this instance is
        /// equal to <paramref name="obj"/>, and a negative number if this instance is earlier than <paramref name="obj"/>.
        /// If <paramref name="obj"/> is null then this instance is considered later than <paramref name="obj"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null and not of type <see cref="HalfHour"/>.
        /// </exception>
        [Pure]
        int IComparable.CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (!(obj is HalfHour halfHour))
                throw new ArgumentException("Object must be of type AnotherTimePeriod.HalfHour.", nameof(obj));
            return CompareTo(halfHour);
        }

        /// <summary>
        /// Evaluates equality with another <see cref="HalfHour"/> instance.
        /// </summary>
        /// <param name="other">The halfHour being compared to.</param>
        /// <returns>True if <paramref name="other"/> represents the same halfHour as the current instance,
        /// false otherwise.</returns>
        [Pure]
        public bool Equals(HalfHour other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return _value.Equals(other._value);
        }

        /// <summary>
        /// Calculates the <see cref="HalfHour"/> a specified number of halfHours after or before the current instance.
        /// </summary>
        /// <param name="numPeriods">The number of halfHours to offset the current instance by.</param>
        /// <returns>The <see cref="HalfHour"/> equal to this instance offset by <paramref name="numPeriods"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="HalfHour"/> being <paramref name="numPeriods"/> halfHours after the current instance if <paramref name="numPeriods"/>
        /// is positive, and the absolute value of <paramref name="numPeriods"/> halfHours before the current instance if
        /// <paramref name="numPeriods"/> is negative.</remarks>
        [Pure]
        public HalfHour Offset(int numPeriods)
        {
            return new HalfHour(_value + numPeriods);
        }

        /// <summary>
        /// Calculates the number of halfHours difference between this instance and another <see cref="HalfHour"/>.
        /// </summary>
        /// <param name="other">The halfHour the difference is calculated from.</param>
        /// <returns>The number of halfHours difference between this instance and <paramref name="other"/>.</returns>
        /// <remarks>This will be a positive number if <paramref name="other"/> is earlier than, and negative if
        /// <paramref name="other"/> is after, the current instance.</remarks>
        [Pure]
        public int OffsetFrom(HalfHour other)
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
            string valueText = reader.ReadElementContentAsString();
#if NET40
            this = Parse(valueText);
#else
            Unsafe.AsRef(this) = Parse(valueText);
#endif
        }

        /// <inheritdoc/>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckParameterNotNull(writer, nameof(writer));
            writer.WriteString(ToString());
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
        /// <returns>True if <paramref name="obj"/> is of type <see cref="HalfHour"/> and represents the same
        /// halfHour as the current instance, false otherwise.</returns>
        [Pure]
        public override bool Equals(object obj)
        {
            if (obj is HalfHour halfHour)
                return Equals(halfHour);
            return false;
        }
		
        #endregion Instance Methods

		#region Operators
        /// <summary>
        /// Evaluates the equality of two <see cref="HalfHour"/> instances.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if the two <see cref="HalfHour"/> instances represent the same halfHour,
        /// false otherwise.</returns>
        public static bool operator ==(HalfHour left, HalfHour right) => left.Equals(right);

        /// <summary>
        /// Evaluates the inequality of two <see cref="HalfHour"/> instances.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if the two <see cref="HalfHour"/> instances represent different halfHours,
        /// false otherwise.</returns>
        public static bool operator !=(HalfHour left, HalfHour right) => !left.Equals(right);

        /// <summary>
        /// Evaluates whether one <see cref="HalfHour"/> instance is later than another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is later than <paramref name="right"/>, false otherwise.</returns>
        public static bool operator >(HalfHour left, HalfHour right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Evaluates whether one <see cref="HalfHour"/> instance is earlier than another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is earlier than <paramref name="right"/>, false otherwise.</returns>
        public static bool operator <(HalfHour left, HalfHour right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Evaluates whether one <see cref="HalfHour"/> instance is later than or equal to than another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is later than or equal to <paramref name="right"/>, false otherwise.</returns>
        public static bool operator >=(HalfHour left, HalfHour right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Evaluates whether one <see cref="HalfHour"/> instance is earlier than or equal to another.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>True if <paramref name="left"/> is earlier than or equal to <paramref name="right"/>, false otherwise.</returns>
        public static bool operator <=(HalfHour left, HalfHour right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Adds a specified number of halfHours to a <see cref="HalfHour"/> instance.
        /// </summary>
        /// <param name="halfHour">The halfHour to add to.</param>
        /// <param name="numPeriods">The number of halfHours added.</param>
        /// <returns>The halfHour <paramref name="numPeriods"/> number of halfHours after <paramref name="halfHour"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="HalfHour"/> being <paramref name="numPeriods"/> halfHours after <paramref name="halfHour"/> if
        /// <paramref name="numPeriods"/> is positive, and the absolute value of <paramref name="numPeriods"/> halfHours
        /// before <paramref name="halfHour"/> if <paramref name="numPeriods"/> is negative.</remarks>
        public static HalfHour operator +(HalfHour halfHour, int numPeriods) => Add(halfHour, numPeriods);

        /// <summary>
        /// Subtracts a specified number of halfHours from a <see cref="HalfHour"/> instance.
        /// </summary>
        /// <param name="halfHour">The halfHour to subtract from.</param>
        /// <param name="numPeriods">The number of halfHours subtracted.</param>
        /// <returns>The halfHour <paramref name="numPeriods"/> number of halfHours before <paramref name="halfHour"/>.</returns>
        /// <remarks>Note that <paramref name="numPeriods"/> can be positive or negative, with the returned
        /// <see cref="HalfHour"/> being <paramref name="numPeriods"/> halfHours before <paramref name="halfHour"/> if <paramref name="numPeriods"/>
        /// is positive, and the absolute value of <paramref name="numPeriods"/> halfHours after <paramref name="halfHour"/> if
        /// <paramref name="numPeriods"/> is negative.</remarks>
        public static HalfHour operator -(HalfHour halfHour, int numPeriods) => Subtract(halfHour, numPeriods);

        /// <summary>
        /// Subtracts one <see cref="HalfHour"/> instance from another, returning the number of halfHours difference.
        /// </summary>
        /// <param name="left">Left-hand side of the operator.</param>
        /// <param name="right">Right-hand side of the operator.</param>
        /// <returns>The number of halfHours that <paramref name="left"/> is after <paramref name="right"/>.</returns>
        /// <remarks>This will be a positive number if <paramref name="left"/> is later than <paramref name="right"/>,
        /// and negative if <paramref name="left"/> is earlier than <paramref name="right"/>.</remarks>
        public static int operator -(HalfHour left, HalfHour right) => Subtract(left, right);

        /// <summary>
        /// Increments a <see cref="HalfHour"/> instance by one halfHour.
        /// </summary>
        /// <param name="halfHour">The halfHour being incremented.</param>
        /// <returns>The halfHour immediately following <paramref name="halfHour"/>.</returns>
        public static HalfHour operator ++(HalfHour halfHour) => halfHour.Offset(1);

        /// <summary>
        /// Decrements a <see cref="HalfHour"/> instance by one halfHour.
        /// </summary>
        /// <param name="halfHour">The halfHour being decremented.</param>
        /// <returns>The halfHour immediately preceding <paramref name="halfHour"/>.</returns>
        public static HalfHour operator --(HalfHour halfHour) => halfHour.Offset(-1);

        #endregion
		
	}
}
