@echo off
d:
cd D:\Working\Projects\Peach\CIMEL\CIMEL.Setup
set output=D:\Working\Projects\Peach\CIMEL\CIMEL.Setup\Release\
set dest=D:\Working\Projects\Peach\CIMEL\..\CIMELPackage\
XCOPY /Y "%output%CIMEL_Install.msi" "%dest%"