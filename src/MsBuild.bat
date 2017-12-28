@echo off
call update_version_prefix.vbs
echo Building NBearV3.sln, please wait a minute...
"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MsBuild.exe" NBearV3.sln /t:Rebuild /p:Configuration=Release > MsBuild.log
cd ..\dist
call ..\dist\UpdateAssemblies.bat