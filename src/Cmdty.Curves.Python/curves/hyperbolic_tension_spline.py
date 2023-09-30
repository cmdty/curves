# Copyright(c) 2023 Jake Fowler
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
import numpy as np
import typing as tp
from curves._common import ContractsType, _last_period, deconstruct_contract, contract_pandas_periods, ShapingTypes
from datetime import date, datetime
from enum import Flag, auto


class KnotPositions(Flag):
    CONTRACT_START = auto()
    CONTRACT_END = auto()
    CONTRACT_START_AND_END = CONTRACT_START | CONTRACT_END
    CONTRACT_CENTRE = auto()
    SPACING_CENTRE = auto()


_years_per_second = 1.0 / 60.0 / 60.0 / 24.0 / 365.0


# Update type hints to include str for contract periods
def hyperbolic_tension_spline(contracts: tp.Union[ContractsType, pd.Series],
                              freq: str,
                              tension: tp.Union[tp.Callable[[tp.Union[pd.Period, pd.Timestamp]], float], float],
                              discount_factor: tp.Optional[tp.Callable[[tp.Union[pd.Period, pd.Timestamp]], float]] = None,
                              average_weight: tp.Optional[tp.Callable[[tp.Union[pd.Period, pd.Timestamp]], float]] = None,
                              mult_season_adjust: tp.Optional[tp.Callable[[tp.Union[pd.Period, pd.Timestamp]], float]] = None,
                              add_season_adjust: tp.Optional[tp.Callable[[tp.Union[pd.Period, pd.Timestamp]], float]] = None,
                              shaping_ratios: tp.Optional[ShapingTypes] = None,
                              shaping_spreads: tp.Optional[ShapingTypes] = None,
                              time_zone: tp.Optional[tp.Union[str, tp.Type['pytz.timezone'], tp.Type['dateutil.tz.tzfile']]] = None,
                              # TODO test that pytz.timezone and dateutil.tz.tzfile type hints work as expected
                              knots: tp.Optional[tp.Union[tp.Iterable[tp.Union[str, pd.Period, pd.Timestamp, date, datetime]],
                              KnotPositions]] = KnotPositions.CONTRACT_START_AND_END,  # TODO update docstring for KnotPositions enum
                              front_1st_deriv: tp.Optional[float] = None,
                              back_1st_deriv: tp.Optional[float] = None,
                              return_spline_coeff: tp.Optional[bool] = False,
                              out_curve: tp.Optional[pd.Series] = None
                              ) -> tp.Union[pd.Series, tp.Tuple[pd.Series, pd.DataFrame]]:
    """
    Creates a smooth interpolated curve from a collection of commodity forward/swap/futures prices using hyperbolic tension spline algorithm.

    Generally this function is used to increase the granularity of a curve from a collection of forward prices.
    The resulting curve will be smooth, and of homogenous granularity. For example a smooth monthly curve can be
    created from a collection of monthly, quarterly and season granularity forward market prices. This algorithm also
    handles input contracts with overlapping delivery periods, i.e. the bootstrapping step of forward curve construction.

    Args:
        contracts (pd.Series or iterable): The input contracts to be interpolated.
            If iterable of tuples, with each tuple describing a forward delivery period and price in one
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
                str
                pandas.Period
                pandas.Timestamp
                date
                datetime
            The delivery periods of all input contracts can be contiguous, overlapping or have gaps. If there are
            overlaps then the spline_knots argument must be provided.
        freq (str): Describes the granularity of curve being constructed using pandas Offset Alias notation.
        tension (float or callable): parameter which specifies the "tension" TODO complete this
            A higher tension value makes the curve less smooth, and closer to piecewise linear function.
        discount_factor (callable, optional): Callable which maps from delivery period to the discount factor for the settlement
            date of this period. This function will be called with an argument of either pandas Period or Timestamp to
            describe the delivery period, at the granularity of the interpolated curve. The discount factor is necessary
            as a no-arbitrage forward price should equal weighted average of it's constituent delivery period forward prices,
            with the weighting being the product of the discount factor and the delivery volume. If omitted, defaults to None, in
            which case a discount factor of 1.0 will be used for all delivery periods, as is suitable for futures.
        average_weight (callable, optional): Mapping from pandas Period or Timestamp instances to float which describes the weighting
            that each forward period contributes to a price for delivery which spans multiple periods. The
            parameter will be at the granularity of the interpolated curve, as specified by the freq argument. An example of such weighting is
            a monthly curve (freq='M') of a commodity which delivers on every calendar day. In this example average_weight would be
            a callable which returns the number of calendar days in the month, e.g.:
                lambda p: p.asfreq('D', 'e').day
            Defaults to None, in which case each period has equal weighting.
        mult_season_adjust (callable, optional): Callable with single parameter of type pandas Period or Timestamp and return type float.
            If this argument is supplied, the value from the underlying spline function is multiplied by the result of mult_season_adjust,
            evaluated for each index period in the resulting curve.
        add_season_adjust (callable, optional): Callable with single parameter of type pandas Period or Timestamp and return type float.
            If this argument is supplied, the value from the underlying spline function has the result of add_season_adjust,
            evaluated for each index period, added to it to derive each price in the resulting curve.
        shaping_ratios (iterable, optional): iterable of tuples, with each tuple describing a constraint on the ratio
            between the prices of different periods on the derived forward curve in the form:
                ([numerator period], [denominator period], [ratio])
            Where:
                [numerator period] is the delivery period corresponding to the numerator part of the ratio.
                [denominator period] is the delivery period corresponding to the denomintor part of the ratio.
                [ratio] (float) is the ratio between the forward prices.
            [numerator period] and [denominator period] can be any of the following types:
                str
                pandas.Period
                date
                datetime
                A 2-tuple of any of the above three types, with the elements specifying the period start and end respectively.
        shaping_spreads (iterable, optional): iterable of tuples, with each tuple describing a constraint on the spread
            between the prices of different periods on the derived forward curve in the form:
                ([period long], [period short], [spread])
            Where:
                [period long] is the delivery period for the long part of the spread.
                [period short] is the delivery period for the short part of the spread.
                [spread] (float) is the spread between the forward prices.
            [period long] and [period short] can be any of the following types:
                str
                pandas.Period
                date
                datetime
                A 2-tuple of any of the above three types, with the elements specifying the period start and end respectively.
        time_zone (str, pytz.timezone or dateutil.tz.tzfile, optional): Time zone applicable for the delivery periods of
            the interpolated curve. This should be specified if interpolating to higher than daily granularity (e.g. hourly)
            as time zone information is necessary to determine lost or gain hours due to clock changes. If omitted,
            defaults to UTC-like behaviour with no clock changes. It is not advisable to specify time_zone if
            interpolating to daily or lower granularity.
        knots (iterable, KnotPositions, optional): If an iterable, the internal knots of the spline constructed,
            i.e. the points in time which serve as boundaries between the piecewise hyperbolic functions.
            Can also be an instance of the KnotPositions flag enum, which provides a convenient way of specifying knots relative
            to the boundaries of the input contracts. If omitted, defaults to KnotPositions.CONTRACT_START_AND_END, in which case the
            start period and end period + 1 of each input contract are used as the internal knots.
        front_1st_deriv (float, optional): Constraint specifying what the first derivative of the spline at the very start of the
            curve must be. If this parameter is omitted no constraint is applied.
        back_1st_deriv (float, optional): Constraint specifying what the first derivative of the spline at the end of the
            curve must be. If this parameter is omitted no constraint is applied.
        return_spline_coeff (bool, optional): Flag to determine whether the solved spline coefficients should be returned as the second
            element in a 2-tuple. Defaults to False if omitted.
        out_curve (pandas.Series, optional): A Series into which the smoothed contiguous curve is written.
            This argument can be used by the called to control the allocation of heap memory used to place the calculated curve.

    Returns:
        Either pandas.Series, or 2-tuple of (pandas.Series, pandas.DataFrame) if return_spline_coeff argument is True.
        In either case the pandas.Series is a smooth contiguous curve with values consistent with prices within the contracts parameter.
        This pandas.Series will have an index of type PeriodIndex or DatetimeIndex (if caller provides time_zone)
        and freq equal to the freq parameter.
        If return_spline_coeff is True a 2-tuple will be returned with the 2nd element being a pandas.DataFrame containing
        the solved spline coefficients solved z_i and y_i at each spline knot.

    Notes:
        Whether time_zone is provided by the caller determines whether pandas Period or Timestamp type is used to
        represent delivery periods in the returned data and as an argument when calling callable arguments. If time_zone is not specified
        then pandas.Period type is used, as instances of Period are not time zone aware type. If time_zone is specified pandas.Timestamp
        type is as used, with the tz property of instances being set accordingly.

        See the following technical document for full details of the tension spline algorithm:
            https://github.com/cmdty/curves/blob/master/docs/tension_spline/tension_spline.pdf
    """
    num_contracts = len(contracts)
    if num_contracts < 2:
        raise ValueError('contracts argument must have length at least 2. Length of contract used is {}.'
                         .format(num_contracts))

    standardised_contracts = []  # Contract as tuples of (Period or Timestamp, Period or Timestamp, price)
    if isinstance(contracts, pd.Series):
        for period, price in contracts.items():  # TODO check this works with Series of Timestamps
            start_period = _to_index_element(period.asfreq(freq, 's'), freq, time_zone)
            end_period = _to_index_element(_last_period(period, freq), freq, time_zone)
            standardised_contracts.append((start_period, end_period, price))
    else:
        for contract in contracts:
            period, price = deconstruct_contract(contract)
            start_period, end_period = contract_pandas_periods(period, freq)
            start_period = _to_index_element(start_period, freq, time_zone)
            end_period = _to_index_element(end_period, freq, time_zone)
            standardised_contracts.append((start_period, end_period, price))

    standardised_contracts = sorted(standardised_contracts, key=lambda x: x[0])  # Sort by start
    shaping_ratios_list = _standardise_shaping(shaping_ratios, freq, time_zone)
    shaping_spreads_list = _standardise_shaping(shaping_spreads, freq, time_zone)

    first_period = standardised_contracts[0][0]
    last_period = max((x[1] for x in standardised_contracts))
    freq_offset = pd.tseries.frequencies.to_offset(freq) # TODO find why Pycharm is warning about frequencies and fix

    starts_ends = {(contract[0], contract[1]) for contract in standardised_contracts} \
                  .union({(shaping_ratio[0], shaping_ratio[1]) for shaping_ratio in shaping_ratios_list}) \
                  .union({(shaping_ratio[2], shaping_ratio[3]) for shaping_ratio in shaping_ratios_list}) \
                  .union({(shaping_spread[0], shaping_spread[1]) for shaping_spread in shaping_spreads_list}) \
                  .union({(shaping_spread[2], shaping_spread[3]) for shaping_spread in shaping_spreads_list})

    # TODO this looks like it will break if latest contract is for a single period. Add test.
    if isinstance(knots, KnotPositions):
        spline_knots_set = set()  # Not worth adding dependency to Sorted Containers package
        if KnotPositions.CONTRACT_START in knots:
            for start, _ in starts_ends:
                spline_knots_set.add(start)
        if KnotPositions.CONTRACT_END in knots:
            for _, end in starts_ends:
                if end < last_period:
                    spline_knots_set.add(end + freq_offset)
        if KnotPositions.CONTRACT_CENTRE in knots:
            for start, end in starts_ends:
                mid_point = _mid_period_or_timestamp(start, end, freq_offset)
                spline_knots_set.add(mid_point)
        if KnotPositions.SPACING_CENTRE in knots:
            start_and_ends_set = ({start for start, _ in starts_ends}
                        .union({end + freq_offset for _, end in starts_ends}))
            sorted_start_and_ends_set = sorted(start_and_ends_set)
            for idx, p2 in enumerate(sorted_start_and_ends_set[1:]):
                p1 = sorted_start_and_ends_set[idx]
                mid_point = _mid_period_or_timestamp(p1, p2, freq_offset)
                spline_knots_set.add(mid_point)
        # Always include first and last period
        spline_knots_set.add(first_period)
        if last_period in spline_knots_set:
            spline_knots_set.remove(last_period)
        spline_knots_list = sorted(spline_knots_set)
    else:
        # TODO put condition on number of knots in maximum smoothness case
        spline_knots_list = [first_period] + [_to_index_element(sb, freq, time_zone) for sb in knots]
        for i in range(1, num_contracts):
            if spline_knots_list[i] < spline_knots_list[i - 1]:
                raise ValueError('spline_knots should be distinct and in ascending order. Elements {} and'
                                 '{} have values {} and {}, hence this is not true.'
                                 .format(i - 1, i, spline_knots_list[i - 1], spline_knots_list[i]))
            if spline_knots_list[i] > last_period:
                raise ValueError('spline_knots should not contain items after the latest contract delivery period.'
                                 'Element {} contains {} which is after the latest delivery of {}.'
                                 .format(i, spline_knots_list[i], last_period))

    if out_curve is not None:
        if out_curve.index.freqstr != freq:
            raise ValueError('out_curve argument must have frequency consistent with freq argument. However, out_curve.index.freqstr is'
                             ' {} and freq is {}.'.format(out_curve.index.freqstr, freq))

    if time_zone is None:
        result_curve_index = pd.period_range(start=first_period, end=last_period, freq=freq)
        datetime_index = pd.date_range(start=first_period.start_time, end=last_period.start_time,
                                       freq=freq)
        t_from_start = (datetime_index - first_period.start_time).total_seconds().to_numpy() * _years_per_second
        del datetime_index
    else:
        result_curve_index = pd.date_range(start=first_period, end=last_period,
                                           freq=freq, tz=time_zone)
        t_from_start = (result_curve_index - first_period).total_seconds().to_numpy() * _years_per_second
    num_result_curve_points = len(result_curve_index)

    # Calculate vectors of coefficients
    if discount_factor is None:
        discount_factors = np.ones(num_result_curve_points)
    else:
        discount_factors = np.fromiter((discount_factor(key) for key in result_curve_index), dtype=np.float64,
                                       count=num_result_curve_points)
    if average_weight is None:
        average_weights = np.ones(num_result_curve_points)
    else:
        average_weights = np.fromiter((average_weight(key) for key in result_curve_index), dtype=np.float64,
                                      count=num_result_curve_points)
    weights_times_discounts = discount_factors * average_weights
    if mult_season_adjust is None:
        mult_season_adjusts = np.ones(num_result_curve_points)
    else:
        mult_season_adjusts = np.fromiter((mult_season_adjust(key) for key in result_curve_index), dtype=np.float64,
                                          count=num_result_curve_points)
    if add_season_adjust is None:
        add_season_adjusts = np.zeros(num_result_curve_points)
    else:
        add_season_adjusts = np.fromiter((add_season_adjust(key) for key in result_curve_index), dtype=np.float64,
                                         count=num_result_curve_points)
    weights_x_discounts_x_mult_adjust = weights_times_discounts * mult_season_adjusts
    # Precalculate sinh vectors
    if isinstance(tension, float):  # TODO handle case if tension is int type?
        if tension <= 0:
            raise ValueError('tension argument should be a positive number, but value of {} has been provided.'
                             .format(tension))

        def get_tension(p) -> float:
            return tension
    else:
        def get_tension(p) -> float:
            tension_val = tension(p)
            if tension_val <= 0:
                raise ValueError('If callable, tension argument should always returns positive number, but value of {} '
                                 'has been returned for period {}.'
                                 .format(tension_val, p))
            return tension_val

    def int_index(del_period):
        return round((del_period - first_period) / freq_offset)

    num_sections = len(spline_knots_list)
    # Using np.zeros rather than empty because easier to understand when debugging
    h_is = np.zeros((num_sections,))
    tension_by_section = np.zeros((num_sections,))

    section_period_indices = []  # 2-tuples of indices for start and (exclusive) end indices of result periods for each section
    last_section_end_idx = 0
    # TODO vectorise below loop?
    for i, section_start in enumerate(spline_knots_list):
        # TODO should section_end be changed to last_period + 1?
        section_end = last_period if i == num_sections - 1 else spline_knots_list[i + 1]
        h_is[i] = _default_time_func(section_start, section_end)
        tension_by_section[i] = get_tension(section_start) / h_is[i]
        section_start_idx = last_section_end_idx
        section_end_idx = None if i == num_sections - 1 else int_index(spline_knots_list[i + 1])
        last_section_end_idx = section_end_idx
        section_period_indices.append((section_start_idx, section_end_idx))

    h_is_expanded = _create_expanded_np_array(h_is, num_result_curve_points, section_period_indices)
    section_end_times = np.cumsum(h_is)
    section_end_t_expanded = _create_expanded_np_array(section_end_times, num_result_curve_points, section_period_indices)
    t_to_section_end = section_end_t_expanded - t_from_start
    t_from_section_start = h_is_expanded - t_to_section_end

    tensions_expanded = _create_expanded_np_array(tension_by_section, num_result_curve_points, section_period_indices)

    tau_h = tension_by_section * h_is
    tau_sinh = np.sinh(tau_h) * tension_by_section
    tau_sqrd_sinh = tau_sinh * tension_by_section
    tension_by_section_sqrd = tension_by_section * tension_by_section
    tau_sqrd_hi = tension_by_section_sqrd * h_is
    cosh_tau_hi = np.cosh(tau_h)

    tau_sqrd_sinh_expanded = _create_expanded_np_array(tau_sqrd_sinh, num_result_curve_points, section_period_indices)
    tau_sqrd_hi_expanded = _create_expanded_np_array(tau_sqrd_hi, num_result_curve_points, section_period_indices)
    sinh_tau_t_from_start = np.sinh(t_from_section_start * tensions_expanded)
    sinh_tau_t_to_end = np.sinh(t_to_section_end * tensions_expanded)

    # TODO: research allocation-efficient vectorisation with numpy. Probably just make operations in-place.
    # Coefficients used in forward price constraint
    zi_coeffs = (sinh_tau_t_from_start / tau_sqrd_sinh_expanded - t_from_section_start / tau_sqrd_hi_expanded) \
                * weights_x_discounts_x_mult_adjust
    zi_minus1_coeffs = (sinh_tau_t_to_end / tau_sqrd_sinh_expanded - t_to_section_end / tau_sqrd_hi_expanded) \
                       * weights_x_discounts_x_mult_adjust
    yi_coeffs = (t_from_section_start / h_is_expanded) * weights_x_discounts_x_mult_adjust
    yi_minus1_coeffs = (t_to_section_end / h_is_expanded) * weights_x_discounts_x_mult_adjust

    num_coeffs_to_solve = num_sections * 2 + 2
    num_shaping_ratios = len(shaping_ratios_list)
    num_shaping_spreads = len(shaping_spreads_list)

    num_constraints = num_contracts + num_shaping_ratios + num_shaping_spreads + num_sections - 1 + \
                      (0 if back_1st_deriv is None else 1) \
                      + (0 if front_1st_deriv is None else 1)
    if num_constraints > num_coeffs_to_solve:
        raise ValueError('The number of constraints should be less than or equal to the number of coefficients to solve. However '
                         'num_constraints = {} and num_coeffs_to_solve = {}.'.format(num_constraints, num_coeffs_to_solve))
    maximum_smoothness = False if num_constraints == num_coeffs_to_solve else True
    if maximum_smoothness:
        matrix_size = num_coeffs_to_solve + num_constraints
        matrix = np.zeros((matrix_size, matrix_size))
        vector = np.zeros((matrix_size, 1))
        _populate_2h_matrix(matrix[:num_coeffs_to_solve, :num_coeffs_to_solve], tension_by_section, tension_by_section_sqrd, tau_sqrd_hi,
                            h_is, tau_sinh, cosh_tau_hi)
        constraint_matrix = matrix[num_coeffs_to_solve:, 0:num_coeffs_to_solve]
        constraint_vector = vector[num_coeffs_to_solve:]
        _populate_constraint_vector_matrix(constraint_matrix, constraint_vector, add_season_adjusts, front_1st_deriv, back_1st_deriv,
                                           cosh_tau_hi, freq_offset,
                                           h_is, int_index, last_period, num_contracts, num_sections, spline_knots_list, standardised_contracts,
                                           tau_sinh, tension_by_section, weights_times_discounts, weights_x_discounts_x_mult_adjust,
                                           yi_coeffs, yi_minus1_coeffs, zi_coeffs, zi_minus1_coeffs, shaping_ratios_list, shaping_spreads_list)
        matrix[0:num_coeffs_to_solve:, num_coeffs_to_solve:] = constraint_matrix.T
        # TODO use block matrix inversion with spare matrices
    else:
        # Not maximum smoothness, so constraint matrix has to be square
        matrix = np.zeros((num_coeffs_to_solve, num_coeffs_to_solve))  # TODO: make this banded matrix
        vector = np.zeros((num_coeffs_to_solve, 1))
        _populate_constraint_vector_matrix(matrix, vector, add_season_adjusts, front_1st_deriv, back_1st_deriv, cosh_tau_hi, freq_offset,
                                           h_is, int_index, last_period, num_contracts, num_sections, spline_knots_list, standardised_contracts,
                                           tau_sinh, tension_by_section, weights_times_discounts, weights_x_discounts_x_mult_adjust,
                                           yi_coeffs, yi_minus1_coeffs, zi_coeffs, zi_minus1_coeffs, shaping_ratios_list, shaping_spreads_list)

    solution = np.linalg.solve(matrix, vector)
    if maximum_smoothness:
        solution_to_use = solution[:num_coeffs_to_solve]
    else:
        solution_to_use = solution

    # Read results off solution
    spline_vals = np.zeros(num_result_curve_points)
    for i, section_start in enumerate(spline_knots_list):
        z_start = solution_to_use[i * 2, 0]
        y_start = solution_to_use[i * 2 + 1, 0]
        z_end = solution_to_use[i * 2 + 2, 0]
        y_end = solution_to_use[i * 2 + 3, 0]
        tension_squared = tension_by_section_sqrd[i]
        start_idx, end_idx = section_period_indices[i]
        spline_vals[start_idx:end_idx] = (z_start * sinh_tau_t_to_end[start_idx:end_idx] + z_end * sinh_tau_t_from_start[start_idx:end_idx]) \
                                         / tau_sqrd_sinh_expanded[start_idx:end_idx] + \
                                         ((y_start - z_start / tension_squared) * t_to_section_end[start_idx:end_idx] +
                                          (y_end - z_end / tension_squared) * t_from_section_start[start_idx:end_idx]) / h_is_expanded[
                                                                                                                         start_idx:end_idx]
    # TODO: handling of periods with zero weight, e.g. power offpeak hours when interpolating peak. Could be:
    # periods aren't included in index
    # NaN price for zero-weight periods
    # Current behaviour: zero price
    # Controls this behaviour with argument?
    # TODO: skip adjustments if these aren't provided
    result_curve_prices = (spline_vals + add_season_adjusts) * mult_season_adjusts
    result_curve = pd.Series(data=result_curve_prices, index=result_curve_index)
    if return_spline_coeff:
        spline_coeff_data = np.zeros(shape=(num_sections + 1, 4))
        spline_coeff_data[1:, 0] = section_end_times  # Knot times
        spline_coeff_data[:, 1] = solution_to_use[1::2, 0]  # y params
        spline_coeff_data[:, 2] = solution_to_use[::2, 0]  # z params
        spline_coeff_data[:-1, 3] = tension_by_section
        spline_coeff_data[-1, 3] = np.nan
        spline_coeffs = pd.DataFrame(data=spline_coeff_data, index=spline_knots_list + [last_period], columns=['t', 'y', 'z', 'tension'])
        return result_curve, spline_coeffs
    else:
        return result_curve


def _populate_constraint_vector_matrix(constraint_matrix, constraint_vector, add_season_adjusts, front_1st_deriv, back_1st_deriv,
                                       cosh_tau_hi, freq_offset, h_is, int_index, last_period, num_contracts, num_sections, spline_knots,
                                       standardised_contracts,
                                       tau_sinh, tension_by_section, weights_times_discounts, weights_x_discounts_x_mult_adjust, yi_coeffs,
                                       yi_minus1_coeffs, zi_coeffs, zi_minus1_coeffs, shaping_ratios, shaping_spreads):
    # Looking online it seems that Pandas index searching isn't particularly efficient, so do this manually
    for i, (start, end, price) in enumerate(standardised_contracts):
        contract_start_idx = int_index(start)
        contract_end_idx = int_index(end) + 1
        weights_times_discounts_slice = weights_times_discounts[contract_start_idx:contract_end_idx]
        add_season_adjusts_slice = add_season_adjusts[contract_start_idx:contract_end_idx]
        weights_x_discounts_x_mult_adjust_slice = weights_x_discounts_x_mult_adjust[contract_start_idx:contract_end_idx]
        constraint_vector[i] = price * np.sum(weights_times_discounts_slice) - \
                               np.dot(add_season_adjusts_slice, weights_x_discounts_x_mult_adjust_slice)
    # Forward price constraints
    # This is made more complicated by flexibility of allowing contracts and spline sections to differ
    first_spline_section_idx = 0
    # TODO tidy this up
    spline_knots_updated = spline_knots + [last_period + freq_offset]
    for contract_idx, (contract_start, contract_end, price) in enumerate(standardised_contracts):
        # Find first spline section
        # TODO use quicker search than this linear scan?
        while not (spline_knots_updated[first_spline_section_idx] <= contract_start < spline_knots_updated[first_spline_section_idx + 1]):
            first_spline_section_idx += 1
        # Loop through spline sections which overlap contract
        _populate_matrix_row(constraint_matrix, contract_end, contract_idx, contract_start, freq_offset, int_index,
                             last_period, num_sections, first_spline_section_idx, spline_knots, yi_coeffs, yi_minus1_coeffs,
                             zi_coeffs, zi_minus1_coeffs, 1.0)
    # Shaping constraints
    num_shaping_ratios = len(shaping_ratios)
    num_shaping_spreads = len(shaping_spreads)
    # Shaping spreads
    for idx, (long_period_start, long_period_end, short_period_start, short_period_end, spread) in enumerate(shaping_spreads):
        row_idx = idx + num_contracts
        long_add_season_adjust_vector_term, long_start_idx, long_end_idx = _calc_add_season_adjust_vector_term(
                                                                            add_season_adjusts, int_index, long_period_start,
                                                                            long_period_end, weights_x_discounts_x_mult_adjust)
        short_add_season_adjust_vector_term, short_start_idx, short_end_idx = _calc_add_season_adjust_vector_term(
                                                                            add_season_adjusts, int_index, short_period_start,
                                                                            short_period_end, weights_x_discounts_x_mult_adjust)
        constraint_vector[row_idx] = spread - long_add_season_adjust_vector_term + short_add_season_adjust_vector_term
        first_spline_section_idx = _find_first_spline_section_idx(long_period_start, spline_knots_updated)
        long_sum_weighting = np.sum(weights_times_discounts[long_start_idx:long_end_idx])
        _populate_matrix_row(constraint_matrix, long_period_end, row_idx, long_period_start, freq_offset, int_index,
                             last_period, num_sections, first_spline_section_idx, spline_knots, yi_coeffs, yi_minus1_coeffs,
                             zi_coeffs, zi_minus1_coeffs, 1.0/long_sum_weighting)
        first_spline_section_idx = _find_first_spline_section_idx(short_period_start, spline_knots_updated)
        short_sum_weighting = np.sum(weights_times_discounts[short_start_idx:short_end_idx])
        _populate_matrix_row(constraint_matrix, short_period_end, row_idx, short_period_start, freq_offset, int_index,
                             last_period, num_sections, first_spline_section_idx, spline_knots, yi_coeffs, yi_minus1_coeffs,
                             zi_coeffs, zi_minus1_coeffs, -1.0/short_sum_weighting)

    for idx, (num_period_start, num_period_end, denom_period_start, denom_period_end, ratio) in enumerate(shaping_ratios):
        row_idx = idx + num_contracts + num_shaping_spreads
        num_add_season_adjust_vector_term, num_start_idx, num_end_idx = _calc_add_season_adjust_vector_term(
                                                                            add_season_adjusts, int_index, num_period_start,
                                                                            num_period_end, weights_x_discounts_x_mult_adjust)
        denom_add_season_adjust_vector_term, denom_start_idx, denom_end_idx = _calc_add_season_adjust_vector_term(
                                                                            add_season_adjusts, int_index, denom_period_start,
                                                                            denom_period_end, weights_x_discounts_x_mult_adjust)
        constraint_vector[row_idx] = -num_add_season_adjust_vector_term + denom_add_season_adjust_vector_term * ratio
        first_spline_section_idx = _find_first_spline_section_idx(num_period_start, spline_knots_updated)
        _populate_matrix_row(constraint_matrix, num_period_end, row_idx, num_period_start, freq_offset, int_index,
                             last_period, num_sections, first_spline_section_idx, spline_knots, yi_coeffs, yi_minus1_coeffs,
                             zi_coeffs, zi_minus1_coeffs, 1.0)
        first_spline_section_idx = _find_first_spline_section_idx(denom_period_start, spline_knots_updated)
        num_sum_weighting = np.sum(weights_times_discounts[num_start_idx:num_end_idx])
        denom_sum_weighting = np.sum(weights_times_discounts[denom_start_idx:denom_end_idx])
        _populate_matrix_row(constraint_matrix, denom_period_end, row_idx, denom_period_start, freq_offset, int_index,
                             last_period, num_sections, first_spline_section_idx, spline_knots, yi_coeffs, yi_minus1_coeffs,
                             zi_coeffs, zi_minus1_coeffs, -ratio*num_sum_weighting/denom_sum_weighting)

    # First derivative continuity constraints
    one_over_h_tau_sqrd = 1.0 / (h_is * tension_by_section * tension_by_section)
    for section_idx in range(0, num_sections - 1):
        row_idx = num_contracts + num_shaping_spreads + num_shaping_ratios + section_idx
        # TODO doc using 1-based indexing for sections gets annoying now
        next_section_idx = section_idx + 1
        # TODO vectorise these outside of the loop?
        constraint_matrix[row_idx, section_idx * 2] -= (
                one_over_h_tau_sqrd[section_idx] - 1.0 / tau_sinh[section_idx])  # deriv_z_i_minus2_coff
        constraint_matrix[row_idx, section_idx * 2 + 1] += 1 / h_is[section_idx]  # deriv_y_i_minus2_coff
        constraint_matrix[row_idx, section_idx * 2 + 2] += one_over_h_tau_sqrd[next_section_idx] - cosh_tau_hi[next_section_idx] / \
                                                           tau_sinh[next_section_idx] - cosh_tau_hi[section_idx] / tau_sinh[section_idx] + \
                                                           one_over_h_tau_sqrd[section_idx]  # deriv_z_i_minus1_coff
        constraint_matrix[row_idx, section_idx * 2 + 3] -= (1 / h_is[next_section_idx] + 1 / h_is[section_idx])  # deriv_y_i_minus1_coff
        constraint_matrix[row_idx, section_idx * 2 + 4] += 1.0 / tau_sinh[next_section_idx] - one_over_h_tau_sqrd[
            next_section_idx]  # deriv_z_i_coff
        constraint_matrix[row_idx, section_idx * 2 + 5] += 1 / h_is[next_section_idx]  # deriv_y_i_coff

    if front_1st_deriv is not None:
        front_1st_deriv_idx = -1 if back_1st_deriv is None else -2
        constraint_matrix[front_1st_deriv_idx, 0] = one_over_h_tau_sqrd[0] - cosh_tau_hi[0] / tau_sinh[0]  # z_0
        constraint_matrix[front_1st_deriv_idx, 1] = -1.0 / h_is[0]  # y_0
        constraint_matrix[front_1st_deriv_idx, 2] = 1.0 / tau_sinh[0] - one_over_h_tau_sqrd[0]  # z_1
        constraint_matrix[front_1st_deriv_idx, 3] = 1.0 / h_is[0]  # y_1
        constraint_vector[front_1st_deriv_idx] = front_1st_deriv

    if back_1st_deriv is not None:
        constraint_matrix[-1, -4] = one_over_h_tau_sqrd[-1] - 1.0 / tau_sinh[-1]  # z_{n-1}
        constraint_matrix[-1, -3] = -1.0 / h_is[-1]  # y_{n-1}
        constraint_matrix[-1, -2] = cosh_tau_hi[-1] / tau_sinh[-1] - one_over_h_tau_sqrd[-1]  # z_n
        constraint_matrix[-1, -1] = 1.0 / h_is[-1]  # y_n
        constraint_vector[-1] = back_1st_deriv



def _calc_add_season_adjust_vector_term(add_season_adjusts, int_index, period_start, period_end,
                                        weights_x_discounts_x_mult_adjust):
    contract_start_idx = int_index(period_start)
    contract_end_idx = int_index(period_end) + 1
    add_season_adjusts_slice = add_season_adjusts[contract_start_idx:contract_end_idx]
    weights_x_discounts_x_mult_adjust_slice = weights_x_discounts_x_mult_adjust[contract_start_idx:contract_end_idx]
    add_season_adjust_vector_term = np.dot(add_season_adjusts_slice, weights_x_discounts_x_mult_adjust_slice)
    return add_season_adjust_vector_term, contract_start_idx, contract_end_idx


def _find_first_spline_section_idx(contract_start, spline_knots_updated):
    first_spline_section_idx = 0
    while not (spline_knots_updated[first_spline_section_idx] <= contract_start < spline_knots_updated[first_spline_section_idx + 1]):
        first_spline_section_idx += 1
    return first_spline_section_idx


def _populate_matrix_row(constraint_matrix, contract_end, row_idx, contract_start, freq_offset, int_index, last_period,
                         num_sections, spline_boundary_idx, spline_knots, yi_coeffs, yi_minus1_coeffs, zi_coeffs,
                         zi_minus1_coeffs, multiplier):
    while spline_boundary_idx < num_sections and spline_knots[spline_boundary_idx] <= contract_end:
        section_start = spline_knots[spline_boundary_idx]
        section_end = spline_knots[spline_boundary_idx + 1] - freq_offset if spline_boundary_idx < num_sections - 1 else last_period
        contract_section_start_period = contract_start if contract_start >= section_start else section_start
        contract_section_end_period = contract_end if contract_end <= section_end else section_end
        contract_section_start_idx = int_index(contract_section_start_period)
        contract_section_end_idx = int_index(contract_section_end_period) + 1
        # Forward price constraints
        constraint_matrix[row_idx, spline_boundary_idx * 2] += np.sum(
            zi_minus1_coeffs[contract_section_start_idx:contract_section_end_idx]) * multiplier
        constraint_matrix[row_idx, spline_boundary_idx * 2 + 1] += np.sum(
            yi_minus1_coeffs[contract_section_start_idx:contract_section_end_idx]) * multiplier
        constraint_matrix[row_idx, spline_boundary_idx * 2 + 2] += np.sum(
            zi_coeffs[contract_section_start_idx:contract_section_end_idx]) * multiplier
        constraint_matrix[row_idx, spline_boundary_idx * 2 + 3] += np.sum(
            yi_coeffs[contract_section_start_idx:contract_section_end_idx]) * multiplier
        spline_boundary_idx += 1


def _populate_2h_matrix(matrix: np.array, tension_by_section, tension_by_section_sqrd, tau_sqrd_hi, h_is, tau_sinh, cosh_tau_hi):
    two_tau_sqrd_over_hi = 2.0 * tension_by_section_sqrd / h_is  # y_i^2 and y_{i-1}^2 coeff
    one_over_tau_sqrd_hi = 1.0 / tau_sqrd_hi
    zi_zi_minus1_coeff = one_over_tau_sqrd_hi - 1.0 / tau_sinh
    zis_sqrd_coeff = 2.0 * (cosh_tau_hi / tau_sinh - one_over_tau_sqrd_hi)
    # TODO pass the below 2 variable in
    num_sections = len(tension_by_section)
    num_coeffs = num_sections * 2 + 2
    diagonal_elements = np.zeros(num_coeffs)
    diagonal_elements[:-2:2] = zis_sqrd_coeff
    diagonal_elements[2::2] += zis_sqrd_coeff
    diagonal_elements[1:-2:2] = two_tau_sqrd_over_hi
    diagonal_elements[3::2] += two_tau_sqrd_over_hi
    np.fill_diagonal(matrix, diagonal_elements)
    # TODO use matrix views to make this more efficient
    for i in range(0, num_sections):
        idx_lower = i * 2
        matrix[idx_lower, idx_lower + 2] = zi_zi_minus1_coeff[i]
        matrix[idx_lower + 2, idx_lower] = matrix[idx_lower, idx_lower + 2]
        matrix[idx_lower + 1, idx_lower + 3] = -two_tau_sqrd_over_hi[i]
        matrix[idx_lower + 3, idx_lower + 1] = matrix[idx_lower + 1, idx_lower + 3]


def _default_time_func(period1, period2):
    time_stamp1 = period1.start_time if isinstance(period1, pd.Period) else period1
    time_stamp2 = period2.start_time if isinstance(period2, pd.Period) else period2
    time_delta = time_stamp2 - time_stamp1
    return time_delta.total_seconds() * _years_per_second  # Convert to years with ACT/365


def _to_index_element(period, freq, tz) -> tp.Union[pd.Period, pd.Timestamp]:
    if tz is None:
        return pd.Period(period, freq=freq)
    else:
        if isinstance(period, pd.Period):
            return period.to_timestamp().tz_localize(tz)
        else:
            return pd.Timestamp(period, tz=tz)


def _create_expanded_np_array(array_from, size, copy_slice_indices):
    array_to = np.zeros((size,))
    for i in range(len(array_from)):
        slice_indices = copy_slice_indices[i]
        array_to[slice_indices[0]:slice_indices[1]] = array_from[i]
    return array_to


def _mid_period_or_timestamp(p1, p2, freq_offset):
    diff = p2 - p1
    num_periods = diff.n / freq_offset.n if isinstance(p1, pd.Period) else diff / freq_offset
    time_to_mid = int(num_periods / 2) * freq_offset
    return p1 + time_to_mid


def _standardise_shaping(shaping_info, freq, tz):
    shaping_info_list = []
    if shaping_info is not None:
        for (period1, period2, shaping_factor) in shaping_info:
            period1_start_period, period1_end_period = contract_pandas_periods(period1, freq)
            period2_start_period, period2_end_period = contract_pandas_periods(period2, freq)
            shaping_info_list.append((_to_index_element(period1_start_period, freq, tz), _to_index_element(period1_end_period, freq, tz),
                    _to_index_element(period2_start_period, freq, tz), _to_index_element(period2_end_period, freq, tz), shaping_factor))
    return shaping_info_list
