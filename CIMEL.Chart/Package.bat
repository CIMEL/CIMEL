@echo off
d:
cd D:\Working\Projects\Peach\CIMEL\CIMEL.Chart
set output=D:\Working\Projects\Peach\CIMEL\CIMEL.Chart\bin\Release\
set dest=D:\Working\Projects\Peach\CIMEL\..\CIMELPackage\
XCOPY /Y "%output%CIMELData" "%dest%CIMELData"
XCOPY /Y "%output%Options" "%dest%Options"
XCOPY /Y "%output%CIMEL太阳光度计数据处理软件.exe" "%dest%"