
## Python PIP Package Releases
### 1.0.1
* Update pythonnet dependency from 2.4.0 to 2.5.1 (allows use of Python 3.8).

### 1.0.2
* Update .NET Cmdty.TimeSeries reference to avoid clash with cmdty-storage package.

### 1.0.3
* Update pythonnet dependency from 2.5.1 to 2.5.2 (allows use of Python 3.9 in latest Anaconda).

### 1.1.0
* Updates bootstrapper algorithm:
	* Change from least-squares solution to that closes to a target curve, being the smallest contract price for each period.
	* Allow caller to specify piecewise block periods.
	* Zero price is used as piecewise flat price for curve points in gaps. This fixes a bug, where the last price not in a gap is filled in.
