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

""" Provides functions to use as the mult_season_adjust and add_season_adjust parameters to curve construction functions."""

# TODO include one-off dates for holidays
def dayofweek(default, monday=None, tuesday=None, wednesday=None, thursday=None, friday=None, saturday=None, sunday=None):
    """
    Creates a function which returns a float based on the day of week of it's parameter. 

    Args:
        default (float): The default value returned by the returned function.
        monday (float, optional): The value that the returned function returns when it's parameter represents a Monday.
        tuesday (float, optional): The value that the returned function returns when it's parameter represents a Tuesday.
        wednesday (float, optional): The value that the returned function returns when it's parameter represents a Wednesday.
        thursday (float, optional): The value that the returned function returns when it's parameter represents a Thursday.
        friday (float, optional): The value that the returned function returns when it's parameter represents a Friday.
        saturday (float, optional): The value that the returned function returns when it's parameter represents a Saturday.
        sunday (float, optional): The value that the returned function returns when it's parameter represents a Sunday.

    Returns:
        callable: Callable accepting a single parameter of type pandas.Period and returning a float. The value 
            that the returned function will return depends on the dayofweek attribute of the parameter. If any of the
            parameters monday/tuesday/wednesday etc have been provided, then the value of the parameter which 
            matches the Period day of week will be returned. Otherwise the value provided as the default
            paramter will be returned.
    """
    adjust_dict = {}

    _populate_dict(adjust_dict, monday, 0)
    _populate_dict(adjust_dict, tuesday, 1)
    _populate_dict(adjust_dict, wednesday, 2)
    _populate_dict(adjust_dict, thursday, 3)
    _populate_dict(adjust_dict, friday, 4)
    _populate_dict(adjust_dict, saturday, 5)
    _populate_dict(adjust_dict, sunday, 6)

    def dayofweekadjust(period):
        return adjust_dict.get(period.dayofweek, default)

    return dayofweekadjust


def _populate_dict(dict_to_populate, arg, dict_index):
    if arg is not None:
        dict_to_populate[dict_index] = arg

