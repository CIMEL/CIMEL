@echo off
d:
cd D:\Working\Projects\Peach\CIMEL\CIMEL.Patch
set output=D:\Working\Projects\Peach\CIMEL\CIMEL.Patch\Release\
set dest=D:\Working\Projects\Peach\CIMEL\..\CIMELPackage\
XCOPY /Y "%output%CIMEL_Patch.msi" "%dest%"