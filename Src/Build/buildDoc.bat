@echo off
SET SRCDIR=%~dp0
set SRCDIR=%SRCDIR:~0,-7%
set nugetfolder="%SRCDIR%\.nuget"
set packageFolder="%SRCDIR%\packages"
set msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set configuration=Release

echo Installing msbuildtasks to %PackageFolder%. Please wait...
%nugetFolder%\NuGet install MsBuildTasks -o %PackageFolder%

%msbuild% default.msbuild /v:n /t:BuildDocs
echo -------------------------------
pause