@echo off
set base_dir=C:\Users\baikangwang\Projects\CIMEL

cd %base_dir%\CIMEL\CIMEL.Setup
set output=%base_dir%\CIMEL\CIMEL.Setup\Release\
set dest=%base_dir%\CIMELPackage\
XCOPY /Y "%output%CIMEL_Install.msi" "%dest%"