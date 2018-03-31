@echo off

if [%1] == [] goto :eof
if not exist "%~1" (
	echo Could not find file '%~1'.
	goto :error
)

set /p output=Name of the folder to extract to, or leave blank to extract to the current folder: 

if [%output%] == [] (
    "%~dp0Narchive.exe" extract "%~1"
) else (
    "%~dp0Narchive.exe" extract --output "%output%" "%~1"
)

if %errorlevel% neq 0 goto :error

goto :eof

:error
pause
