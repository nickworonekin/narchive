@echo off

if "%~1" == "" goto :eof
if not exist "%~1\*" (
	echo '%1' is not a folder.
	goto :error
)

:enteroutput
set /p output=Name of the NARC archive to create, including extension: 

if "%output%" == "" (
    echo Name cannot be blank.
    goto :enteroutput
)

set /p hasfilenames=Should entries within the NARC archive have filenames (y/n)?: 

if "%hasfilenames%" == "n" (
	"%~dp0Narchive.exe" create --nofilenames "%output%" "%~1"
) else (
    "%~dp0Narchive.exe" create "%output%" "%~1"
)

if %errorlevel% neq 0 goto :error

goto :eof

:error
pause
