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
using System.ComponentModel;
using System.Linq;
using Cmdty.TimePeriodValueTypes;
using JetBrains.Annotations;
using MathNet.Numerics.LinearAlgebra;

namespace Cmdty.Curves
{
    public sealed class Bootstrapper<T> : IBootstrapper<T>, IBootstrapperAddOptionalParameters<T>
        where T : ITimePeriod<T>
    {
        private readonly List<Contract<T>> _contracts;
        private readonly List<Shaping<T>> _shapings;
        private Func<T, double> _weighting;
        private bool _allowRedundancy;

        public Bootstrapper()
        {
            _contracts = new List<Contract<T>>();
            _shapings = new List<Shaping<T>>();
        }

        public IBootstrapperAddOptionalParameters<T> AddContract([NotNull] Contract<T> contract)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));
            _contracts.Add(contract);
            return this;
        }

        IBootstrapperAddOptionalParameters<T> IBootstrapperAddOptionalParameters<T>.AddContract(Contract<T> contract)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));
            _contracts.Add(contract);
            return this;
        }

        IBootstrapperAddOptionalParameters<T> IBootstrapperAddOptionalParameters<T>.AddShaping(Shaping<T> shaping)
        {
            if (shaping == null) throw new ArgumentNullException(nameof(shaping));
            _shapings.Add(shaping);
            return this;
        }

        IBootstrapperAddOptionalParameters<T> IBootstrapperAddOptionalParameters<T>.AddShapings([NotNull] IEnumerable<Shaping<T>> shapings)
        {
            if (shapings == null) throw new ArgumentNullException(nameof(shapings));
            _shapings.AddRange(shapings);
            return this;
        }

        IBootstrapperAddOptionalParameters<T> IBootstrapperAddOptionalParameters<T>.WithAverageWeighting([NotNull] Func<T, double> weighting)
        {
            _weighting = weighting ?? throw new ArgumentNullException(nameof(weighting));
            return this;
        }

        IBootstrapperAddOptionalParameters<T> IBootstrapperAddOptionalParameters<T>.AllowRedundancy()
        {
            _allowRedundancy = true;
            return this;
        }

        BootstrapResults<T> IBootstrapperAddOptionalParameters<T>.Bootstrap()
        {
            return Calculate(_contracts, 
                _weighting ?? (timePeriod => (timePeriod.End - timePeriod.Start).TotalMinutes), _shapings, _allowRedundancy);
        }

        // TODO include discount factors
        private static BootstrapResults<T> Calculate([NotNull] List<Contract<T>> contracts, [NotNull] Func<T, double> weighting,
            [NotNull] List<Shaping<T>> shapings, bool allowRedundancy = false)
        {
            if (contracts == null) throw new ArgumentNullException(nameof(contracts));
            if (weighting == null) throw new ArgumentNullException(nameof(weighting));
            if (shapings == null) throw new ArgumentNullException(nameof(shapings));

            var contractsPlusShapingsCount = contracts.Count + shapings.Count;

            if (contractsPlusShapingsCount < 2)
                throw new ArgumentException("contracts and shapings combined must contain at least two elements", nameof(contracts));

            // TODO check if two contracts have the same Start and End?
            var minTimePeriod = contracts.Select(contract => contract.Start)
                .Concat(shapings.Select(shaping => shaping.Start1))
                .Concat(shapings.Select(shaping => shaping.Start2))
                .Min(timePeriod => timePeriod);

            var maxTimePeriod = contracts.Select(contract => contract.End)
                .Concat(shapings.Select(shaping => shaping.End1))
                .Concat(shapings.Select(shaping => shaping.End2))
                .Max(timePeriod => timePeriod);

            var numTimePeriods = maxTimePeriod.OffsetFrom(minTimePeriod) + 1;

            var matrix = Matrix<double>.Build.Dense(contractsPlusShapingsCount, numTimePeriods);
            var vector = Vector<double>.Build.Dense(contractsPlusShapingsCount);

            for (int i = 0; i < contracts.Count; i++)
            {
                var contract = contracts[i];
                double sumWeight = 0.0;
                var startOffset = contract.Start.OffsetFrom(minTimePeriod);
                var endOffset = contract.End.OffsetFrom(minTimePeriod);
                for (int j = startOffset; j <= endOffset; j++)
                {
                    var timePeriod = minTimePeriod.Offset(j);
                    var weight = weighting(timePeriod);
                    matrix[i, j] = weight;
                    sumWeight += weight;
                }
                if (sumWeight <= 0)
                {
                    throw new ArgumentException(
                        "sum of weighting evaluated to non-positive number for the following contract: " + contract);
                }
                vector[i] = contract.Price * sumWeight;
            }

            for (int i = 0; i < shapings.Count; i++)
            {
                var shaping = shapings[i];
                int rowIndex = i + contracts.Count;
                var startOffset1 = shaping.Start1.OffsetFrom(minTimePeriod);
                var endOffset1 = shaping.End1.OffsetFrom(minTimePeriod);

                var startOffset2 = shaping.Start2.OffsetFrom(minTimePeriod);
                var endOffset2 = shaping.End2.OffsetFrom(minTimePeriod);

                double sumWeighting1 = 0.0;
                for (int j = startOffset1; j <= endOffset1; j++)
                {
                    var timePeriod = minTimePeriod.Offset(j);
                    var weight = weighting(timePeriod);
                    matrix[rowIndex, j] = weight;
                    sumWeighting1 += weight;
                }

                for (int j = startOffset1; j <= endOffset1; j++)
                {
                    matrix[rowIndex, j] /= sumWeighting1;
                }

                double sumWeighting2 = shaping.Start2.EnumerateTo(shaping.End2).Sum(weighting);
                // TODO refactor to eliminate duplicate evaluation of weighting

                if (shaping.ShapingType == ShapingType.Spread)
                {
                    // TODO refactor out common code to shared methods
                    for (int j = startOffset2; j <= endOffset2; j++)
                    {
                        var timePeriod = minTimePeriod.Offset(j);
                        var weight = weighting(timePeriod);
                        matrix[rowIndex, j] -= weight / sumWeighting2;
                    }
                    vector[rowIndex] = shaping.Value;
                }
                else if (shaping.ShapingType == ShapingType.Ratio)
                {
                    // TODO refactor out common code to shared methods
                    for (int j = startOffset2; j <= endOffset2; j++)
                    {
                        var timePeriod = minTimePeriod.Offset(j);
                        var weight = weighting(timePeriod);
                        matrix[rowIndex, j] += -weight * shaping.Value / sumWeighting2;
                    }
                    vector[rowIndex] = 0.0; // Not necessary, but just being explicit
                }
                else
                {
                    throw new InvalidEnumArgumentException($"shapings[{i}].ShapingType", (int)shaping.ShapingType,
                                            typeof(ShapingType)); // TODO check the exception message and whether InvalidEnumArgumentException should be used in this context
                }
            }

            if (!allowRedundancy)
            {
                var matrixRank = matrix.Rank();
                if (matrixRank < matrix.RowCount)
                {
                    throw new ArgumentException("Redundant contracts and shapings are present");
                }
            }

            // Calculate least squares solution
            // TODO use alternative decomposition if full-rank http://www.imagingshop.com/linear-and-nonlinear-least-squares-with-math-net/
            // TODO does Math.Net offer a double tolerance parameter like NMath
            Vector<double> leastSquaresSolution = matrix.Svd(true).Solve(vector);

            var curvePeriods = new List<T>(leastSquaresSolution.Count);
            var curvePrices = new List<double>(leastSquaresSolution.Count);

            // TODO pass raw results into BootstrapResults and have the processed result lazy calculated on access

            var bootstrappedContracts = new List<Contract<T>>();

            // TODO check if only one element?
            var allOutputPeriods = minTimePeriod.EnumerateTo(maxTimePeriod.Offset(1)).Skip(1).ToList();
            T contractStart = minTimePeriod;

            for (var i = 0; i < allOutputPeriods.Count; i++)
            {
                var outputPeriod = allOutputPeriods[i];
                var inputStartsHere = contracts.Any(contract => contract.Start.Equals(outputPeriod)) ||
                        shapings.Any(shaping => shaping.Start1.Equals(outputPeriod) || shaping.Start2.Equals(outputPeriod));

                var inputEndsOneStepBefore = contracts.Any(contract => contract.End.Next().Equals(outputPeriod)) ||
                        shapings.Any(shaping => shaping.End1.Next().Equals(outputPeriod) || shaping.End2.Next().Equals(outputPeriod));

                // TODO refactor OR
                if (inputStartsHere || inputEndsOneStepBefore) // New output period
                {
                    var contractEnd = outputPeriod.Previous();

                    // Calculate weighted average price
                    // Add to bootstrappedContracts
                    double price = WeightedAveragePrice(contractStart, contractEnd, minTimePeriod, leastSquaresSolution, weighting);
                    bootstrappedContracts.Add(new Contract<T>(contractStart, contractEnd, price));

                    foreach (var period in contractStart.EnumerateTo(contractEnd))
                    {
                        curvePeriods.Add(period);
                        curvePrices.Add(price);
                    }

                    if (i < allOutputPeriods.Count - 1)
                    {
                        // Set contractStart unless last item of loop
                        if (!IsGapInContractsAndShapings(contracts, shapings, outputPeriod))
                        {
                            contractStart = outputPeriod;
                        }
                        else // outputPeriod is in a gap so need to increment i until not a gap
                        {
                            // TODO find more efficient way of doing this calculation
                            do
                            {
                                curvePeriods.Add(outputPeriod);
                                curvePrices.Add(price);
                                i++;
                                outputPeriod = outputPeriod.Next();
                            } while (IsGapInContractsAndShapings(contracts, shapings, outputPeriod));

                            contractStart = outputPeriod; // TODO remove top block of if as redundant?
                        }
                    }
                }
            }

            var curve = new DoubleCurve<T>(curvePeriods, curvePrices, weighting);
            return new BootstrapResults<T>(curve, bootstrappedContracts);
        }

        private static double WeightedAveragePrice(T start, T end, T minOutputPeriod, Vector<double> outputCurve, Func<T, double> weighting)
        {
            double sumWeightedPrice = 0.0;
            double sumWeight = 0.0;
            foreach (var period in start.EnumerateTo(end))
            {
                var index = period.OffsetFrom(minOutputPeriod);
                double price = outputCurve[index];
                var weight = weighting(period);
                sumWeightedPrice += price * weight;
                sumWeight += weight;
            }
            return sumWeightedPrice / sumWeight;
        }

        private static bool IsGapInContractsAndShapings(List<Contract<T>> contracts, List<Shaping<T>> shapings, T timePeriod)
        {
            return !(contracts.Any(contract =>
                    contract.Start.CompareTo(timePeriod) <= 0 && contract.End.CompareTo(timePeriod) >= 0) ||
                     shapings.Any(shaping =>
                         (shaping.Start1.CompareTo(timePeriod) <= 0 && shaping.End1.CompareTo(timePeriod) >= 0) ||
                         (shaping.Start2.CompareTo(timePeriod) <= 0 && shaping.End2.CompareTo(timePeriod) >= 0)));
        }

    }
}
