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

import unittest
from datetime import date
from curves import shape_preserving_average_spline


class TestShapePreservingSpline(unittest.TestCase):

    def test_shape_preserving_spline_averages_back_to_inputs(self):
        contracts = [
            (date(2019, 1, 2), date(2019, 1, 2), 32.7),
            (date(2019, 1, 3), date(2019, 1, 7), 29.3),
            (date(2019, 1, 8), date(2019, 1, 31), 24.66)
        ]

        shape_preserved = shape_preserving_average_spline(contracts, freq='D')

        self.assertNotEqual(0, len(shape_preserved))

        print(shape_preserved)

        for start, end, price in contracts:
            averaged_price = shape_preserved[start:end].mean()
            print(averaged_price)

        for start, end, price in contracts:
            averaged_price = shape_preserved[start:end].mean()
            self.assertAlmostEqual(averaged_price, price, delta=1E-8)

    def test_shape_preserving_spline_averages_back_to_inputs_2(self):
        contracts = [
            (date(2019, 1, 2), date(2019, 1, 2), 32.7),
            (date(2019, 1, 3), date(2019, 1, 7), 29.3),
            (date(2019, 1, 8), date(2019, 1, 31), 24.66),
            (date(2019, 9, 1), date(2019, 9, 30), 25.66),
            (date(2019, 10, 1), date(2019, 10, 31), 24.66),
            (date(2019, 11, 1), date(2019, 11, 30), 25.66),
            (date(2019, 12, 1), date(2019, 12, 31), 27.66),
        ]

        shape_preserved = shape_preserving_average_spline(contracts, freq='D')

        self.assertNotEqual(0, len(shape_preserved))

        print('shape_preserved')
        print(shape_preserved)

        for start, end, price in contracts:
            averaged_price = shape_preserved[start:end].mean()
            print(averaged_price)

        for start, end, price in contracts:
            averaged_price = shape_preserved[start:end].mean()
            self.assertAlmostEqual(averaged_price, price, delta=1E-6)