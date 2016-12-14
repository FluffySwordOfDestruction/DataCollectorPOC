@echo off
SC QUERY DataCollectorScheduler > NUL
IF ERRORLEVEL 1060 GOTO INSTALLING
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil /u "%~dp0DataCollector.Scheduler.exe"

:INSTALLING
echo Installing DataCollector Scheduler Windows Service...
echo ---------------------------------------------------
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil "%~dp0DataCollector.Scheduler.exe"
echo ---------------------------------------------------
pause
echo Done.