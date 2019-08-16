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
from curves import adjustments
import pandas as pd

class TestAdjustments(unittest.TestCase):

    def test_dayofweek_values_specified(self):

        dayofweek_adjust = adjustments.dayofweek(0.5, monday=3.4, tuesday=1.2, wednesday=1.1, 
                                                 thursday=0.9, friday=0.7, saturday=0.2, sunday=0.1)

        self.assertEqual(3.4, dayofweek_adjust(pd.Period('2019-05-13', freq='D'))) # monday
        self.assertEqual(1.2, dayofweek_adjust(pd.Period('2019-05-14', freq='D'))) # tuesday
        self.assertEqual(1.1, dayofweek_adjust(pd.Period('2019-05-15', freq='D'))) # wednesday
        self.assertEqual(0.9, dayofweek_adjust(pd.Period('2019-05-16', freq='D'))) # thursday
        self.assertEqual(0.7, dayofweek_adjust(pd.Period('2019-05-17', freq='D'))) # friday
        self.assertEqual(0.2, dayofweek_adjust(pd.Period('2019-05-18', freq='D'))) # saturday
        self.assertEqual(0.1, dayofweek_adjust(pd.Period('2019-05-19', freq='D'))) # sunday

    def test_dayofweek_value_not_specified_default_returned(self):

        default_value = 0.5

        dayofweek_adjust = adjustments.dayofweek(default_value)

        self.assertEqual(default_value, dayofweek_adjust(pd.Period('2019-05-13', freq='D'))) # monday
        self.assertEqual(default_value, dayofweek_adjust(pd.Period('2019-05-14', freq='D'))) # tuesday
        self.assertEqual(default_value, dayofweek_adjust(pd.Period('2019-05-15', freq='D'))) # wednesday
        self.assertEqual(default_value, dayofweek_adjust(pd.Period('2019-05-16', freq='D'))) # thursday
        self.assertEqual(default_value, dayofweek_adjust(pd.Period('2019-05-17', freq='D'))) # friday
        self.assertEqual(default_value, dayofweek_adjust(pd.Period('2019-05-18', freq='D'))) # saturday
        self.assertEqual(default_value, dayofweek_adjust(pd.Period('2019-05-19', freq='D'))) # sunday


if __name__ == '__main__':
    unittest.main()
