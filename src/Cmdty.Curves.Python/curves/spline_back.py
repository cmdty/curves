import math
import scipy as sp
import numpy as np


def delta_pow(time_to_start, time_to_end, power):
    return math.pow(time_to_end, power) - math.pow(time_to_start, power)


def create_2h_bottom_right_submatrix(contract, curve_start, time_func):
    "TODO: contract is a Contract object, time_func is a func of {T,T, double}"
    timeToStart = time_func(curve_start, contract.start)
    timeToEnd = time_func(curve_start, contract.end.Offset(1))

    subMatrix = np.empty(3, 3) #is this dense?

    deltaPow2 = delta_pow(timeToStart, timeToEnd, 2.0)
    deltaPow3 = delta_pow(timeToStart, timeToEnd, 3.0)
    deltaPow4 = delta_pow(timeToStart, timeToEnd, 4.0)

    subMatrix[0, 0] = 8.0 * (timeToEnd - timeToStart)
    subMatrix[0, 1] = 12.0 * deltaPow2
    subMatrix[0, 2] = 16.0 * deltaPow3

    subMatrix[1, 0] = 12.0 * deltaPow2
    subMatrix[1, 1] = 24.0 * deltaPow3
    subMatrix[1, 2] = 36.0 * deltaPow4

    subMatrix[2, 0] = 16.0 * deltaPow3
    subMatrix[2, 1] = 36.0 * deltaPow4
    subMatrix[2, 2] = 57.6 * delta_pow(timeToStart, timeToEnd, 5.0)

    return subMatrix


def build(contracts, weighting, mult_adjust_func, add_adjust_func, 
          time_func, front_first_derivative = None, back_first_derivative = None):
    #TODO: contracts = list(contract objects), weighting = Func<T, dbl>
    if contracts.count < 2:
        raise ValueError("contracts must have at least two elements", nameof(contracts))

    curveStartPeriod = contracts[0].Start
    numGaps = 0
    timeToPolynomialBoundaries = []

    #TODO: optionally do/don't allow gaps in contracts
    


    return ""


