nlog.exe -l 1234 -vv
pause

@REM $ nlog.exe
@REM nlog [-l port]    - listen on port
@REM nlog [-c ip port] - connect to ip on port
@REM [-v]              - verbose output
@REM [-vv]             - even more verbose output

@REM # This will listen on port 1234
@REM $ nlog.exe -l 1234

@REM # This will connect to 127.0.0.1 on port 1234
@REM $ nlog.exe -c 127.0.0.1 1234