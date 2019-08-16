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

import pandas as pd

def _num_calendar_days(period):

    if period.freqstr == 'D':
        return 1

    return (period.asfreq('D', 'e') - period.asfreq('D', 's')).delta.days + 1


def weighted_average_slice_curve(curve, freq, input_period, weighting=_num_calendar_days):
    if isinstance(input_period, tuple):
        start = input_period[0]
        end = input_period[1]
    else:
        if isinstance(input_period, pd.Period):
            start = input_period.asfreq(freq, 's')
            end = input_period.asfreq(freq, 'e')
        else:
            start = input_period
            end = input_period
    curve_slice = curve[start:end]
    return weighted_average(curve_slice, weighting)


def weighted_average(ts, weighting):
    weights_array = ts.index.map(weighting).values
    weighted_values = weights_array * ts.values
    return weighted_values.sum() / weights_array.sum()

