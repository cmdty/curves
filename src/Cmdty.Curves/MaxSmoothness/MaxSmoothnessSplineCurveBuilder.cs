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
using System.Linq;
using Cmdty.TimePeriodValueTypes;
using JetBrains.Annotations;
using MathNet.Numerics.LinearAlgebra;

namespace Cmdty.Curves
{
    public sealed class MaxSmoothnessSplineCurveBuilder<T> : IMaxSmoothSplineCurveBuilder<T>, ISplineAddOptionalParameters<T>
        where T : ITimePeriod<T>
    {
        private readonly List<Contract<T>> _contracts;
        private Func<T, double> _weighting;
        private Func<T, double> _multAdjust;
        private Func<T, double> _addAdjust;
        private double? _frontFirstDerivative;
        private double? _backFirstDerivative;
        private Func<T, T, double> _timeFunc;
        
        public MaxSmoothnessSplineCurveBuilder()
        {
            _contracts = new List<Contract<T>>();
        }

        public ISplineAddOptionalParameters<T> AddContract([NotNull] Contract<T> contract)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));
            _contracts.Add(contract);
            return this;
        }

        ISplineAddOptionalParameters<T> ISplineAddOptionalParameters<T>.AddContract([NotNull] Contract<T> contract)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));
            _contracts.Add(contract);
            return this;
        }

        ISplineAddOptionalParameters<T> ISplineAddOptionalParameters<T>.WithWeighting([NotNull] Func<T, double> weighting)
        {
            _weighting = weighting ?? throw new ArgumentNullException(nameof(weighting));
            return this;
        }

        ISplineAddOptionalParameters<T> ISplineAddOptionalParameters<T>.WithMultiplySeasonalAdjustment(
            [NotNull] Func<T, double> multAdjust)
        {
            _multAdjust = multAdjust ?? throw new ArgumentNullException(nameof(multAdjust));
            return this;
        }

        ISplineAddOptionalParameters<T> ISplineAddOptionalParameters<T>.WithAdditiveSeasonalAdjustment(
            [NotNull] Func<T, double> addAdjust)
        {
            _addAdjust = addAdjust ?? throw new ArgumentNullException(nameof(addAdjust));
            return this;
        }

        ISplineAddOptionalParameters<T> ISplineAddOptionalParameters<T>.WithTimeFunc([NotNull] Func<T, T, double> timeFunc)
        {
            _timeFunc = timeFunc ?? throw new ArgumentNullException(nameof(timeFunc));
            return this;
        }

        ISplineAddOptionalParameters<T> ISplineAddOptionalParameters<T>.WithFrontFirstDerivative(double firstDerivative)
        {
            _frontFirstDerivative = firstDerivative;
            return this;
        }

        ISplineAddOptionalParameters<T> ISplineAddOptionalParameters<T>.WithBackFirstDerivative(double firstDerivative)
        {
            _backFirstDerivative = firstDerivative;
            return this;
        }

        public DoubleCurve<T> BuildCurve()
        {
            return Build(_contracts.OrderBy(contract => contract.Start).ToList(),
                _weighting ?? (timePeriod => (timePeriod.End - timePeriod.Start).TotalMinutes),
                _multAdjust ?? (timePeriod => 1.0),
                _addAdjust ?? (timePeriod => 0.0),
                _timeFunc ?? ((period1, period2) => period2.OffsetFrom(period1)),
                _frontFirstDerivative,
                _backFirstDerivative
            ); 
        }

        private static DoubleCurve<T> Build(List<Contract<T>> contracts, Func<T, double> weighting,
            Func<T, double> multAdjustFunc, Func<T, double> addAdjustFunc, Func<T, T, double> timeFunc,
            double? frontFirstDerivative, double? backFirstDerivative)
        {
            if (contracts.Count < 2)
                throw new ArgumentException("contracts must have at least two elements", nameof(contracts));

            var curveStartPeriod = contracts[0].Start;

            int numGaps = 0;
            var timeToPolynomialBoundaries = new List<double>();

            // TODO optionally do/don't allow gaps in contracts
            for (int i = 0; i < contracts.Count - 1; i++)
            {
                var contractEnd = contracts[i].End;
                var nextContractStart = contracts[i + 1].Start;

                if (contractEnd.CompareTo(nextContractStart) >= 0)
                {
                    throw new ArgumentException("contracts are overlapping");
                }
                timeToPolynomialBoundaries.Add(timeFunc(curveStartPeriod, nextContractStart));
                
                if (contractEnd.OffsetFrom(nextContractStart) < -1) // Gap in contracts
                {
                    numGaps++;
                    timeToPolynomialBoundaries.Add(timeFunc(curveStartPeriod, contractEnd.Next()));
                }
            }

            int numPolynomials = contracts.Count + numGaps;
            int numCoefficientsToSolve = numPolynomials * 5;

            int numConstraints =
                (numPolynomials - 1) * 3 // Spline value, 1st derivative, and 2nd derivative constraints
                + numPolynomials - numGaps // Price constraints
                + (frontFirstDerivative.HasValue ? 1 : 0)
                + (backFirstDerivative.HasValue ? 1 : 0);

            MatrixBuilder<double> matrixBuilder = Matrix<double>.Build;
            VectorBuilder<double> vectorBuilder = Vector<double>.Build;

            var constraintMatrix = matrixBuilder.Dense(numConstraints, numCoefficientsToSolve);
            var vector = vectorBuilder.Dense(numPolynomials * 5 + numConstraints);

            var twoHMatrix = matrixBuilder.Dense(numPolynomials * 5, numPolynomials * 5);

            int inputContractIndex = 0;
            
            bool gapFilled = false;

            int rowNum = 0;
            for (int i = 0; i < numPolynomials; i++)
            {
                int colOffset = i * 5;
                if (i < numPolynomials - 1)
                {
                    double timeToPolynomialBoundary = timeToPolynomialBoundaries[i];
                    double timeToPolynomialBoundaryPow2 = Math.Pow(timeToPolynomialBoundary, 2);
                    double timeToPolynomialBoundaryPow3 = Math.Pow(timeToPolynomialBoundary, 3);
                    double timeToPolynomialBoundaryPow4 = Math.Pow(timeToPolynomialBoundary, 4);

                    // Polynomial equality at boundaries
                    constraintMatrix[rowNum, colOffset] = 1.0;
                    constraintMatrix[rowNum, colOffset + 1] = timeToPolynomialBoundary;
                    constraintMatrix[rowNum, colOffset + 2] = timeToPolynomialBoundaryPow2;
                    constraintMatrix[rowNum, colOffset + 3] = timeToPolynomialBoundaryPow3;
                    constraintMatrix[rowNum, colOffset + 4] = timeToPolynomialBoundaryPow4;

                    constraintMatrix[rowNum, colOffset + 5] = -1.0;
                    constraintMatrix[rowNum, colOffset + 6] = -timeToPolynomialBoundary;
                    constraintMatrix[rowNum, colOffset + 7] = -timeToPolynomialBoundaryPow2;
                    constraintMatrix[rowNum, colOffset + 8] = -timeToPolynomialBoundaryPow3;
                    constraintMatrix[rowNum, colOffset + 9] = -timeToPolynomialBoundaryPow4;

                    // Polynomial first derivative equality at boundaries
                    constraintMatrix[rowNum + 1, colOffset] = 0.0;
                    constraintMatrix[rowNum + 1, colOffset + 1] = 1.0;
                    constraintMatrix[rowNum + 1, colOffset + 2] = 2.0 * timeToPolynomialBoundary;
                    constraintMatrix[rowNum + 1, colOffset + 3] = 3.0 * timeToPolynomialBoundaryPow2;
                    constraintMatrix[rowNum + 1, colOffset + 4] = 4.0 * timeToPolynomialBoundaryPow3;

                    constraintMatrix[rowNum + 1, colOffset + 5] = 0.0;
                    constraintMatrix[rowNum + 1, colOffset + 6] = -1.0;
                    constraintMatrix[rowNum + 1, colOffset + 7] = -2.0 * timeToPolynomialBoundary;
                    constraintMatrix[rowNum + 1, colOffset + 8] = -3.0 * timeToPolynomialBoundaryPow2;
                    constraintMatrix[rowNum + 1, colOffset + 9] = -4.0 * timeToPolynomialBoundaryPow3;

                    // Polynomial second derivative equality at boundaries
                    constraintMatrix[rowNum + 2, colOffset] = 0.0;
                    constraintMatrix[rowNum + 2, colOffset + 1] = 0.0;
                    constraintMatrix[rowNum + 2, colOffset + 2] = 2.0;
                    constraintMatrix[rowNum + 2, colOffset + 3] = 6.0 * timeToPolynomialBoundary;
                    constraintMatrix[rowNum + 2, colOffset + 4] = 12.0 * timeToPolynomialBoundaryPow2;

                    constraintMatrix[rowNum + 2, colOffset + 5] = 0.0;
                    constraintMatrix[rowNum + 2, colOffset + 6] = 0.0;
                    constraintMatrix[rowNum + 2, colOffset + 7] = -2.0;
                    constraintMatrix[rowNum + 2, colOffset + 8] = -6 * timeToPolynomialBoundary;
                    constraintMatrix[rowNum + 2, colOffset + 9] = -12.0 * timeToPolynomialBoundaryPow2;
                }

                // Contract price constraint
                if (i == 0 || // Can't be gap at the first position
                    contracts[inputContractIndex - 1].End.OffsetFrom(contracts[inputContractIndex].Start) ==
                    -1 || // No gap from previous
                    gapFilled) // Gap has already been dealt with
                {
                    Contract<T> contract = contracts[inputContractIndex];
                    double sumWeight = 0.0;
                    double sumWeightMult = 0.0;
                    double sumWeightMultTime = 0.0;
                    double sumWeightMultTimePow2 = 0.0;
                    double sumWeightMultTimePow3 = 0.0;
                    double sumWeightMultTimePow4 = 0.0;
                    double sumWeightMultAdd = 0.0;

                    foreach (T timePeriod in contract.Start.EnumerateTo(contract.End))
                    {
                        double timeToPeriod = timeFunc(curveStartPeriod, timePeriod);
                        double weight = weighting(timePeriod);
                        double multAdjust = multAdjustFunc(timePeriod);
                        double addAdjust = addAdjustFunc(timePeriod);

                        sumWeight += weight;
                        sumWeightMult += weight * multAdjust;
                        sumWeightMultTime += weight * multAdjust * timeToPeriod;
                        sumWeightMultTimePow2 += weight * multAdjust * Math.Pow(timeToPeriod, 2.0);
                        sumWeightMultTimePow3 += weight * multAdjust * Math.Pow(timeToPeriod, 3.0);
                        sumWeightMultTimePow4 += weight * multAdjust * Math.Pow(timeToPeriod, 4.0);
                        sumWeightMultAdd += weight * multAdjust * addAdjust;
                    }

                    int priceConstraintRow = i == (numPolynomials - 1) ? rowNum : rowNum + 3;

                    constraintMatrix[priceConstraintRow, colOffset] = sumWeightMult; // Coefficient of a
                    constraintMatrix[priceConstraintRow, colOffset + 1] = sumWeightMultTime; // Coefficient of b
                    constraintMatrix[priceConstraintRow, colOffset + 2] = sumWeightMultTimePow2; // Coefficient of c
                    constraintMatrix[priceConstraintRow, colOffset + 3] = sumWeightMultTimePow3; // Coefficient of d
                    constraintMatrix[priceConstraintRow, colOffset + 4] = sumWeightMultTimePow4; // Coefficient of e

                    vector[numPolynomials * 5 + priceConstraintRow] = sumWeight * contract.Price - sumWeightMultAdd;

                    twoHMatrix.SetSubMatrix(i * 5 + 2, i * 5 + 2,
                                Create2HBottomRightSubMatrix(contract, curveStartPeriod, timeFunc));

                    inputContractIndex++;
                    rowNum += 4;
                    gapFilled = false;
                }
                else
                {
                    // Gap in contracts
                    rowNum += 3;
                    gapFilled = true;
                }
            }

            // TODO unit test first derivative constraints. How?
            rowNum -= 3;
            if (frontFirstDerivative.HasValue)
            {
                constraintMatrix[rowNum, 1] = 1; // Coefficient of b
                vector[numPolynomials * 5 + rowNum] = frontFirstDerivative.Value;
                rowNum++;
            }

            if (backFirstDerivative.HasValue)
            {
                T lastPeriod = contracts[contracts.Count - 1].End;
                double timeToEnd = timeFunc(curveStartPeriod, lastPeriod.Offset(1));
                constraintMatrix[rowNum, numCoefficientsToSolve - 4] = 1; // Coefficient of b
                constraintMatrix[rowNum, numCoefficientsToSolve - 3] = 2 * timeToEnd; // Coefficient of c
                constraintMatrix[rowNum, numCoefficientsToSolve - 2] = 3 * Math.Pow(timeToEnd, 2); // Coefficient of d
                constraintMatrix[rowNum, numCoefficientsToSolve - 1] = 4 * Math.Pow(timeToEnd, 3); // Coefficient of e
                vector[numPolynomials * 5 + rowNum] = backFirstDerivative.Value;
            }
            
            // Create system of equations to solve
            Matrix<double> tempMatrix1 = twoHMatrix.Append(constraintMatrix.Transpose());
            var tempMatrix2 = constraintMatrix.Append(
                matrixBuilder.Dense(constraintMatrix.RowCount, constraintMatrix.RowCount));

            var matrix = tempMatrix1.Stack(tempMatrix2);

            Vector<double> solution = matrix.Solve(vector);

            // Read off results from polynomial
            T curveEndPeriod = contracts[contracts.Count - 1].End;
            int numOutputPeriods = curveEndPeriod.OffsetFrom(curveStartPeriod) + 1;
            var outputCurvePeriods = new T[numOutputPeriods];
            var outputCurvePrices = new double[numOutputPeriods];
            int outputContractIndex = 0;

            gapFilled = false;
            inputContractIndex = 0;

            for (int i = 0; i < numPolynomials; i++)
            {
                double EvaluateSpline(T timePeriod)
                {
                    double timeToPeriod = timeFunc(curveStartPeriod, timePeriod);

                    int solutionOffset = i * 5;
                    double splineValue = solution[solutionOffset] +
                                         solution[solutionOffset + 1] * timeToPeriod +
                                         solution[solutionOffset + 2] * Math.Pow(timeToPeriod, 2) +
                                         solution[solutionOffset + 3] * Math.Pow(timeToPeriod, 3) +
                                         solution[solutionOffset + 4] * Math.Pow(timeToPeriod, 4);

                    double multAdjust = multAdjustFunc(timePeriod);
                    double addAdjust = addAdjustFunc(timePeriod);

                    return (splineValue + addAdjust) * multAdjust;
                }

                T start;
                T end;
                if (i == 0 || // Can't be gap at the first position
                    contracts[inputContractIndex - 1].End.OffsetFrom(contracts[inputContractIndex].Start) == -1 || // No gap from previous
                    gapFilled) // Gap has already been dealt with
                {
                    Contract<T> contract = contracts[inputContractIndex];
                    start = contract.Start;
                    end = contract.End;
                    inputContractIndex++;
                    gapFilled = false;
                }
                else
                {
                    start = contracts[inputContractIndex - 1].End.Next();
                    end = contracts[inputContractIndex].Start.Previous();
                    gapFilled = true;
                }

                foreach (var timePeriod in start.EnumerateTo(end))
                {
                    outputCurvePrices[outputContractIndex] = EvaluateSpline(timePeriod);
                    outputCurvePeriods[outputContractIndex] = timePeriod;
                    outputContractIndex++;
                }
            }

            return new DoubleCurve<T>(outputCurvePeriods, outputCurvePrices, weighting);
        }

        private static Matrix<double> Create2HBottomRightSubMatrix(Contract<T> contract, T curveStartPeriod,
                                                        Func<T, T, double> timeFunc)
        {
            double timeToStart = timeFunc(curveStartPeriod, contract.Start);
            double timeToEnd = timeFunc(curveStartPeriod, contract.End.Offset(1));

            var subMatrix = Matrix<double>.Build.Dense(3, 3);

            double deltaPow2 = DeltaPow(timeToStart, timeToEnd, 2.0);
            double deltaPow3 = DeltaPow(timeToStart, timeToEnd, 3.0);
            double deltaPow4 = DeltaPow(timeToStart, timeToEnd, 4.0);

            subMatrix[0, 0] = 8.0 * (timeToEnd - timeToStart);
            subMatrix[0, 1] = 12.0 * deltaPow2;
            subMatrix[0, 2] = 16.0 * deltaPow3;

            subMatrix[1, 0] = 12.0 * deltaPow2;
            subMatrix[1, 1] = 24.0 * deltaPow3;
            subMatrix[1, 2] = 36.0 * deltaPow4;

            subMatrix[2, 0] = 16.0 * deltaPow3;
            subMatrix[2, 1] = 36.0 * deltaPow4;
            subMatrix[2, 2] = 57.6 * DeltaPow(timeToStart, timeToEnd, 5.0);
            
            return subMatrix;
        }

        private static double DeltaPow(double timeToStart, double timeToEnd, double power)
        {
            return Math.Pow(timeToEnd, power) - Math.Pow(timeToStart, power);
        }

    }
}
