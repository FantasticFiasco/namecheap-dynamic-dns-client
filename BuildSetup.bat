@echo off
set MSBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

%MSBuildDir%\MSBuild.exe DynDnsClient.proj /t:BuildSetup