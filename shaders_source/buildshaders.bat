@echo off
cd %~dp0
forfiles /m "*.fx" /c "cmd /c fxc2.exe /T fx_2_0 @file /Fo %~dp0\..\bin\content\\shaders\\@fname.xnb"
