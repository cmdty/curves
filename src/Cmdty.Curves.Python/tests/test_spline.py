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
from datetime import date, datetime
from curves import max_smooth_interp, FREQ_TO_PERIOD_TYPE
from curves._common import deconstruct_contract
from curves.contract_period import quarter, winter, summer, gas_year
from tests._test_common import weighted_average_slice_curve

class TestSpline(unittest.TestCase):
    
    contracts = [
            (date(2019, 1, 1), 32.7), # date
            ((date(2019, 1, 2), date(2019, 1, 2)), 32.7),
            ((date(2019, 1, 3), date(2019, 1, 7)), 29.3),
            ((date(2019, 1, 8), date(2019, 1, 31)), 24.66),
            (quarter(year=2019, quarter=2), 18.3),
            (quarter(year=2019, quarter=3), 17.1),
            (winter(2019), 22.4),
            (summer(2020), 19.9),
            (gas_year(2020), 20.01)
        ]
       
    test_case_data = [
                        {
                            "freq" : 'D',
                            "contracts" : contracts,
                            "mult_season_adjust" : lambda x: 1.0,
                            "add_season_adjust" : lambda x: 0.1,
                            "time_func" : lambda period1, period2: (period2 - period1).delta.days,
                            "average_weight" : lambda x: 0.1,
                            "front_1st_deriv" : 0.0,
                            "back_1st_deriv" : 0.0,
                        }, 
                        {
                            "freq" : 'D',
                            "contracts" : contracts,
                        },
                        {
                            "freq" : "30min",
                            "contracts" : [
                                                (datetime(2019, 6, 7, hour=7, minute=0), datetime(2019, 6, 7, hour=8, minute=30), 56.84),
                                                (datetime(2019, 6, 7, hour=9, minute=0), datetime(2019, 6, 7, hour=18, minute=0), 57.05),
                                                (datetime(2019, 6, 7, hour=18, minute=30), datetime(2019, 6, 7, hour=23, minute=30), 60.11),
                                           ],
                        },
                        {
                            "freq" : "15min",
                            "contracts" : [
                                                (datetime(2019, 6, 7, hour=7, minute=0), datetime(2019, 6, 7, hour=8, minute=45), 56.84),
                                                (datetime(2019, 6, 7, hour=9, minute=0), datetime(2019, 6, 7, hour=18, minute=0), 57.05),
                                                (datetime(2019, 6, 7, hour=18, minute=15), datetime(2019, 6, 7, hour=23, minute=45), 60.11),
                                           ],
                        }
                         # TODO add more test cases
                    ]
            
    # TODO use better seasonal adjustments

    # TODO properly parameterise these tests
    def test_max_smooth_interp_averages_back_to_inputs(self):
        for test_data in self.test_case_data:
            interp_curve = max_smooth_interp(**test_data)
            average_weight = test_data['average_weight'] if 'average_weight' in test_data else lambda x: 1.0
            for contract in test_data['contracts']:
                (period, contract_price) = deconstruct_contract(contract)
                curve_average_price = weighted_average_slice_curve(interp_curve, test_data['freq'], period, average_weight)
                self.assertAlmostEqual(curve_average_price, contract_price, delta=1E-10)

    daily_contracts = [
                        (pd.Period('2019-5-14', freq='D'), 15.2),
                        ((pd.Period('2019-5-15', freq='D'), pd.Period('2019-5-16', freq='d')), 14.05),
                        ((pd.Period('2019-5-17', freq='D'), pd.Period('2019-5-18', freq='d')), 13.18),
                        ]

    def test_max_smooth_interp_mult_season_adjust_called_as_expected(self):

        adjust_arg_values = []

        def mult_season_adjust(period):
            adjust_arg_values.append(period)
            return 1.0

        curve = max_smooth_interp(self.daily_contracts, freq='D', mult_season_adjust=mult_season_adjust)

        expected_first_arg = pd.Period('2019-5-14', freq='D')
        expected_arg_values = [expected_first_arg + i for i in range(0, 5)] * 2

        self.assertListEqual(expected_arg_values, adjust_arg_values)


    def test_max_smooth_interp_add_season_adjust_called_as_expected(self):

        adjust_arg_values = []

        def add_season_adjust(period):
            adjust_arg_values.append(period)
            return 1.0

        curve = max_smooth_interp(self.daily_contracts, freq='D', add_season_adjust=add_season_adjust)

        expected_first_arg = pd.Period('2019-5-14', freq='D')
        expected_arg_values = [expected_first_arg + i for i in range(0, 5)] * 2

        self.assertListEqual(expected_arg_values, adjust_arg_values)


    def test_max_smooth_interp_average_weight_called_as_expected(self):

        weight_arg_values = []

        def average_weight(period):
            weight_arg_values.append(period)
            return 1.0

        curve = max_smooth_interp(self.daily_contracts, freq='D', average_weight=average_weight)

        expected_first_arg = pd.Period('2019-5-14', freq='D')
        expected_arg_values = [expected_first_arg + i for i in range(0, 5)]

        self.assertListEqual(expected_arg_values, weight_arg_values)


if __name__ == '__main__':
    unittest.main()
