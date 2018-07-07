@echo off
set base_dir=C:\Users\baikangwang\Projects\CIMEL
cd %base_dir%\CIMEL\CIMEL.Installer
set output=%base_dir%\CIMEL\CIMEL.Installer\bin\Release\
set dest=%base_dir%\CIMEL\..\CIMELPackage\
XCOPY /Y "%output%installer.dll" "%dest%"