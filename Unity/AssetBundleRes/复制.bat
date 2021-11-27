@echo off

SET _ZIP_PATH=.\Output
SET _COPY_PATH=..\..\Server\HttpServerTest\bin\Debug\net5.0\zip_src
del %_PROTO2CS_PATH%\*.cs

md %_COPY_PATH%

copy /y %_ZIP_PATH%\* %_COPY_PATH%

pause