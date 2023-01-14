
## Python PIP Package Releases
### 1.0.1
* Update pythonnet dependency from 2.4.0 to 2.5.1 (allows use of Python 3.8).

### 1.0.2
Update .NET Cmdty.TimeSeries reference to avoid clash with cmdty-storage package.

### 1.0.3
* Update pythonnet dependency from 2.5.1 to 2.5.2 (allows use of Python 3.9 in latest Anaconda).

### 1.1.0
* Updates bootstrapper algorithm:
	* Change from least-squares solution to find a solution that is closest to a target curve, rather than zero. 
	Each point on the target curve is derived as the price of the smallest contract that each period is within.
	* Zero price is used as piecewise flat price for curve points in gaps. This fixes a bug, where the last price not in a gap is filled in.

### 1.2.0
* Update pythonnet dependency from 2.5.1 to 3.0.1 to allow compatibility with Python up to version 3.11.

### 1.3.0 (Not yet released)
* .NET binaries included in Python package target .NET Standard, not .NET Framework, hence compatible with
more .NET types.
* For non-Windows OS default to trying .NET (Core), rather than Mono as default runtime.
