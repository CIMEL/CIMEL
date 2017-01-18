@echo off
d:
cd D:\Working\Projects\Peach\CIMEL\CIMEL.Setup
set output=D:\Working\Projects\Peach\CIMEL\CIMEL.Setup\Release\
set dest=D:\Working\Projects\Peach\CIMEL\..\CIMELPackage\
XCOPY /Y "%output%CIMEL太阳光度计数据处理软件.msi" "%dest%"