﻿#region License
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
using Cmdty.TimePeriodValueTypes;

namespace Cmdty.Curves
{
    public interface ISplineAddOptionalParameters<T>
        where T : ITimePeriod<T>
    {
        ISplineAddOptionalParameters<T> AddContract(Contract<T> contract);
        ISplineAddOptionalParameters<T> WithWeighting(Func<T, double> weighting);
        ISplineAddOptionalParameters<T> WithMultiplySeasonalAdjustment(Func<T, double> adjustment);
        ISplineAddOptionalParameters<T> WithAdditiveSeasonalAdjustment(Func<T, double> adjustment);
        ISplineAddOptionalParameters<T> WithFrontFirstDerivative(double firstDerivative);
        ISplineAddOptionalParameters<T> WithBackFirstDerivative(double firstDerivative);
        ISplineAddOptionalParameters<T> WithTimeFunc(Func<T, T, double> timeFunc);
        ISplineAddOptionalParameters<T> WithTensionParameter(double tension);
        MaxSmoothnessSplineResults<T> BuildCurve();
    }
}