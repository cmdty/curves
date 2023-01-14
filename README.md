Commodity Curves
================
[![Build Status](https://dev.azure.com/cmdty/github/_apis/build/status/cmdty.curves?branchName=master)](https://dev.azure.com/cmdty/github/_build/latest?definitionId=1&branchName=master)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/cmdty/github/1)
[![NuGet](https://img.shields.io/nuget/v/cmdty.curves.svg)](https://www.nuget.org/packages/Cmdty.Curves/)
[![PyPI](https://img.shields.io/pypi/v/curves.svg)](https://pypi.org/project/curves/)

Set of tools written in C# for constructing commodity forward/futures/swap curves with a fluent API. Python API (created using [pythonnet](https://github.com/pythonnet/pythonnet)) also provided which integrates with the pandas library time series types.

### Table of Contents
* [Overview](#overview)
* [Installing](#installing)
* [.NET Dependency](#net-dependency)
* [Getting Started](#getting-started)
    * [Using From C#](#using-from-c)
        * [Bootstrapper](#bootstrapper)
        * [Spline](#spline)
    * [Using From Python](#using-from-python)
        * [Bootstrapper](#bootstrapper-1)
        * [Spline](#spline-1)
        * [Python Version Compatibility](#python-version-compatibility)
    * [Handling Clock Changes](#handling-clock-changes)
        * [Building Curves of Daily or Lower Granularity](#building-curves-of-daily-or-lower-granularity)
        * [Building Curves of Higher Than Daily Granularity](#building-curves-of-higher-than-daily-granularity)
* [Technical Documentation](#technical-documentation)
* [Building](#building)
    * [Build Prerequisites](#build-prerequisites)
    * [Running the Build](#running-the-build)
    * [Build Artifacts](#build-artifacts)
    * [Building from Linux and macOS](#building-from-linux-and-macos)
* [Why the Strange Tech Stack?](#why-the-strange-tech-stack)
* [Debugging C# Code From a Jupyter Notebook](#debugging-c-code-from-a-jupyter-notebook)
    * [Debugging a Released PyPI Package](#debugging-a-released-pypi-package)
    * [Debugging Code With Custom Modifications](#debugging-code-with-custom-modifications)
* [License](#license)

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

The core of the curves package essentially consists of two models; the bootstrapper and the spline.

* The **bootstrapper** model takes a set of forward prices, for contracts with overlapping delivery/fixing periods, and returns a curve with overlapping periods removed, but with prices consistent with the original inputs. In addition the bootstrapper can be used to apply shaping to the forward prices by applying a predefined spread or ratios between pairs of contract prices.
* The **spline** model allows the creation of a smooth curve, with higher granularity than the input contracts. This uses a [maximum smoothness algorithm](docs/max_smoothness/max_smoothness_spline.pdf) to interpolate input contracts with a fourth-order spline, whilst maintaining the average price constraints inherent in the input contract prices.

See [Getting Started](#getting-started) below for more details on how to use these two model from both C# and Python.

## Installing
For use from C# install the NuGet package Cmdty.Curves.
```
PM> Install-Package Cmdty.Curves
```

For use from Python install the curves package from PyPI.
```
> pip install curves
```

## .NET Dependency
As Cmdty.Curves is mostly written in C# it requires the .NET runtime to be installed to execute.
The dlls are targetting [.NET Standard 2.0](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0) which is compatible with .NET Framework versions 4.6.1
upwards. A version of .NET Framework meeting this restriction should be installed on most
Windows computers, so nothing extra is required.

If running on a non-Windows OS then the runtime of a cross-platform type of .NET will be 
required. .NET Standard is compatible with .NET and Mono, with the former being recommended.
For the Python package, by default it will try to use .NET, and if this isn't installed it will
try Mono. See the Microsoft documentation on installing the .NET runtime on [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux)
and on [macOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos).

## Getting Started

### Using From C#
This section gives some basic examples of using the C# API.
For more sophisticated examples of usage see [samples/csharp/](https://github.com/cmdty/curves/tree/master/samples/csharp).
The C# API makes extensive use of the [Time Period Value Types](https://github.com/cmdty/time-period-value-types) library for 
representing delivery periods as time periods.
#### Bootstrapper
The C# code below gives an example of user the bootstrapper on overlapping Q1-20 and Jan-20 forward prices.
```c#
BootstrapResults<Month> bootstrapResults = new Bootstrapper<Month>()
                    .AddContract(Month.CreateJanuary(2020), 19.05)
                    .AddContract(Quarter.CreateQuarter1(2020), 17.22)
                    .Bootstrap();

Console.WriteLine("Derived piecewise flat curve:");
Console.WriteLine(bootstrapResults.Curve.FormatData("F5"));

Console.WriteLine();

Console.WriteLine("Equivalent bootstrapped contracts:");
foreach (Contract<Month> contract in bootstrapResults.BootstrappedContracts)
{
    Console.WriteLine(contract);
}
```
This results in the following to be printed to the console.
```
Derived piecewise flat curve:
Count = 3
2020-01  19.05000
2020-02  16.27450
2020-03  16.27450

Equivalent bootstrapped contracts:
Start: 2020-01, End: 2020-01, Price: 19.05
Start: 2020-02, End: 2020-03, Price: 16.2745
```

See [Program.cs](https://github.com/cmdty/curves/tree/master/samples/csharp/Cmdty.Curves.Samples.Bootstrap/Program.cs) for examples of applying shaping using the bootstrapper, and alternative average weighting schemes, e.g. business day weighting.

#### Spline
The following C# code shows how to use the spline to derive a smooth daily curve
from monthly and quarterly granularity input contract prices. Also demonstrated is
the optional application of a seasonal adjustment factor, in this case used to apply day-of-week
seasonality.

```c#
var dayOfWeekAdjustment = new Dictionary<DayOfWeek, double>
{
    [DayOfWeek.Monday] = 0.95,
    [DayOfWeek.Tuesday] = 0.99,
    [DayOfWeek.Wednesday] = 1.05,
    [DayOfWeek.Thursday] = 1.01,
    [DayOfWeek.Friday] = 0.98,
    [DayOfWeek.Saturday] = 0.92,
    [DayOfWeek.Sunday] = 0.91
};

DoubleCurve<Day> curve = new MaxSmoothnessSplineCurveBuilder<Day>()
    .AddContract(Month.CreateJuly(2019), 77.98)
    .AddContract(Month.CreateAugust(2019), 76.01)
    .AddContract(Month.CreateSeptember(2019), 78.74)
    .AddContract(Quarter.CreateQuarter4(2019), 85.58)
    .AddContract(Quarter.CreateQuarter1(2020), 87.01)
    .WithMultiplySeasonalAdjustment(day => dayOfWeekAdjustment[day.DayOfWeek])
    .BuildCurve();

Console.WriteLine(curve.FormatData("F5"));
```
Which prints the following.
```
Count = 275
2019-07-01  77.68539
2019-07-02  80.83184
2019-07-03  85.59869
2019-07-04  82.21079
....................
2020-03-28  81.21869
2020-03-29  80.30742
2020-03-30  83.80771
2020-03-31  87.30550
```

See [Program.cs](https://github.com/cmdty/curves/tree/master/samples/csharp/Cmdty.Curves.Samples.Spline/Program.cs) for an example of using the spline with alternative average weighting scheme, e.g. business day weighting.

### Using From Python
This section gives same basic example of using the Python package.
See the Jupyter Notebook [curves_tutorial](samples/python/curves_tutorial.ipynb) for a more thorough introduction.

#### Bootstrapper
Below is a basic example showing prices for January and Q1 delivery period periods being bootstrapped into consistent January, February and March forward prices.
``` python
from curves import bootstrap_contracts
from datetime import date

q1_price = 19.05
contracts = [
    (date(2019, 1, 1), 18.95), # Jan-19
    (date(2019, 1, 1), date(2019, 3, 1), 19.05) # Q1-19
]
piecewise_curve, bootstrapped_contracts = bootstrap_contracts(contracts, freq='M')
print(piecewise_curve)
print()
for bc in bootstrapped_contracts:
    print("{0}, {1}, {2:.3f}".format(repr(bc.start), repr(bc.end), bc.price))
```

The above code prints to the following:
```
2019-01    18.950000
2019-02    19.102542
2019-03    19.102542
Freq: M, dtype: float64

Period('2019-01', 'M'), Period('2019-01', 'M'), 18.950
Period('2019-02', 'M'), Period('2019-03', 'M'), 19.103
```
#### Spline
The example below creates a daily granularity curve, from input contracts of various granularities. As would usually be the case in a practical scenario, 
the bootstrap_contracts method is first used to remove the overlaps from the contracts.
```python
from curves import max_smooth_interp
from curves import contract_period as cp
import pandas as pd

contracts = [
    (date(2019, 5, 31), 34.875), 
    (date(2019, 6, 1), date(2019, 6, 2), 32.87),
    ((date(2019, 6, 3), date(2019, 6, 9)), 32.14),
    (pd.Period(year=2019, month=6, freq='M'), 31.08),
    (cp.month(2019, 7), 29.95),
    (cp.q_3(2019), 30.18),
    (cp.q_4(2019), 37.64),
    (cp.winter(2019), 38.05),
    (cp.summer(2020), 32.39),
    (cp.winter(2020), 37.84),
    (cp.gas_year(2020), 35.12)
]

pc_for_spline, bc_for_spline = bootstrap_contracts(contracts, freq='D')
smooth_curve = max_smooth_interp(bc_for_spline, freq='D')

print(smooth_curve)
```

Results in the following being printed:
```
2019-05-31    34.875000
2019-06-01    33.404383
2019-06-02    32.335617
2019-06-03    31.800171
2019-06-04    31.676636
2019-06-05    31.804146
2019-06-06    32.057113
2019-06-07    32.337666
2019-06-08    32.575648
2019-06-09    32.728620
2019-06-10    32.781858
2019-06-11    32.745075
                ...    
2021-09-19    26.727181
2021-09-20    26.652039
2021-09-21    26.576895
2021-09-22    26.501749
2021-09-23    26.426602
2021-09-24    26.351454
2021-09-25    26.276305
2021-09-26    26.201156
2021-09-27    26.126006
2021-09-28    26.050856
2021-09-29    25.975706
2021-09-30    25.900556
Freq: D, Length: 854, dtype: float64
```

### Python Version Compatibility
The curves package should be compatible with the Python interpreter up to **version 3.11**.

Limitations on the Python version which the curves package can be used
are largely driven by the [pythonnet](https://github.com/pythonnet/pythonnet) package dependency. The latest version of curves (1.3.0) depends on
pythonnet version 3.0.1, which itself works with Python up to version 3.11.
Hence this is also the maximum version with which curves works.

## Handling Clock Changes
Clock changes cause complications in two scenarios when building curves. Both of these are most relevant
when building power (electricity) forward curves where the additional (or lost time) caused
by a clock change results in more (or less) of the commodity being delivered. In such markets
physical nomination, and contracts at the very front of the curve will be in higher than
daily granularity, for example half-hourly in UK power markets.

### Building Curves of Daily or Lower Granularity
This case is simple to handle. If for example building a daily power forward curve,
the weighting function provided to the bootstrapper or spline needs to take into account
different volume of power delivered in each day due to the clock change. Ignoring the discount 
factor in the weighting for simplicity, take as an example the weighting function used for a 
baseload daily power curve. This would evaluate to:
* 24.0 for days when there is no clock change, hence 24 hours in the day.
* 25.0 for days when clocks move back an hour, hence there is an extra hour in the day.
* 23.0 for dailys when clocks move forward and hour, hence there is an hour missing.

### Building Curves of Higher Than Daily Granularity
An example of this would be building a half-hourly power forward curve. Unfortunately,
at present the Cmdty.Curves library does not support the use of time zone aware types to 
represent the contract delivery periods. This would be the ideal way to handle the problem.
As such, the correct way to handle clock changes is to tranform all contract delivery periods
to their UTC time equivalents before feeding into the Cmdty.Curves algorithms. UTC time is
continuous and so does not have the complication of clock changes. The resulting
curve produced would also be represented in UTC time, so the caller might want to transform
this back to local time, using time zone aware types for the index of the collections holding
the derived high-granularity curve.


## Technical Documentation
The PDF file [max_smoothness_spline.pdf](docs/max_smoothness/max_smoothness_spline.pdf) contains details of the mathematics behind the maximum smoothness algorithm.

## Building
This section describes how to run a scripted build on a cloned repo. Visual Studio 2019 is used for development, and can also be used to build the C# and run unit tests on the C# and Python APIs. However, the scripted build process also creates packages (NuGet and Python), builds the C# samples, and verifies the C# interactive documentation. [Cake](https://github.com/cake-build/cake) is used for running scripted builds. This is relevant for running the scripted build on Windows. For running on a non-Windows OS see [Building from Linux and macOS](#building-from-linux-and-macos).

#### Build Prerequisites
The following are required on the host machine in order for the build to run.
* The .NET Core SDK. Check the [global.json file](global.json) for the version necessary, taking into account [the matching rules used](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json#matching-rules).
* The Python interpretter, accessible by being in a file location in the PATH environment variable. Version 3.7 is used, although other 3.x versions might work.
* The following Python packages installed:
    * virtualenv.
    * setuptools.
    * wheel.

#### Running the Build
The build is started by running the PowerShell script build.ps1 from a PowerShell console, ISE, or the Visual Studio Package Manager Console.

```
PM> .\build.ps1
```

#### Build Artifacts
The following results of the build will be saved into the artifacts directory (which itelf will be created in the top directory of the repo).
* The NuGet package: Cmdty.Curves.[version].nupkg
* The Python package files:
    * curves-[version]-py3-none-any.whl

### Building from Linux and macOS
Running the full build on non-Windows plaforms is still work in progress- the aim to to make it completely plaform agnostic. However, at the moment only the C# parts of the build are functioning cross-plaform.

The Cake build can be invoked using the bootstrapper Bash script build.sh. After first granting it execute permissions as below, the "Pack-NuGet" target results in the building and unit testing of the C#, before the creation of the Cmdty.Curves NuGet package.
```
> chmod +x build.sh
> ./build.sh --target=Pack-NuGet.
```

Alternatively, if [PowerShell Core](https://github.com/PowerShell/PowerShell) is installed,
the build can be run with the following command:
```
> pwsh ./build.ps1
```


## Why the Strange Tech Stack?
Users of the Python API might be perplexed as to the technology used: Python calling into .NET, which itself calls into native code for the Intel MKL numerical routines.
This is certainly not a common structure, especially for a package focussed on complex numerical calculations.

The Cmdty project started off as a .NET only project, written in C#, due to the author being mainly
a C# guy during the day-job. The Python wrapper was added later as it became apparent that there was a demand to
use the models from Python. Since then it now seems that there are many more users of the Python API than
the C# NuGet package, resulting in significant time being spent on the Python API, and documentation.

If the project was started again from scratch, it would have been written entirely in Python. However, due to time constraints, and not wanting to have the maintenance headache of 
having two independent implementations side-by-side there is no plan to work on this. That said,
if others want to have a go at a pure Python implementation it would be very much welcomed and I would
happily help out.

A Python-only implementation would solve the following problems:
* Limitations to the Python version, as described [above](#python-version-compatibility).
* Currently only certain curve granularities (Pandas period frequencies) are supported, as described at the bottom of the [PyPI package README](https://pypi.org/project/curves/).
* Reduce the size of the package.
* Makes it easy to use the PyPI package on non-Windows platforms. Use on Linux is possible, but requires that the Mono runtime be installed.

## Debugging C# Code From a Jupyter Notebook
This section contains the procedure to follow in order to debug the calculations in the C# 
code, as invoked from Python running in a Jupyter notebook. The following steps are a prerequisite
to the procedures described below.
* Install the following software for building the C#:
    * Visual Studio 2022.
    * The .NET Core SDK version, as described in the section [Build Prerequisites](#build-prerequisites).
* In Visual Studio uncheck the box for the debugging option "Enable Just My Code" in Tools > Options > Debugging > General.
* Clone the curves repo onto your machine.

The below descriptions have been used from a Windows desktop. As Visual Studio is available for
Apple computers a similar procedure might work with Apple hardware, but has never been tested.


### Debugging a Released PyPI Package
This section describes how to debug the execution of the curves package installed from PyPI.
* Do a git checkout to the git tag associated with the version of the curves package you are 
running in Jupyter. The git tags for each release are found on GitHub [here](https://github.com/cmdty/curves/tags).
* In the cloned repo open Cmdty.Curves.sln in Visual Studio and build in Debug configuration.
* Set breakpoints in the C# code. The most useful places for breakpoints are at the start of
the core calculations:
    * For the spline, the Build method of the class MaxSmoothnessSplineCurveBuilder, as found in [MaxSmoothnessSplineCurveBuilder.cs](./src/Cmdty.Curves/MaxSmoothness/MaxSmoothnessSplineCurveBuilder.cs).
    * For the bootstrapper, the Calculate method of the class Bootstrapper, as found in [Bootstrapper](./src/Cmdty.Curves/Bootstrap/Bootstrapper.cs).
* It is likely that there are many running processes for the python.exe interpretter. It is
necessary to identify the PID (Process ID) of the exact python.exe process which is being used
by Jupyter. One way to do this uses [Sysinternals Process Explorer](https://learn.microsoft.com/en-us/sysinternals/downloads/process-explorer):
    * Launch Process Explorer and ensure PID is one of the displayed columns.
    * Order the displayed processes by process name and locate the section which contains the
    python.exe processes.
    * Run the Jupyter notebook and observe the specific python.exe process for which the CPU usage 
    increases, taking a note of the PID. In the image below the PID is found to be 33568.
    ![Identifying PID](./assets/debug_identify_python_process.png)
* In the Visual Studio menu bar select Debug > Attach to Process. In the resulting dialogue box
search the processes using the noted PID. Select this process and press the Attach button.
* Execute the Jupyter notebook. The C# code should break at the placed breakpoints.

### Debugging Code With Custom Modifications
This section describes the more advanced scenario of running and debugging Cmdty.Curves
code which has been modified, and so is different to that used to created released PyPI packages.
The process of debugging the C# code with custom modifications is identical to that described
above, except that a [pip local project install](https://pip.pypa.io/en/stable/topics/local-project-installs/) is required. This should be done in the Anaconda Prompt using the
path of the directory src\Cmdty.Curves.Python\ within the cloned curves repo as the path in
the pip install command.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
