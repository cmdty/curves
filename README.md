Commodity Curves
================
[![Build Status](https://dev.azure.com/cmdty/github/_apis/build/status/cmdty.curves?branchName=master)](https://dev.azure.com/cmdty/github/_build/latest?definitionId=1&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/cmdty.curves.svg)](https://www.nuget.org/packages/Cmdty.Curves/)
[![PyPI](https://img.shields.io/pypi/v/curves.svg)](https://pypi.org/project/curves/)

Set of tools written in C# for constructing commodity forward/futures/swap curves with a fluent API. Python API also provided which integrates with the pandas library time series types.

## Overview

The curves package contains a set of tools for building commodity forward, swaps, and futures curves.

More specifically, the problem being solved is to take a collection of traded forward prices, and tranform these into a 
forward curve of homogenous granularity. Additionally the derived curve can constructed to be in a granularity higher 
than what is traded in the market.

Examples of types of curve which can be constructed using this package:
* Monthly granularity oil products swap curves from traded monthly, quarterly, and calendar yearly granularity
market swap rates.
* Daily granularity natural gas forward curves from traded daily, weekly, monthly, quarterly, seasonal, and
gas year granularity forward and futures prices.
* Half-hourly granularity power forward curves from traded daily, weekly, monthly, quarterly, and seasonal
granularity forward and futures prices.

The resulting curves should be consistent with inputs, in that they average back to the input forward contract prices.
This is a necessary to ensure that there are no arbitrage opportunities introduced between input contracts, and the derived forward curve.

## Getting Started

### Installing
For use from a C# install the NuGet package Cmdty.Curves.
```
PM> Install-Package Cmdty.Curves
```

For use from Python install the curves package from PyPI.
```
> pip install curves
```

### Using From C#
For examples of usage see samples/c#/.

### Using From Python
See the Jupyter Notebook [curves_quick_start_tutorial](samples/python/curves_quick_start_tutorial.ipynb) for an introduction on how to use the Python API.

## Technical Documentation
The PDF file [max_smoothness_spline.pdf](docs/max_smoothness/max_smoothness_spline.pdf) contains details of the mathematics behind the maximum smoothness algorithm.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details