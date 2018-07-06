@echo off
c:
cd C:\Users\baikangwang\Projects\CIMEL\CIMEL\CIMEL.Setup
set output=C:\Users\baikangwang\Projects\CIMEL\CIMEL\CIMEL.Setup\Release\
set dest=C:\Users\baikangwang\Projects\CIMEL\CIMELPackage\
XCOPY /Y "%output%CIMEL_Install.msi" "%dest%"