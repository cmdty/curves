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
""" Provide convenience function for creating an objects which represent the delivery period of a forward contract."""

from datetime import date
import pandas as pd

def month(year, month):
    """
    Creates an instance of pandas.Period representing a specific month.

    Args:
        year (int): Year of the month period being created.
        month (int): Month of year (from 1 to 12) of month period being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing calendar month as specified by the month and year parameters.
    """
    return pd.Period(year=year, month=month, freq='M')


def jan(year):
    """
    Creates an instance of pandas.Period representing the month of January for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month January of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=1, freq='M')


def feb(year):
    """
    Creates an instance of pandas.Period representing the month of February for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month February of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=2, freq='M')


def mar(year):
    """
    Creates an instance of pandas.Period representing the month of March for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month March of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=3, freq='M')


def apr(year):
    """
    Creates an instance of pandas.Period representing the month of April for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month April of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=4, freq='M')


def may(year):
    """
    Creates an instance of pandas.Period representing the month of May for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month May of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=5, freq='M')


def jun(year):
    """
    Creates an instance of pandas.Period representing the month of June for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month June of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=6, freq='M')


def jul(year):
    """
    Creates an instance of pandas.Period representing the month of July for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month July of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=7, freq='M')


def aug(year):
    """
    Creates an instance of pandas.Period representing the month of August for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month August of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=8, freq='M')


def sep(year):
    """
    Creates an instance of pandas.Period representing the month of September for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month September of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=9, freq='M')


def oct(year):
    """
    Creates an instance of pandas.Period representing the month of October for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month October of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=10, freq='M')


def nov(year):
    """
    Creates an instance of pandas.Period representing the month of November for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month November of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=11, freq='M')


def dec(year):
    """
    Creates an instance of pandas.Period representing the month of December for a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'M' representing the calendar month December of the year specified by the year parameter.
    """
    return pd.Period(year=year, month=12, freq='M')


def quarter(year, quarter):
    """
    Creates an instance of pandas.Period representing a specific quarter of a calendar year.

    Args:
        year (int): Year of the quarter period being created.
        quarter (int): Quarter of year (from 1 to 4) of quarter period being created.

    Returns:
        pandas.Period: Period with freqstr 'Q' representing calendar quarter as specified by the month and quarter parameters.
    """
    return pd.Period(year=year, quarter=quarter, freq='Q')


def q_1(year):
    """
    Creates an instance of pandas.Period representing the first quarter of a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'Q' representing the first quarter of the calendar year specified by the year parameter.
    """
    return pd.Period(year=year, quarter=1, freq='Q')


def q_2(year):
    """
    Creates an instance of pandas.Period representing the second quarter of a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'Q' representing the second quarter of the calendar year specified by the year parameter.
    """
    return pd.Period(year=year, quarter=2, freq='Q')


def q_3(year):
    """
    Creates an instance of pandas.Period representing the third quarter of a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'Q' representing the third quarter of the calendar year specified by the year parameter.
    """
    return pd.Period(year=year, quarter=3, freq='Q')


def q_4(year):
    """
    Creates an instance of pandas.Period representing the fourth quarter of a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'Q' representing the fourth quarter of the calendar year specified by the year parameter.
    """
    return pd.Period(year=year, quarter=4, freq='Q')


def summer(year):
    """
    Creates an instance of pandas.Period representing the energy forward definition of summer (2nd and 3rd quarter) of a specific year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr '2Q' representing the period spanning the 2nd and 3rd quarters of the calendar year specified by the year parameter.
    """
    return pd.Period(year=year, quarter=2, freq='2Q')


def winter(year):
    """
    Creates an instance of pandas.Period representing the energy forward definition of winter (4th and proceeding 1st quarter) of a specific year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr '2Q' representing the period spanning the 4th and proceeding 1st quarters, starting in the calendar 
            year specified by the year parameter.
    """
    return pd.Period(year=year, quarter=4, freq='2Q')


def cal_year(year):
    """
    Creates an instance of pandas.Period representing a specific calendar year.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        pandas.Period: Period with freqstr 'A' representing the calendar year specified by the year parameter.
    """
    return pd.Period(year=year, freq='A')


def gas_year(year):
    """
    Creates an tuple representing an energy forward definition of a gas year, that being the year starting on the 1st of October.

    Args:
        year (int): Year of the Period object being created.

    Returns:
        (date, date): Tuple of date instances representing the gas year specified by the year parameter, where each tuple element is:
            The first element will be the date for the 1st of October of the year specified by the year parameter.
            The second element will be the date for the 30th of October of the proceeding year specified by the year parameter.
    """
    return (date(year, 10, 1), date(year+1, 9, 30))

