@echo off
d:
cd D:\Working\Projects\Peach\CIMEL\CIMEL.Installer
set output=D:\Working\Projects\Peach\CIMEL\CIMEL.Installer\bin\Release\
set dest=D:\Working\Projects\Peach\CIMEL\..\CIMELPackage\
XCOPY /Y "%output%installer.dll" "%dest%"