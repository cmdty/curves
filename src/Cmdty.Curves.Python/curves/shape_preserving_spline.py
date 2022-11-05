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
from datetime import timedelta, date


def shape_preserving_average_spline(contracts) -> pd.Series:
    num_contracts = len(contracts)
    base_date = contracts[0][0]
    times_to_mid = np.zeros(num_contracts)
    guess_y_points = np.zeros(num_contracts)
    for idx, (start, end, price) in enumerate(contracts):
        times_to_mid[idx] = ((start - base_date).days + (end - base_date).days) / 2.0 / 365.0
        guess_y_points[idx] = price

    def forward_curve_error(y_points):
        pchip = interpolate.PchipInterpolator(times_to_mid, y_points, extrapolate=True)
        sum_squared_error = 0.0
        for (idx2, (start2, end2, contract_price)) in enumerate(contracts):
            sum_pchip_contract_price = 0.0
            contract_date = start2
            while contract_date <= end2:
                time_to_contract_date = (contract_date - base_date).days/365.0
                sum_pchip_contract_price += pchip(time_to_contract_date)
                contract_date = contract_date + timedelta(days=1)
            contract_num_days = (end2 - start2).days + 1
            avg_pchip_contract_price = sum_pchip_contract_price / contract_num_days
            sum_squared_error += (avg_pchip_contract_price - contract_price) ** 2
        return sum_squared_error

    optimize_results = optimize.minimize(forward_curve_error, guess_y_points, method='CG', tol=0.0000001)
#                                         method='powell', options={'ftol': 0.000000001})  # TODO another optimiser might be better
    if not optimize_results.success:
        raise ValueError('Optimisation not successful ' + optimize_results.message)

    print(optimize_results)
    best_pchip = interpolate.PchipInterpolator(times_to_mid, optimize_results.x, extrapolate=True)

    first_date = contracts[0][0]
    last_date = contracts[-1][1]
    num_output_periods = (last_date - first_date).days + 1
    index = pd.period_range(start=first_date, freq='D', periods=num_output_periods)
    smooth_curve = pd.Series(index=index, data=np.zeros(num_output_periods), dtype=np.float64)

    date_loop = first_date
    i = 0
    while date_loop <= last_date:
        time_to_date_loop = (date_loop - base_date).days / 365.0
        smooth_price = best_pchip(time_to_date_loop)
        smooth_curve[date_loop] = smooth_price
        date_loop += timedelta(days=1)

    return smooth_curve
