@echo on
call "%VS100COMNTOOLS%vsvars32.bat"

msbuild.exe /ToolsVersion:4.0 "..\GongSolutions.Wpf.DragDrop (NET35).sln" /p:StrongName=True /p:configuration=Release
msbuild.exe /ToolsVersion:4.0 "..\GongSolutions.Wpf.DragDrop (NET4).sln" /p:StrongName=True /p:configuration=Release

..\.nuget\NuGet.exe pack %~dp0..\GongSolutions.Wpf.DragDrop\GongSolutions.Wpf.DragDrop.nuspec -OutputDirectory %~dp0
pause