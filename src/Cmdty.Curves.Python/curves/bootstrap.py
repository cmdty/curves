# Copyright(c) 2019 Jake Fowler
#
# Permission is hereby granted, free of charge, to any person
# obtaining a copy of this software and associated documentation
# files (the "Software"), to deal in the Software without
# restriction, including without limitation the rights to use, 
# copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the
# Software is furnished to do so, subject to the following
# conditions:
#
# The above copyright notice and this permission notice shall be
# included in all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
# EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
# OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
# NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
# HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
# WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
# FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
# OTHER DEALINGS IN THE SOFTWARE.

import clr
from System import Func, Double, DayOfWeek
from pathlib import Path
clr.AddReference(str(Path("curves/lib/Cmdty.Curves")))
from Cmdty.Curves import Bootstrapper, IBootstrapper, BootstrapperExtensions, IBootstrapperAddOptionalParameters, IBetween, Shaping, IIs, IAnd
from collections import namedtuple
from curves._common import FREQ_TO_PERIOD_TYPE, tranform_time_func, net_time_series_to_pandas_series, contract_period, net_time_period_to_pandas_period, deconstruct_contract

Contract = namedtuple('Contract', 'start, end, price')
BootstrapResults = namedtuple('BootstrapResults', 'piecewise_curve, bootstrapped_contracts')

def bootstrap_contracts(contracts, freq, average_weight=None, shaping_ratios=None, shaping_spreads=None, allow_redundancy=False):
    """
    Bootstraps a collection of commodity forward/swap/futures prices by removing the overlapping periods and optionally applies shaping.

    Args:
        contracts (iterable): iterable of tuples, with each tuple describing a forward delivery period and price in one
            of the following forms:
                ([period], [price]) 
                ([period start], [period end], [price])
                (([period start], [period end]), [price])
            Where:
                [price] (float) is the price of commodity delivered over the delivery period.
                [period] specifies the delivery period.
                [period start] specifies the start of the contract delivery period.
                [period end] specifies the inclusive end of the contract delivery period.
            [period], [period start] and [period end] can be any of the following types:
                pandas.Period
                date
                datetime
        freq (str): Describes the granularity of curve being constructed using pandas Offset Alias notation. 
            Must be a key to the dict variable curves.FREQ_TO_PERIOD_TYPE.
        average_weight (callable, optional): Mapping from pandas.Period type to float which describes the weighting
            that each forward period contributes to a price for delivery which spans multiple periods. The
            pandas.Period parameter will have freq equal to the freq parameter. An example of such weighting is
            a monthly curve (freq='M') of a commodity which delivers on every calendar day. In this example average_weight would be
            a callable which returns the number of calendar days in the month, e.g.:
                lambda p: p.asfreq('D', 'e').day
            Defaults to None, in which case each period has equal weighting.
        shaping_ratios (iterable, optional): iterable of tuples, with each tuple describing a constraint on the ratio
            between the prices of different periods on the derived forward curve in the form:
                ([numerator period], [denominator period], [ratio])
            Where:
                [ratio] (float) is the ratio between the forward prices.
                [numerator period] is the delivery period corresponding to the numerator part of the ratio.
                [denominator period] is the delivery period corresponding to the denomintor part of the ratio.
            [numerator period] and [denominator period] can be any of the following types:
                pandas.Period
                date
                datetime
                A 2-tuple of any of the above three types, with the elements specifying the period start and end respectively.
        shaping_spreads (iterable, optional): iterable of tuples, with each tuple describing a constraint on the spread
            between the prices of different periods on the derived forward curve in the form:
                ([period long], [period short], [spread])
            Where:
                [spread] (float) is the spread between the forward prices.
                [period long] is the delivery period for the long part of the spread.
                [period short] is the delivery period for the short part of the spread.
            [period long] and [period short] can be any of the following types:
                pandas.Period
                date
                datetime
                A 2-tuple of any of the above three types, with the elements specifying the period start and end respectively.
        allow_redundancy (bool, optional): Flag indicating whether the input contracts are allowed to have redundancy
            without an exception being thrown. An example of redundancy is contracts including prices for
            quarter and also all three constituent months of the same quarter. Defaults to False.

    Returns:
        (pandas.Series, list of tuple): named tuple with the following elements:
        piecewise_curve: A contiguous forward curve, consistent with the contracts parameter.
            This pandas.Series will have an index of type PeriodIndex and freq equal to the freq parameter.
        bootstrapped_contracts: Equivalent to contracts parameter, but with overlapping periods removed, represented by a 3-item named tuple (start, end, price):
            start (pandas.Period): Inclusive start of the contract delivery period.
            end (pandas.Period): Inclusive end of the contract delivery period.
            price (float): Forward price of commodity delivered over periods specified by start and end.
    """
    if freq not in FREQ_TO_PERIOD_TYPE:
        raise ValueError("freq parameter value of '{}' not supported. The allowable values can be found in the keys of the dict curves.FREQ_TO_PERIOD_TYPE.".format(freq))

    time_period_type = FREQ_TO_PERIOD_TYPE[freq]
    
    bootstrapper = IBootstrapperAddOptionalParameters[time_period_type](Bootstrapper[time_period_type]())

    for contract in contracts:
        (period, price) = deconstruct_contract(contract)
        (start, end) = contract_period(period, freq, time_period_type)
        BootstrapperExtensions.AddContract[time_period_type](bootstrapper, start, end, price)

    if allow_redundancy == True:
        bootstrapper.AllowRedundancy()

    if shaping_ratios is not None:
        for (num, denom, ratio) in shaping_ratios:
            (num_start, num_end) = contract_period(num, freq, time_period_type)
            (denom_start, denom_end) = contract_period(denom, freq, time_period_type)
            shaping_ratio = IIs[time_period_type](IAnd[time_period_type](IBetween[time_period_type](Shaping[time_period_type].Ratio).Between(num_start, num_end)).And(denom_start, denom_end)).Is(ratio)
            bootstrapper.AddShaping(shaping_ratio)

    if shaping_spreads is not None:
        for (period1, period2, spread) in shaping_spreads:
            (period1_start, period1_end) = contract_period(period1, freq, time_period_type)
            (period2_start, period2_end) = contract_period(period2, freq, time_period_type)
            shaping_spread = IIs[time_period_type](IAnd[time_period_type](IBetween[time_period_type](Shaping[time_period_type].Spread).Between(period1_start, period1_end)).And(period2_start, period2_end)).Is(spread)
            bootstrapper.AddShaping(shaping_spread)

    if average_weight is not None:
        tranformed_average_weight = tranform_time_func(freq, average_weight)
        bootstrapper.WithAverageWeighting(Func[time_period_type, Double](tranformed_average_weight))

    dotnet_bootstrap_results = bootstrapper.Bootstrap()
    
    piecewise_curve = net_time_series_to_pandas_series(dotnet_bootstrap_results.Curve, freq)

    bootstrapped_contracts = []
    for contract in dotnet_bootstrap_results.BootstrappedContracts:
        bootstrapped_contracts.append(Contract(net_time_period_to_pandas_period(contract.Start, freq), net_time_period_to_pandas_period(contract.End, freq), contract.Price))

    return BootstrapResults(piecewise_curve, bootstrapped_contracts)
