import pandas as pd
from datetime import date
from curves import bootstrap_contracts
from curves import max_smooth_interp
from curves import contract_period as cp

contracts = [
    (date(2019, 5, 31), 34.875),
    (date(2019, 6, 1), date(2019, 6, 2), 32.87),
    ((date(2019, 6, 3), date(2019, 6, 9)), 32.14),
    (pd.Period(year=2019, month=6, freq='M'), 31.08),
    (cp.month(2019, 7), 29.95),
    (cp.q_3(2019), 30.18),
    (cp.q_4(2019), 37.64),
    (cp.winter(2019), 38.05),
    (cp.summer(2020), 32.39),
    (cp.winter(2020), 37.84),
    (cp.gas_year(2020), 35.12)
]

pc_for_spline, bc_for_spline = bootstrap_contracts(contracts, freq='D')
smooth_curve = max_smooth_interp(bc_for_spline, freq='D')

pc_for_spline.plot(legend=True)
ax = smooth_curve.plot(legend=True)
ax.legend(["Piecewise Daily Curve", "Smooth Daily Curve"])

