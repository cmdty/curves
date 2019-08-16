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

## Usage
Two functions in the curves package are the core of the curve building calculations; bootstrap_contracts and max_smooth_interp.
Some basic examples of both of these function are given below. Both of these methods make extensive use of pandas, with
the pandas.Series used to represent forward curves, and pandas.Period for the delivery period of an input forward contract
or derived forward curve.

For details of more advanced usage, view the docstrings for either of these functions and the Jupyter Notebook curves_quick_start_tutorial.ipynb 
which can be found in the samples/python/ directory of the cmdty/curves GitHub repository.

### Bootstrapper
The basic functionality of the method bootstrap_contracts is to take a set of forward prices, for contracts with overlapping 
delivery/fixing periods, and return a curve with overlapping periods removed, but with prices consistent with the original inputs.
Below is a basic example showing prices for January and Q1 delivery period periods being bootstrapped into 
consistent January, February and March forward prices.

```python
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

### Spline Interpolation
In order to facility creating a curve with higher granularity than the input contracts, the curves package includes the max_smooth_interp function. 
This uses a maximum smoothness algorithm to interpolate input contracts with a fourth-order spline, whilst maintaining the average price constraints 
inherent in the input contract prices.

The example below creates a daily granularity curve, from input contracts of various granularities. As would usually be the case in a practical scenario, 
the bootstrap_contracts method is first used to remove the overlaps from the contracts. The example below also shows how the input contracts can have gaps 
between them, which the spline will interpolate over, filling in the gaps in the final smooth curve. It also demonstrates the different ways of representing 
the contract delivery period in the contract tuples and use of the helper module contract_period.

```python
from curves import max_smooth_interp
from curves import contract_period as cp

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

The above code prints to the following:
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

