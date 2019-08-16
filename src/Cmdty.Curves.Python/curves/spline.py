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
import pandas as pd
from datetime import datetime
from System import Func, Double
from curves._common import FREQ_TO_PERIOD_TYPE, tranform_time_func, tranform_two_period_func, net_time_series_to_pandas_series, contract_period, deconstruct_contract
from pathlib import Path
clr.AddReference(str(Path("curves/lib/Cmdty.Curves")))
from Cmdty.Curves import MaxSmoothnessSplineCurveBuilder, MaxSmoothnessSplineCurveBuilderExtensions, ISplineAddOptionalParameters


def max_smooth_interp(contracts, freq, mult_season_adjust=None, 
                       add_season_adjust=None, average_weight=None, time_func=None, 
                       front_1st_deriv=None, back_1st_deriv=None):
    """
    Creates a smooth interpolated curve from a collection of commodity forward/swap/futures prices using maximum smoothness algorithm.

    Generally this function is used to increase the granularity of a curve from a collection of forward prices for non-overlapping
    delivery periods. The resulting curve will be smooth, and of homogenous granularity. For example a smooth monthly curve can be 
    created from a collection of monthly, quarterly and season granularity forward market prices. If input contracts have overlapping 
    delivery periods the function bootstrap_contracts (also in this package) can be used to remove the overlaps before a call to
    this function.

    Args:
        contracts (iterable): iterable of tuples, with each tuple describing a forward delivery period and price in one
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
            pandas.Period parameter will have freq equal to the freq parameter. An example of such weighting is
            a monthly curve (freq='M') of a commodity which delivers on every calendar day. In this example average_weight would be
            a callable which returns the number of calendar days in the month, e.g.:
                lambda p: p.asfreq('D', 'e').day
            Defaults to None, in which case each period has equal weighting.
        time_func (callable, optional): Callable accepting two parameters, both of type pandas.Period, with return type of float.
            This parameter specifies how the small-t variable of the spline polynomials is calculated from the index periods of the curve
            being constructed. Small-t for each curve point will be calculated as time_func evaluated with the first period at the front of 
            the derived curve as the first argument, and the period for the specific curve point as the second argument. If this parameter
            is omitted time_func will default to the number of periods difference between the two parameter periods.
        front_1st_deriv (float, optional): Constraint specifying what the first derivative of the spline at the very start of the 
            curve must be. Used to add some optional control of the curve generated. If this parameter is omitted no constraint is applied.
        back_1st_deriv (float, optional): Constraint specifying what the first derivative of the spline at the end of the 
            curve must be. Used to add some optional control of the curve generated. If this parameter is omitted no constraint is applied.

    Returns:
        pandas.Series: Series with index of type PeriodIndex and freqstr equal to the freq parameter. This Series will
            represent a smooth contiguous curve with values consistent with prices within the contracts parameter.

    Note:
        The underlying algorithm uses a fourth-order spline, solved with the constraint of averaging back to the input contract
        prices, under the criteria of maximising the smoothness. In this implementation maximising the smoothness is defined as
        minimising the integral of the second derivative squared.
    """
    
    if freq not in FREQ_TO_PERIOD_TYPE:
        raise ValueError("freq parameter value of '{}' not supported. The allowable values can be found in the keys of the dict curves.FREQ_TO_PERIOD_TYPE.".format(freq))

    time_period_type = FREQ_TO_PERIOD_TYPE[freq]

    spline_builder = ISplineAddOptionalParameters[time_period_type](MaxSmoothnessSplineCurveBuilder[time_period_type]())

    for contract in contracts:
        (period, price) = deconstruct_contract(contract)
        (start, end) = contract_period(period, freq, time_period_type)
        MaxSmoothnessSplineCurveBuilderExtensions.AddContract[time_period_type](spline_builder, start, end, price)

    if mult_season_adjust is not None:
        transformed_mult_season_adjust = tranform_time_func(freq, mult_season_adjust)
        spline_builder.WithMultiplySeasonalAdjustment(Func[time_period_type, Double](transformed_mult_season_adjust))

    if add_season_adjust is not None:
        tranformed_add_season_adjust = tranform_time_func(freq, add_season_adjust)
        spline_builder.WithAdditiveSeasonalAdjustment(Func[time_period_type, Double](tranformed_add_season_adjust))
    
    if average_weight is not None:
        tranformed_average_weight = tranform_time_func(freq, average_weight)
        spline_builder.WithWeighting(Func[time_period_type, Double](tranformed_average_weight))

    if time_func is not None:
        tranformed_time_func = tranform_two_period_func(freq, time_func)
        spline_builder.WithTimeFunc(Func[time_period_type, time_period_type, Double](tranformed_time_func))

    if front_1st_deriv is not None:
        spline_builder.WithFrontFirstDerivative(front_1st_deriv)

    if back_1st_deriv is not None:
        spline_builder.WithBackFirstDerivative(back_1st_deriv)

    curve = spline_builder.BuildCurve()

    result = net_time_series_to_pandas_series(curve, freq)

    return result

