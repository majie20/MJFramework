@echo off

SET _ZIP_PATH=.\Output
SET _COPY_PATH=..\..\Server\HttpServerTest\bin\Debug\net5.0\zip_src
del /Q %_COPY_PATH%

md %_COPY_PATH%

copy /y %_ZIP_PATH%\* %_COPY_PATH%

pause