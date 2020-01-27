from datetime import datetime, timedelta
from spline_back import build

def create_contract(date, price):
    start = date
    end = date + timedelta(hours=23)
    return {
        'Start': start,
        'End': end,
        'Price': price
    }

if __name__ == "__main__":
    intercept = 45.7
    dailySlope = 0.8

    dailyPrice = intercept
    contractDay = datetime(2019, 5, 11)
    contracts = []

    for i in range(14):
        contracts.append(create_contract(contractDay, dailyPrice))
        contractDay + timedelta(days=1)
        dailyPrice += dailySlope

    weights = [(x['End'] - x['Start']).seconds/60 for x in contracts]
    multiAdj = [1]*len(weights)
    addAdj = [0]*len(weights)
    time_func = [()]
    curve = build(contracts, weights)


