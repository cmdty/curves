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

import unittest
import pandas as pd
from curves import weighting
from datetime import date

class TestWeighting(unittest.TestCase):

    def test_num_business_days(self):
        holidays = [date(2019, 5, 6), date(2019, 5, 27)]
        may19 = pd.Period(year=2019, month=5, freq='M')
        may19_business_days = weighting.num_business_days(holidays)(may19)
        self.assertEqual(21, may19_business_days)

    def test_num_num_weekdays(self):
        may19 = pd.Period(year=2019, month=5, freq='M')
        may19_weekdays = weighting.num_weekdays()(may19)
        self.assertEqual(23, may19_weekdays)

    def test_num_periods_equals_23_hours_for_clock_change_forward_day(self):
        hours_count = weighting.num_periods(freq='H', tz='Europe/London')
        day = pd.Period('2019-03-31 00:00', freq='D')
        num_hours = hours_count(day)
        self.assertEqual(23, num_hours)

    def test_num_periods_equals_46_half_hours_for_clock_change_forward_day(self):
        half_hours_count = weighting.num_periods(freq='30min', tz='Europe/London')
        day = pd.Period('2019-03-31 00:00', freq='D')
        num_half_hours = half_hours_count(day)
        self.assertEqual(46, num_half_hours)

    def test_num_periods_equals_24_hours_for_tz_none(self):
        hours_count = weighting.num_periods(freq='H')
        day = pd.Period('2019-03-31 00:00', freq='D')
        num_hours = hours_count(day)
        self.assertEqual(24, num_hours)

    def test_num_periods_equals_48_half_hours_for_tz_none(self):
        half_hours_count = weighting.num_periods(freq='30min')
        day = pd.Period('2019-03-31 00:00', freq='D')
        num_half_hours = half_hours_count(day)
        self.assertEqual(48, num_half_hours)

    def test_num_periods_equals_25_hours_for_clock_change_back_day(self):
        hours_count = weighting.num_periods(freq='H', tz='Europe/London')
        day = pd.Period('2019-10-27 00:00', freq='D')
        num_hours = hours_count(day)
        self.assertEqual(25, num_hours)

    def test_num_periods_equals_50_half_hours_for_clock_change_back_day(self):
        half_hours_count = weighting.num_periods(freq='30min', tz='Europe/London')
        day = pd.Period('2019-10-27 00:00', freq='D')
        num_half_hours = half_hours_count(day)
        self.assertEqual(50, num_half_hours)

