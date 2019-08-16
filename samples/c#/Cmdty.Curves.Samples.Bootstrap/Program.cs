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
using Cmdty.TimeSeries;

namespace Cmdty.Curves.Samples.Bootstrap
{
    class Program
    {
        static void Main()
        {
            //===============================================================================================
            // BASIC BOOTSTRAPPING CALCULATION

            var jan20 = Month.CreateJanuary(2020);

            // Bootstrapping 1 quarterly price and 1 monthly price into a monthly curve
            var (pieceWiseCurve, bootstrappedContracts) = new Bootstrapper<Month>()
                                .AddContract(jan20, 19.05)
                                .AddContract(Quarter.CreateQuarter1(2020), 17.22)
                                .Bootstrap();

            Console.WriteLine("Derived piecewise flat curve:");
            Console.WriteLine(pieceWiseCurve.FormatData("F5"));

            Console.WriteLine();

            Console.WriteLine("Equivalent bootstrapped contracts:");
            PrintBootstrapContracts(bootstrappedContracts);

            Console.WriteLine();
            Console.WriteLine();
            
            //===============================================================================================
            // APPLYING SHAPING DURING BOOTSTRAPPING

            var feb20 = Month.CreateFebruary(2020);
            var mar20 = Month.CreateMarch(2020);

            // Shaping applied as a ratio between Feb and Mar
            const double ratio = 1.1;
            var (pieceWiseCurveWithRatio, _) = new Bootstrapper<Month>()
                .AddContract(jan20, 19.05)
                .AddContract(Quarter.CreateQuarter1(2020), 17.22)
                .AddShaping(Shaping<Month>.Ratio.Between(feb20).And(mar20).Is(ratio))
                .Bootstrap();

            Console.WriteLine($"Derived piecewise flat curve with {ratio} ratio applied between Feb and Mar:");
            Console.WriteLine(pieceWiseCurveWithRatio.FormatData("F5"));

            Console.WriteLine();

            Console.WriteLine("Ratio in derived curve: {0:F5}", 
                pieceWiseCurveWithRatio[feb20]/pieceWiseCurveWithRatio[mar20]);

            Console.WriteLine();
            Console.WriteLine();

            // Shaping applied as a spread between Feb and Mar
            const double spread = 0.21;
            var (pieceWiseCurveWithSpread, _) = new Bootstrapper<Month>()
                .AddContract(jan20, 19.05)
                .AddContract(Quarter.CreateQuarter1(2020), 17.22)
                .AddShaping(Shaping<Month>.Spread.Between(feb20).And(mar20).Is(spread))
                .Bootstrap();
            
            Console.WriteLine($"Derived piecewise flat curve with {spread} spread applied between Feb and Mar:");
            Console.WriteLine(pieceWiseCurveWithSpread.FormatData("F5"));

            Console.WriteLine();

            Console.WriteLine("Spread in derived curve: {0:F5}",
                            pieceWiseCurveWithSpread[feb20] - pieceWiseCurveWithSpread[mar20]);

            Console.WriteLine();
            Console.WriteLine();

            //===============================================================================================
            // HANDLING REDUNDANCY OF CONTRACTS AND SHAPING FACTORS

            try
            {
                // If the price of Feb-20 is added, this essentially represents redundancy in the inputs, resulting in an exception
                new Bootstrapper<Month>()
                    .AddContract(jan20, 22.95)
                    .AddContract(feb20, 21.05)
                    .AddContract(Quarter.CreateQuarter1(2020), 19.05)
                    .AddShaping(Shaping<Month>.Spread.Between(feb20).And(mar20).Is(spread))
                    .Bootstrap();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Exception raised when redundancy inputs provided:");
                Console.WriteLine(e);
            }

            Console.WriteLine();
            Console.WriteLine();

            // Using AllowRedundancy() allows the calculation to perform without checks on redundancy
            var (pieceWiseCurveWithRedundancy, _) = new Bootstrapper<Month>()
                            .AddContract(jan20, 22.95)
                            .AddContract(feb20, 21.05)
                            .AddContract(Quarter.CreateQuarter1(2020), 19.05)
                            .AddShaping(Shaping<Month>.Spread.Between(feb20).And(mar20).Is(spread))
                            .AllowRedundancy()
                            .Bootstrap();

            Console.WriteLine("Derived piecewise flat curve with redundant inputs, after AllowRedundancy() called:");
            Console.WriteLine(pieceWiseCurveWithRedundancy.FormatData("F5"));

            Console.WriteLine();
            Console.WriteLine();

            //===============================================================================================
            // APPLYING AN ALTERNATIVE WEIGHTING SCHEME

            // By default, the bootstrap calculations assume averages are weighted by the number of minutes in a contract period.
            // This assumption is fine for instruments where the commodity is delivered at a constant rate, e.g. natural gas forwards.
            // An alternative weighting scheme can be added by calling WithAverageWeighting and supplying a weighting scheme as a function.
            // The below example makes use of the Weighting helper class to provide the weighting function as the count of business days.
            // An example of when such a weighting scheme should be used is for oil swaps, based on an index which is only published on a business day.

            var holidays = new List<Day>(){new Day(2020, 1, 1)};
            Func<Month, double> busDayWeight = Weighting.BusinessDayCount<Month>(holidays);
            var (pieceWiseCurveBusDayWeight, _) = new Bootstrapper<Month>()
                            .AddContract(jan20, 19.05)
                            .AddContract(Quarter.CreateQuarter1(2020), 17.22)
                            .WithAverageWeighting(busDayWeight)
                            .Bootstrap();

            Console.WriteLine("Derived piecewise flat curve with business day weighting:");
            Console.WriteLine(pieceWiseCurveBusDayWeight.FormatData("F5"));

            Console.ReadKey();
        }

        private static void PrintBootstrapContracts(IReadOnlyList<Contract<Month>> bootstrappedContracts)
        {
            foreach (var contract in bootstrappedContracts)
            {
                Console.WriteLine(contract);
            }
        }
    }
}
