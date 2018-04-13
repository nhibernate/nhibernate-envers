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

echo -------------------------------
echo.
echo Created a new nuget package with version %Version% to output folder.
echo Assembly versions set to %AssemblyFileVersion%.
echo Created html docs to output folder
echo.
echo Remember to...
echo * Tag current changeset with version %Version%
echo * Push changes (tag) to server repo
echo * Close and create a new release on JIRA
echo * Push nuget package to nuget server (and symbol server)
echo * Update version number on docs
echo * Push new docs to docs repo

echo.
pause