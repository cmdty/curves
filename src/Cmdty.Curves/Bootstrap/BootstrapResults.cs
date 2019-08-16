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
using Cmdty.TimePeriodValueTypes;
using JetBrains.Annotations;

namespace Cmdty.Curves
{
    public sealed class BootstrapResults<T> where T : ITimePeriod<T>
    {
        public DoubleCurve<T> Curve { get; }
        public IReadOnlyList<Contract<T>> BootstrappedContracts { get; }

        public BootstrapResults([NotNull] DoubleCurve<T> curve, [NotNull] List<Contract<T>> bootstrappedContracts)
        {
            Curve = curve ?? throw new ArgumentNullException(nameof(curve));
            BootstrappedContracts = bootstrappedContracts ?? throw new ArgumentNullException(nameof(bootstrappedContracts));
        }

        public void Deconstruct(out DoubleCurve<T> curve, out IReadOnlyList<Contract<T>> bootstrappedContracts)
        {
            curve = Curve;
            bootstrappedContracts = BootstrappedContracts;
        }

    }
}