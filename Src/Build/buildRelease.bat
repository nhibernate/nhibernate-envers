@echo off
SET sn="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\sn.exe"
set gacutil="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\gacutil.exe"
set msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set configuration=Release
%msbuild% default.msbuild /v:n /t:Build;nuget
echo -------------------------------
pause