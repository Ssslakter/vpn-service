@echo off
set serviceName=VpnIdeService

REM Check if the service is running
sc query %serviceName% | find "RUNNING" >nul

REM Check the error level to determine if the service is running
if %errorlevel%==0 (
    REM The service is running, stop it
    net stop %serviceName%
    echo %serviceName% has been stopped.
    REM Run the "ovpnconnector stop" command here
    ovpnconnector stop
) else (
    REM The service is not running, start it
    net start %serviceName%
    echo %serviceName% has been started.
)

pause
