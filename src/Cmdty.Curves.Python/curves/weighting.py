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

""" Provides functions to use as the average_weight parameter to curve construction functions."""
import pandas as pd

def num_business_days(holidays):
    """
    Creates a function which returns the number of business days in a pandas.Period, typically for use as the average_weight parameter for other functions.

    Args:
        holidays (iterable): Collection of date-like objects which represent the holidays for the resulting business day count.

    Returns:
        callable: Function accepting a single parameter of type pandas.Period and returning the number of business days within this
            period as a float.
    """
    def num_business_days_func(period):
        start_day = period.asfreq('D', 's').to_timestamp()
        end_day = period.asfreq('D', 'e').to_timestamp()
        bday_range = pd.bdate_range(start=start_day, end=end_day, freq='C', holidays=holidays)
        return float(len(bday_range))

    return num_business_days_func

# TODO make more efficient by not calling into num_business_days?
def num_weekdays():
    """
    Creates a function which returns the number of weekdays in a pandas.Period, typically for use as the average_weight parameter for other functions.

    Returns:
        callable: Function accepting a single parameter of type pandas.Period and returning the number of weekdays within this
            period as a float.
    """
    return num_business_days([])

# TODO return zero for non-existent period, and 2 times for duplicated periods
# TODO example in docstring
def num_periods(freq, tz=None):
    """
    Creates a function which returns the number of occurances of pandas.Period of a specific freq in an instance of another pandas.Period.

    Args:
        freq (str): Pandas offset alias string for the Period being counted.
        tz (str, optional): Time zone used in calculating the number of periods. If omitted, Periods can be assumed to be in 
            UTC with no clock changes.

    Returns:
        callable: Function accepting a single parameter of type pandas.Period, returning the number of pandas.Period instances, with
            offset specified by the freq parameter, which fit within the parameter Period, as a float.
    """
    def num_periods_func(period):
        start = period.asfreq(freq, 's').to_timestamp()
        end = period.asfreq(freq, 'e').to_timestamp()
        date_range = pd.date_range(start=start, end=end, freq=freq, tz=tz)
        return float(len(date_range))

    return num_periods_func


# TODO implement this function
#def bom_wrapper(wrapped_weighting, pricing_date, pricing_date_fixed):
#    pass

