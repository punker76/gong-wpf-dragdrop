@echo on
call "%VS120COMNTOOLS%vsvars32.bat"

msbuild.exe /ToolsVersion:4.0 "..\GongSolutions.Wpf.DragDrop.sln" /p:StrongName=True /p:configuration=Release /t:Clean,Rebuild
..\.nuget\NuGet.exe pack %~dp0..\GongSolutions.Wpf.DragDrop\GongSolutions.Wpf.DragDrop.nuspec -OutputDirectory %~dp0

msbuild.exe /ToolsVersion:4.0 "..\GongSolutions.Wpf.DragDrop.sln" /p:StrongName=True /p:configuration=Debug /t:Clean,Rebuild
..\.nuget\NuGet.exe pack %~dp0..\GongSolutions.Wpf.DragDrop\GongSolutions.Wpf.DragDrop.ALPHA.nuspec -OutputDirectory %~dp0

pause