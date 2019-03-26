@echo off 
set base_dir=C:\Users\baikangwang\Projects\CIMEL

cd %base_dir%\CIMEL\CIMEL.Chart
set output=%base_dir%\CIMEL\CIMEL.Chart\bin\Release\
set dest=%base_dir%\CIMEL\..\CIMELPackage\
XCOPY /Y "%output%CIMELData" "%dest%CIMELData"
XCOPY /Y "%output%Options" "%dest%Options"
XCOPY /Y "%output%大气气溶胶光学参数处理软件.exe" "%dest%"

pause;