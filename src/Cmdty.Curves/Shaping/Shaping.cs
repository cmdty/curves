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

using Cmdty.TimePeriodValueTypes;

namespace Cmdty.Curves
{
    public sealed class Shaping<T> : IBetween<T>, IAnd<T>, IIs<T>
        where T : ITimePeriod<T>
    {
        public ShapingType ShapingType { get; }
        public T Start1 { get; private set; }
        public T End1 { get; private set; }
        public T Start2 { get; private set; }
        public T End2 { get; private set; }
        public double Value { get; private set; }

        private Shaping(ShapingType shapingType)
        {
            ShapingType = shapingType;
        }

        public static IBetween<T> Ratio => new Shaping<T>(ShapingType.Ratio);

        public static IBetween<T> Spread => new Shaping<T>(ShapingType.Spread);

        IAnd<T> IBetween<T>.Between(T start, T end)
        {
            Start1 = start;
            End1 = end;
            return this;
        }

        IAnd<T> IBetween<T>.Between(T startAndEnd)
        {
            Start1 = startAndEnd;
            End1 = startAndEnd;
            return this;
        }

        IAnd<T> IBetween<T>.Between<T2>(T2 timePeriod)
        {
            Start1 = timePeriod.First<T>();
            End1 = timePeriod.Last<T>();
            return this;
        }

        IIs<T> IAnd<T>.And(T start, T end)
        {
            Start2 = start;
            End2 = end;
            return this;
        }

        IIs<T> IAnd<T>.And(T startAndEnd)
        {
            Start2 = startAndEnd;
            End2 = startAndEnd;
            return this;
        }

        IIs<T> IAnd<T>.And<T2>(T2 timePeriod) 
        {
            Start2 = timePeriod.First<T>();
            End2 = timePeriod.Last<T>();
            return this;
        }

        Shaping<T> IIs<T>.Is(double value)
        {
            Value = value;
            return this;
        }

        public override string ToString()
        {
            string shapingTypeSymbol = this.ShapingType == ShapingType.Spread ? "-" : "/";
            return $"({Start1}:{End1}){shapingTypeSymbol}({Start2}:{End2})={Value}";
        }

    }

    public interface IBetween<T> where T : ITimePeriod<T>
    {
        IAnd<T> Between(T start, T end);
        IAnd<T> Between(T startAndEnd);
        IAnd<T> Between<T2>(T2 timePeriod) where T2 : ITimePeriod<T2>;
    }
    public interface IAnd<T> where T : ITimePeriod<T>
    {
        IIs<T> And(T start, T end);
        IIs<T> And(T startAndEnd);
        IIs<T> And<T2>(T2 timePeriod) where T2 : ITimePeriod<T2>;
    }
    public interface IIs<T> where T : ITimePeriod<T>
    {
        Shaping<T> Is(double value);
    }
}
