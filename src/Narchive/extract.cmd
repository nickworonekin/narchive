@echo off

if [%1] == [] goto :eof
if not exist "%~f1" (
	echo Could not find file '%1'.
	goto :eof
)

set /p output=Name of the folder to extract to, or leave blank to extract to the current folder: 

if [%output%] == [] (
    "%~dp0Narchive.exe" extract "%~1"
) else (
    "%~dp0Narchive.exe" extract --output "%output%" "%~1"
)