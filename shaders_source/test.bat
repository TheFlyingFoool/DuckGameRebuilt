@echo off
echo %~dp0
forfiles /m "*.fx" /c "cmd /c fxc.exe /T fx_2_0 @file /Fo %~dp0\..\bin\content\\shaders\\@fname.xnb"
