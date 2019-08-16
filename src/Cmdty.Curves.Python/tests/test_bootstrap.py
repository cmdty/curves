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
from datetime import date, datetime
from curves.contract_period import month, quarter, winter, summer, gas_year
from curves import bootstrap_contracts, weighting
from curves._common import deconstruct_contract
import pandas as pd
from tests._test_common import weighted_average_slice_curve
import clr
from System import ArgumentException


class TestBootstrap(unittest.TestCase):
            
    def test_bootstrap_contracts_averages_back_to_inputs_daily(self):

        contracts = [
                ((date(2019, 1, 1), date(2019, 1, 2)), 32.7), # manually specified contract period
                ((date(2019, 1, 1), date(2019, 1, 7)), 29.3), # manually specified contract period as nested tuple
                (quarter(2019, 1), 22.1), 
                (datetime(2019, 1, 1), datetime(2019, 1, 31), 25.5), # January 2019
                (month(2019, 2), 23.3),
                (quarter(2019, 2), 18.3),
                (quarter(2019, 3), 17.1),
                (quarter(2019, 4), 20.1),
                (winter(2019), 22.4),
                (summer(2020), 19.9),
                (winter(2020), 21.8),
                (gas_year(2020), 20.01)
            ]

        ratios = [
                    (quarter(2021, 1), quarter(2020, 4), 1.09),
                    (quarter(2020, 3), quarter(2020, 2), 1.005)
                ]

        spreads = [
                    (month(2020, 1), month(2020, 2), 0.5),
                ]

        piecewise_curve, bootstrapped_contracts = bootstrap_contracts(contracts, freq='D', shaping_ratios=ratios, shaping_spreads=spreads, average_weight=lambda t:1.0)
        
        self.assertEqual(16, len(bootstrapped_contracts))
        self.assertEqual(1004, len(piecewise_curve))

        for contract in contracts:
            (period, contract_price) = deconstruct_contract(contract)
            curve_average_price = weighted_average_slice_curve(piecewise_curve, 'D', period)
            self.assertAlmostEqual(curve_average_price, contract_price, delta=1E-10)

            
    def test_bootstrap_contracts_averages_back_to_inputs_monthly(self):

        contracts = [
                        (month(2019, 1), 12.35),
                        (month(2019, 2), 13.20),
                        (quarter(2019, 1), 12.85)
                    ]

        piecewise_curve, bootstrapped_contracts = bootstrap_contracts(contracts, freq='M')

        for contract in contracts:
            (period, contract_price) = deconstruct_contract(contract)
            curve_average_price = weighted_average_slice_curve(piecewise_curve, 'D', period)
            self.assertAlmostEqual(curve_average_price, contract_price, delta=1E-10)


    def test_bootstrap_contracts_averages_back_to_inputs_halfhourly(self):

        contracts = [
                (pd.Period('2019-06-07 00:00', freq='30min'), 47.854),
                (datetime(2019, 6, 7, hour=0, minute=30), 47.705),
                (datetime(2019, 6, 7, hour=0, minute=30), datetime(2019, 6, 7, hour=1, minute=30), 46.625),
            ]

        piecewise_curve, bootstrapped_contracts = bootstrap_contracts(contracts, freq='30min')

        for contract in contracts:
            (period, contract_price) = deconstruct_contract(contract)
            curve_average_price = weighted_average_slice_curve(piecewise_curve, '30min', period)
            self.assertAlmostEqual(curve_average_price, contract_price, delta=1E-10)


    def test_bootstrap_contracts_averages_back_to_inputs_quarterhourly(self):

        contracts = [
                (pd.Period('2019-06-07 00:00', freq='15min'), 47.854),
                (datetime(2019, 6, 7, hour=0, minute=30), 47.705),
                (datetime(2019, 6, 7, hour=0, minute=0), datetime(2019, 6, 7, hour=1, minute=45), 46.625),
            ]

        piecewise_curve, bootstrapped_contracts = bootstrap_contracts(contracts, freq='15min')

        for contract in contracts:
            (period, contract_price) = deconstruct_contract(contract)
            curve_average_price = weighted_average_slice_curve(piecewise_curve, '15min', period)
            self.assertAlmostEqual(curve_average_price, contract_price, delta=1E-10)


    def test_average_weight_called_as_expected(self):

        contracts = [
                        (month(2019, 1), 58.64),
                        (month(2019, 2), 58.64),
                        (month(2019, 3), 58.64)
                    ]

        weight_arg_values = []

        def average_weight(period):
            weight_arg_values.append(period)
            return 1.0

        bootstrap_results = bootstrap_contracts(contracts, freq='D', average_weight=average_weight)

        expected_first_arg = pd.Period(year=2019, month=1, day=1, freq='D')
        expected_arg_values = [expected_first_arg + i for i in range(0, 90)] * 2

        self.assertListEqual(expected_arg_values, weight_arg_values)


    def test_error_raised_when_redundant_contracts_allow_redundancy_default_false(self):
        
        contracts = [
                (month(2019, 1), 68.64),
                (month(2019, 2), 59.01),
                (month(2019, 3), 55.48),
                (quarter(2019, 1), 62.64),
            ]

        with self.assertRaises(ArgumentException): # TODO assert against exception message
            bootstrap_results = bootstrap_contracts(contracts, freq='M')


    def test_error_not_raised_when_redundant_contracts_allow_redundancy_default_false(self):
        
        contracts = [
                (month(2019, 1), 68.64),
                (month(2019, 2), 59.01),
                (month(2019, 3), 55.48),
                (quarter(2019, 1), 62.64),
            ]

        piecewise_curve, bootstrapped_contracts = bootstrap_contracts(contracts, freq='M', allow_redundancy=True)

        self.assertEqual(len(piecewise_curve), 3)
        self.assertEqual(len(bootstrapped_contracts), 3)



if __name__ == '__main__':
    unittest.main()
