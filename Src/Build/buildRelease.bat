@echo off
SET SRCDIR=%~dp0
set SRCDIR=%SRCDIR:~0,-7%
set nugetfolder="%SRCDIR%\.nuget"
set packageFolder="%SRCDIR%\packages"
set msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set configuration=Release
set msbuildtasksVersion=1.4.0.65

echo Before creating a release, remember to...
echo * Update release notes
echo * Update nuspec file to point to correct NH Core version
echo.
pause
echo.

echo Installing msbuildtasks to %PackageFolder%. Please wait...
%nugetFolder%\NuGet install MsBuildTasks -o %PackageFolder% -Version %msbuildtasksVersion%

echo.
set /p Version=Please enter version number, eg 1.2.0.0: 
set /p NugetVersion=Please enter nuget version number, eg 1.2.0: 

%msbuild% default.msbuild /v:q /t:BuildRelease

hg revert -C %SRCDIR%\NHibernate.Envers\Properties\AssemblyInfo.cs

echo -------------------------------
echo.
echo Created a new nuget package with version %nugetversion% to output folder.
echo Assembly versions set to %Version%.
echo Created html docs to output folder
echo.
echo Remember to...
echo * Tag current changeset with version %nugetversion%
echo * Push changes (tag) to server repo
echo * Close and create a new release on JIRA
echo * Push nuget package to nuget server (and symbol server)
echo * Update version number on docs
echo * Push new docs to docs repo

echo.
pause