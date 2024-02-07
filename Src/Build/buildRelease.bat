@echo off
SET SRCDIR=%~dp0
set SRCDIR=%SRCDIR:~0,-7%
set nugetfolder="%SRCDIR%\.nuget"
set packageFolder="%SRCDIR%\packages"
set configuration=Release

echo Before creating a release, remember to...
echo * Update release notes
echo.
pause
echo.

echo.
set /p AssemblyFileVersion=Please enter version number, eg 1.2.0.0:
set /p Version=Please enter nuget version number, eg 1.2.0: 
 
dotnet msbuild default.msbuild /t:BuildRelease

git status --porcelain |findstr . && echo "Repo contains unexpected modifications. Check that before publishing package!" && exit -1

echo -------------------------------
echo.
echo Created a new nuget package with version %Version% to output folder.
echo Assembly versions set to %AssemblyFileVersion%.
echo.
echo Remember to...
echo * Push nuget package to nuget server (and symbol server)
echo * (Update version number on docs)
echo * (Push new docs to docs repo)

echo.
pause