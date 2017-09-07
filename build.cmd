@echo off
tools\nuget\nuget.exe update -self
tools\nuget\nuget.exe install Cake -OutputDirectory tools -ExcludeVersion

IF "%1" == "" tools\Cake\Cake.exe build.cake
ELSE tools\Cake\Cake.exe build.cake --target=%1

exit /b %errorlevel%
