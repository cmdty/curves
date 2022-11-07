# Copyright(c) 2022 Jake Fowler
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

import numpy as np
import pandas as pd
from scipy import interpolate, optimize
from typing import Optional, Callable, Union
from curves._common import _last_period, ContractsType, deconstruct_contract, contract_pandas_periods


def shape_preserving_average_spline(contracts: Union[ContractsType, pd.Series],
                                    freq: str,
                                    mult_season_adjust: Optional[Callable[[pd.Period], float]] = None,
                                    add_season_adjust: Optional[Callable[[pd.Period], float]] = None,
                                    average_weight: Optional[Callable[[pd.Period], float]] = None,
                                    time_func: Optional[Callable[[pd.Period, pd.Period], float]] = None,
                                    optimize_method: Optional[str] = None,
                                    optimize_tol: Optional[float] = None,
                                    optimize_options: Optional[dict] = None
                                    ) -> pd.Series:
    """
    Quick hack for an averaging shape-preserving spline algorithm. Very slow, inaccurate and potentially unstable.
    In the future a proper implementation will be released which won't have these issues. Use at your own risk!

    Generally this function is used to increase the granularity of a curve from a collection of forward prices for non-overlapping
    delivery periods. The resulting curve will be smooth, and of homogenous granularity. For example a smooth monthly curve can be
    created from a collection of monthly, quarterly and season granularity forward market prices. If input contracts have overlapping
    delivery periods the function bootstrap_contracts (also in this package) can be used to remove the overlaps before a call to
    this function.

    Args:
        contracts (pd.Series or iterable): The input contracts to be interpolated.
            If iterable of tuples, with each tuple describing a forward delivery period and price in one
            of the following forms:
                ([period], [price])
                ([period start], [period end], [price])
                (([period start], [period end]), [price])
            Where:
                [price] is a float, being the price of commodity delivered over the delivery period.
                [period] specifies the delivery period.
                [period start] specifies the start of the contract delivery period.
                [period end] specifies the inclusive end of the contract delivery period.
            [period], [period start] and [period end] can be any of the following types:
                pandas.Period
                date
                datetime
        freq (str): Describes the granularity of curve being constructed using pandas Offset Alias notation.
            Must be a key to the dict variable curves.FREQ_TO_PERIOD_TYPE.
        mult_season_adjust (callable, optional): Callable with single parameter of type pandas.Period and return type float.
            If this argument is supplied, the value from the underlying spline funtion is multiplied by the result of mult_season_adjust,
            evaluated for each index period in the resulting curve.
        add_season_adjust (callable, optional): Callable with single parameter of type pandas.Period and return type float.
            If this argument is supplied, the value from the underlying spline funtion has the result of add_season_adjust,
            evaluated for each index period, added to it to derive each price in the resulting curve.
        average_weight (callable, optional): Mapping from pandas.Period type to float which describes the weighting
            that each forward period contributes to a price for delivery which spans multiple periods. The
            pandas. Period parameter will have freq equal to the freq parameter. An example of such weighting is
            a monthly curve (freq='M') of a commodity which delivers on every calendar day. In this example average_weight would be
            a callable which returns the number of calendar days in the month, e.g.:
                lambda p: p.asfreq('D', 'e').day
            Defaults to None, in which case each period has equal weighting.
        time_func (callable, optional): Callable accepting two parameters, both of type pandas.Period, with return type of float.
            This parameter specifies how the small-t variable of the spline polynomials is calculated from the index periods of the curve
            being constructed. Small-t for each curve point will be calculated as time_func evaluated with the first period at the front of
            the derived curve as the first argument, and the period for the specific curve point as the second argument. If this parameter
            is omitted time_func will default to the number of periods difference between the two parameter periods.
        optimize_method (string, optional): Value passed to scipy.optimize.minimize function method argument when fitting the curve.
        optimize_tol (float, optional): Value passed to scipy.optimize.minimize function tol argument when fitting the curve.
        optimize_options (dict, optional): Value passed to scipy.optimize.minimize function options argument when fitting the curve.

    Returns:
        pandas.Series: Series with index of type PeriodIndex and freqstr equal to the freq parameter. This Series will
            represent a smooth contiguous curve with values consistent with prices within the contracts parameter.
    """
    num_contracts = len(contracts)
    if num_contracts == 0:
        raise ValueError('Contracts has no elements.')

    mult_season_adjust = _one if mult_season_adjust is None else mult_season_adjust
    add_season_adjust = _zero if add_season_adjust is None else add_season_adjust
    average_weight = _one if average_weight is None else average_weight
    time_func = _default_time_func if time_func is None else time_func

    standardised_contracts = []  # Contract as tuples of (PeriodIndex, price)
    if isinstance(contracts, pd.Series):
        for period, price in contracts.items():
            start_period = period.asfreq(freq, 's')
            end_period = _last_period(period, freq)
            contract_periods = pd.period_range(start_period, end_period)
            standardised_contracts.append((contract_periods, price))
    else:
        for contract in contracts:
            period, price = deconstruct_contract(contract)
            start_period, end_period = contract_pandas_periods(period, freq)
            contract_periods = pd.period_range(start_period, end_period)
            standardised_contracts.append((contract_periods, price))

    base_period = standardised_contracts[0][0][0]
    times_to_mid = np.zeros(num_contracts)
    guess_y_points = np.zeros(num_contracts)
    for idx, (contract_periods, price) in enumerate(standardised_contracts):
        start = contract_periods[0]
        end = contract_periods[-1]
        times_to_mid[idx] = (time_func(base_period, start) + time_func(base_period, end)) / 2.0
        guess_y_points[idx] = price

    def forward_curve_error(y_points):
        pchip = interpolate.PchipInterpolator(times_to_mid, y_points, extrapolate=True)
        sum_squared_error = 0.0
        for (idx2, (contract_periods2, contract_price)) in enumerate(standardised_contracts):
            sum_weighted_pchip_contract_price = 0.0
            sum_weight = 0.0
            for contract_period in contract_periods2:
                weight = average_weight(contract_period)
                add_adjust = add_season_adjust(contract_period)
                mult_adjust = mult_season_adjust(contract_period)
                sum_weight += weight
                time_to_contract_date = time_func(base_period, contract_period)
                interpolated_price = pchip(time_to_contract_date)
                seasonally_adjusted_interpolated_price = (interpolated_price + add_adjust) * mult_adjust
                sum_weighted_pchip_contract_price += seasonally_adjusted_interpolated_price * weight
            avg_pchip_contract_price = sum_weighted_pchip_contract_price / sum_weight
            sum_squared_error += (avg_pchip_contract_price - contract_price) ** 2
        return sum_squared_error

    optimize_results = optimize.minimize(forward_curve_error, guess_y_points, tol=optimize_tol,
                                         method=optimize_method, options=optimize_options)
    if not optimize_results.success:
        raise ValueError('Optimisation not successful ' + optimize_results.message)

    best_pchip = interpolate.PchipInterpolator(times_to_mid, optimize_results.x, extrapolate=True)
    first_period = standardised_contracts[0][0][0]
    last_period = standardised_contracts[-1][0][-1]
    result_index = pd.period_range(start=first_period, end=last_period)
    smooth_curve = pd.Series(index=result_index, data=np.zeros(len(result_index)), dtype=np.float64)
    for period in result_index:
        time_to_period = time_func(base_period, period)
        smooth_price = best_pchip(time_to_period)
        add_adjust = add_season_adjust(period)
        mult_adjust = mult_season_adjust(period)
        smooth_curve[period] = (smooth_price + add_adjust) * mult_adjust
    return smooth_curve


def _default_time_func(period1, period2):
    time_stamp1 = period1.start_time
    time_stamp2 = period2.start_time
    time_delta = time_stamp2 - time_stamp1
    return time_delta.total_seconds() / 60.0 / 60.0 / 24.0 / 365.0  # Convert to years with ACT/365


def _zero(period):
    return 0.0


def _one(period):
    return 1.0
