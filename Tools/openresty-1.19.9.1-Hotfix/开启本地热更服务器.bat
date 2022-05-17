start nginx
pause

@REM 1、启动：
@REM start nginx
@REM 或
@REM nginx.exe

@REM 2、停止：
@REM nginx.exe -s stop(完整有序的停止nginx)
@REM 或
@REM nginx.exe -s quit(快速停止nginx) 
@REM 或
@REM taskkill /f /t /im nginx.exe

@REM 3、重新载入Nginx：
@REM nginx.exe -s reload

@REM 4、重新打开日志文件：
@REM nginx.exe -s reopen

@REM 5、查看Nginx版本：
@REM nginx -v

@REM 检查80端口是否被占用的命令是： netstat -ano | findstr 0.0.0.0:80 或 netstat -ano | findstr "80"